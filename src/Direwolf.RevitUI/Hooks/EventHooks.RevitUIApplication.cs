using Autodesk.Revit.UI;

namespace Direwolf.RevitUI.Hooks;

public partial class EventHooks
{
    private readonly UIControlledApplication _ui;


    public List<Action> OnApplicationStartup = [];
    public List<Action> OnApplicationShutdown = [];

    private void a()
    {
    }
}