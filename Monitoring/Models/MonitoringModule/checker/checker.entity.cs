namespace Monitoring.Models.MonitoringModule.checker;
using System.Text.Json.Serialization;

public class checker_entity
{
    public string checker_key { get; set; }
    public string class_name { get; set; }
    [JsonPropertyName("parameters")]
    public Dictionary<string, object> Parameters { get; set; }
}