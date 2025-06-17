namespace Direwolf;

//TODO: receive JSON from MCP source, use MCP JSON schema and JSON-RCP standards
//
public sealed class Hunter
{
    private static readonly object Lock = new();
    private static Hunter? _instance;

    private Hunter()
    {
    }

    public static Hunter GetInstance()
    {
        if (_instance is not null) return _instance;
        lock (Lock)
        {
            if (_instance is not null) return _instance;
            _instance = new Hunter();
            return _instance;
        }
    }
    
    
}