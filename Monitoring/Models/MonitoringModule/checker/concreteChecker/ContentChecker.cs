namespace Monitoring.Models.MonitoringModule.checker.ConcreteChecker;

public class ContentChecker : IChecker
{
    
    public string _content { get; set; }
    public int _retries { get; set; }
    private static readonly HttpClient _httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(10) 
    };

    
    public void initialize(string content,int retries)
    {
        _content = content;
        _retries = retries;

    }
    public async Task<CheckResults> check(Website website)
    {
        CheckResults checkResult = new CheckResults
        {
            websiteId = website.Id,
            Timestamp = DateTime.UtcNow
        };
        try
        {
            var start = DateTime.Now;
            HttpResponseMessage response = await _httpClient.GetAsync(website.Url);
            string webSiteContent = await response.Content.ReadAsStringAsync();
            checkResult.status = response.StatusCode;
            var end = DateTime.Now;
            if (webSiteContent.Contains(_content))
            {
                checkResult.isUp = true;
                
            }
            else
            {
                checkResult.isUp = true;
                checkResult.ErrorMessage = "Content not found";
            }
            
            checkResult.ResponseTime = (int)(end - start).TotalMilliseconds;
        }
        catch (HttpRequestException e)
        {
            checkResult.ErrorMessage = e.Message;
            checkResult.isUp = false;
        }
        catch (TaskCanceledException)
        {
            checkResult.ErrorMessage = "Request timed out.";
            checkResult.isUp = false;
        }
        catch (Exception e)
        {
            checkResult.ErrorMessage = e.Message;
            checkResult.isUp = false;
        }

        return checkResult;
    }
}