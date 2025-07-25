namespace Direwolf;

/// <summary>
/// Direwolf's Server. Handles any external communications between Direwolf and any third-party program.
/// <list type="bullet">
///     <item>
///         <term>Connection with an external PostgreSQL database.</term>
///     </item>
/// </list>
/// </summary>
public class Hunter
{
    private static readonly object Lock = new();
    private static Hunter? _instance;
    private Hunter() { }

    public static Hunter GetInstance()
    {
        if (_instance is not null) return _instance;
        lock(Lock)
        {
            if (_instance is not null) return _instance;
            _instance = new Hunter(); 
            return _instance;
        }
    }
}