using Microsoft.EntityFrameworkCore;

namespace Monitoring.Models.MonitoringModule.checker.ConcreteChecker;
using System;
using System.Net.Http;
using System.Threading.Tasks;
public class HTTPStatusChecker : IChecker
{
    public int timeout { get; set; }
    public int retries { get; set; }
    private HttpClient _httpClient = new HttpClient();
    


    public void initialize(string content, int retires, MonitoringDbContext context)
    {
        timeout = 10;
        _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
    }
    public async Task<CheckResult> check(Website website)
    {
        CheckResult result = new CheckResult();
        result.websiteId = website.Id;
        result.Timestamp = DateTime.Now;
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(website.Url);
            result.status = response.StatusCode;
            if (!response.IsSuccessStatusCode)
            {
                result.error = response.ReasonPhrase ?? "Unknown error";
            }
            else
            {
                result.error = null;
            }

            result.isUp = response.IsSuccessStatusCode;

            
        }
        catch (HttpRequestException e)
        {
            result.error = e.Message;
            result.isUp = false;
        }
        catch (TaskCanceledException)
        {
            result.error = "Request timed out.";
            result.isUp = false;
        }
        Console.WriteLine("HTTPStatusChecker: " + result);
        if(result.error == null)
            result.error = "No error";
        
        return result;
    }
}