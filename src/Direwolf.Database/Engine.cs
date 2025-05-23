using System.Diagnostics;
using System.Runtime.Caching;

using Autodesk.Revit.DB;

using Direwolf.Definitions.Internal;

namespace Direwolf.Database;


public sealed class Engine
{
    public static event EventHandler EngineReadyEventArgs;
    
    private static readonly object Lock = new object();
    private static Engine? _instance;
    private static List<Token> _tokens = [];

    private Engine()
    {
        Debug.Print("Initializing Engine");
    }

    public static Engine GetEngine()
    {
        if (_instance is null)
        {
            lock (Lock)
            {
                if (_instance is null)
                {
                    _instance = new Engine();
                }
            }
            return _instance;
        }
        return _instance;
    }
    
}