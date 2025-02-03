using System.Collections.Concurrent;
using Monitoring.Models.MonitoringModule.checker;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Monitoring.Models;
using System.Data.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Monitoring.Models.MonitoringModule.checker.ConcreteChecker;

public class CheckerFactory
{
    private readonly MonitoringDbContext _dbContext;

    public CheckerFactory(MonitoringDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    

    public IChecker CreateChecker(string checkerClass)
    {
        IChecker checker;
        if (checkerClass.ToLower() == "contentchecker")
        {
            checker = new ContentChecker();
        }
        else if (checkerClass.ToLower() == "sslchecker")
        {
            checker = new SSLChecker();
        }
        else if (checkerClass.ToLower() == "httpstatuschecker")
        {
            checker = new HTTPStatusChecker();
        }
        else
        {
            return null;
        }
        return checker;

    }
}
