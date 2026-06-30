namespace Api.Jobs;

public class ReleaseStageMonitorOptions
{
    public const string SectionName = "ReleaseStageMonitor";

    public bool Enabled { get; set; } = true;

    public string Cron { get; set; } = "0 * * * *";
}
