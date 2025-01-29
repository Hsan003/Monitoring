using System.Collections.Concurrent;
using System.Data;
using Monitoring.Models.MonitoringModule.checker;
using System.Text.Json;
public class CheckerFactory
{
    private readonly ConcurrentDictionary<string, Func<IChecker>> _registry = new ConcurrentDictionary<string, Func<IChecker>>();

    public CheckerFactory(IDbConnection dbConnection)
    {
        LoadCheckersFromDatabase(dbConnection);
    }

    private void LoadCheckersFromDatabase(IDbConnection dbConnection)
    {
        const string sql = "SELECT checker_key, class_name, parameters FROM checker_entity";

        try
        {
            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;

                dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string key = reader["checker_key"].ToString();
                        string className = reader["class_name"].ToString();
                        string parametersJson = reader["parameters"].ToString();

                        var parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(parametersJson);

                        RegisterChecker(key, className, parameters);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to load checkers from database", ex);
        }
        finally
        {
            dbConnection.Close();
        }
    }

    private void RegisterChecker(string key, string className, Dictionary<string, object> parameters)
    {
        try
        {
            var type = Type.GetType(className);
            if (type == null)
            {
                throw new Exception($"Checker class not found: {className}");
            }

            _registry[key] = () =>
            {
                try
                {
                    // Create an instance of the checker
                    IChecker checker = (IChecker)Activator.CreateInstance(type);
                    checker.initialize(parameters, new MonitoringDbContext());

                    return checker;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to instantiate checker: {className}", ex);
                }
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error registering checker: {key}", ex);
        }
    }

    public IChecker CreateChecker(string checkerKey)
    {
        if (!_registry.TryGetValue(checkerKey, out var supplier))
        {
            throw new ArgumentException($"Unknown checker: {checkerKey}");
        }

        return supplier.Invoke();
    }
}

public abstract class Checker
{
    public abstract void Initialize(Dictionary<string, object> parameters);
}
