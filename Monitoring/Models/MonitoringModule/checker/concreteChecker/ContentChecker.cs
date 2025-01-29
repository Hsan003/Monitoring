namespace Monitoring.Models.MonitoringModule.checker.ConcreteChecker;

public class ContentChecker : IChecker
{
    
    public string content { get; set; }
    private static readonly HttpClient _httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(10) 
    };
    private  MonitoringDbContext _context;

    
    public void initialize(Dictionary<string, object> param, MonitoringDbContext context)
    {
        _context = context;
        content = (string) param["content"];
    }
    public async Task<CheckResult> check(Website website)
    {
        CheckResult checkResult = new CheckResult
        {
            websiteId = website,
            Timestamp = DateTime.UtcNow
        };
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(website.Url);
            string webSiteContent = await response.Content.ReadAsStringAsync();
            if (webSiteContent.Contains(content))
            {
                checkResult.isUp = true;
            }
            else
            {
                checkResult.isUp = false;
                checkResult.error = "Content not found";
            }
        }
        catch (HttpRequestException e)
        {
            checkResult.error = e.Message;
            checkResult.isUp = false;
        }
        catch (TaskCanceledException)
        {
            checkResult.error = "Request timed out.";
            checkResult.isUp = false;
        }

        return checkResult;
    }
}