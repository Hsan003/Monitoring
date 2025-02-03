namespace Monitoring.Models.MonitoringModule.checker.ConcreteChecker;

public class ContentChecker : IChecker
{
    
    public string _content { get; set; }
    public int _retries { get; set; }
    private static readonly HttpClient _httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(10) 
    };
    private  MonitoringDbContext _context;

    
    public void initialize(string content,int retries, MonitoringDbContext context)
    {
        _context = context;
        _content = content;
        _retries = retries;

    }
    public async Task<CheckResult> check(Website website)
    {
        CheckResult checkResult = new CheckResult
        {
            websiteId = website.Id,
            Timestamp = DateTime.UtcNow
        };
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(website.Url);
            string webSiteContent = await response.Content.ReadAsStringAsync();
            if (webSiteContent.Contains(_content))
            {
                checkResult.isUp = true;
            }
            else
            {
                checkResult.isUp = false;
                checkResult.error = "Content not found";
            }
            _context.CheckResults.Add(checkResult);
            await _context.SaveChangesAsync();
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