using System.Runtime.CompilerServices;

using Autodesk.Revit.ApplicationServices;

namespace Direwolf.Database;

// handle signals
public sealed class Dispatcher
{
    private static readonly object Lock = new();
    private static Dispatcher? _instance;
    private Dispatcher() {  }
    
    
    public delegate void                  ApplicationReadyHandler();
    public event ApplicationReadyHandler? ApplicationReady;
    private static ControlledApplication? _app;
    
    public static Dispatcher GetDispatcherInstance()
    {
        if (_instance is not null)
        {
            _instance.ApplicationReady?.Invoke();
            return _instance;
        }
        lock (Lock)
        {
            _instance ??= new Dispatcher();
        }
        _instance.ApplicationReady?.Invoke();
        return _instance;
    } 
}