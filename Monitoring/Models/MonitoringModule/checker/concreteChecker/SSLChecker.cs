using System.Diagnostics;

namespace Monitoring.Models.MonitoringModule.checker.ConcreteChecker;
using System;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;

public class SSLChecker : IChecker
{
    private MonitoringDbContext _context;

        public void initialize(Dictionary<string, object> param, MonitoringDbContext context)
        {
            _context = context;
        }
    public async Task<CheckResult> check(Website website )
        {
            Console.WriteLine($"Checking SSL status for {website.Url}...");
            CheckResult checkResult = new CheckResult
            {
                websiteId = website,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                var stopwatch = Stopwatch.StartNew();

                using (var httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    {
                        if (sslPolicyErrors != SslPolicyErrors.None)
                        {
                            checkResult.error = $"SSL Policy Error: {sslPolicyErrors}";
                            Console.WriteLine(checkResult.error);
                            return false;
                        }

                        if (cert != null)
                        {
                            Console.WriteLine($"Certificate Issuer: {cert.Issuer}");
                            Console.WriteLine($"Certificate Subject: {cert.Subject}");
                            Console.WriteLine($"Valid From: {cert.GetEffectiveDateString()}");
                            Console.WriteLine($"Valid To: {cert.GetExpirationDateString()}");

                            var expirationDate = DateTime.Parse(cert.GetExpirationDateString());
                            if (expirationDate < DateTime.UtcNow)
                            {
                                checkResult.error = "Certificate has expired.";
                                Console.WriteLine(checkResult.error);
                                return false;
                            }
                        }

                        return true; 
                    };

                    using (var httpClient = new HttpClient(httpClientHandler))
                    {
                        var response = httpClient.GetAsync(website.Url).Result;
                        stopwatch.Stop(); 
                        checkResult.responseTime = (int)stopwatch.ElapsedMilliseconds;
                        checkResult.status = response.StatusCode;

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("SSL check passed.");
                            checkResult.isUp = true;
                        }
                        else
                        {
                            Console.WriteLine("SSL check failed.");
                            checkResult.isUp = false;
                            checkResult.error = "Non-successful HTTP status code received.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                checkResult.isUp = false;
                checkResult.error = $"Error during SSL check: {ex.Message}";
                Console.WriteLine(checkResult.error);
            }

            return checkResult;
        }
}
