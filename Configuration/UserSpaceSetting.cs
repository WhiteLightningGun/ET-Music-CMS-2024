namespace UpLoader_For_ET.Configuration;

/// <summary>
/// Provides a means by which user upload space can be limited via the appsettings.json, use megabytes.
/// </summary>
public class UserSpaceLimitSetting
{
    public int? userLimitMB { get; set; }
}
