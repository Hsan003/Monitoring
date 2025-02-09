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
    


    public void initialize(string content, int retires)
    {
        timeout = 10;
        _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
    }
    public async Task<CheckResults> check(Website website)
    {
        CheckResults result = new CheckResults();
        result.websiteId = website.Id;
        result.Timestamp = DateTime.Now;
        try
        {
            var start = DateTime.Now;
            HttpResponseMessage response = await _httpClient.GetAsync(website.Url);
            result.status = response.StatusCode;
            var end = DateTime.Now;
            if (!response.IsSuccessStatusCode)
            {
                result.ErrorMessage = response.ReasonPhrase ?? "Unknown error";
            }
            else
            {
                result.ResponseTime = (int)(end - start).TotalMilliseconds;
                result.ErrorMessage = null;
            }

            result.isUp = response.IsSuccessStatusCode;

            
        }
        catch (HttpRequestException e)
        {
            result.ErrorMessage = e.Message;
            result.isUp = false;
        }
        catch (TaskCanceledException)
        {
            result.ErrorMessage = "Request timed out.";
            result.isUp = false;
        }
        catch (Exception e)
        {
            result.ErrorMessage = e.Message;
            result.isUp = false;
        }
        Console.WriteLine("HTTPStatusChecker: " + result);
        if(result.ErrorMessage == null)
            result.ErrorMessage = "No error";
        
        return result;
    }
}