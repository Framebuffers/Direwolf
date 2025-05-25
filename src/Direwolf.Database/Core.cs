using System.Runtime.Caching;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;

using Direwolf.Database.EventArgs;
using Direwolf.Definitions;
using Direwolf.Definitions.Revit;

namespace Direwolf.Database;

// how to deal with documents?
// => track the object around?
// => serialize uuid at export?

public sealed partial class Core
{
    private static readonly object Lock = new();
    private static Core? _instance;
    private ControlledApplication? _controlledApplication;
    private Dispatcher? _dispatcher => Dispatcher.GetDispatcherInstance();
    public ControlledApplication? ControlledApplication
    {
        get => _controlledApplication;
        set
        {
            _controlledApplication = value!;
            InitCore();
        }
        
    }
    private Core()
    {
    }

    public static Core GetCoreInstance()
    {
        if (_instance is not null) return _instance;
        lock (Lock)
        {
            _instance ??= new Core();
        }
        return _instance;
    }
    
}

public sealed partial class Core
{
    private static readonly MemoryCache _revitElementCache = MemoryCache.Default;
    private static Queue<ElementToken> _revitElementQueue = [];
  
    public delegate void ApplicationReadyHandler();
    public event ApplicationReadyHandler ApplicationReady;
    

    public void OnDocumentChanged(Document doc)
    {
        
    }
    private static void InitCore()
    {
        
    }
}