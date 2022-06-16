namespace HostApi;

/// <summary>
/// This enumeration provides three levels of importance for messages.
/// </summary>
public enum DotNetMessageImportance
{
    /// <summary>High importance, appears in less verbose logs</summary>
    High,
    
    /// <summary>Normal importance</summary>
    Normal,
    
    /// <summary>Low importance, appears in more verbose logs</summary>
    Low,
}