using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Revit.Contracts;
using Revit.Async.ExternalEvents;

namespace Direwolf.Revit.UI.EventHandlers;

public class HowlEventHandler : SyncGenericExternalEventHandler<IRevitHowl, bool>
{
    public override string GetName()
    {
        return "HowlEventHandler";
    }

    public override object Clone()
    {
        return MemberwiseClone();
    }

    protected override bool Handle(UIApplication app, IRevitHowl parameter)
    {
        var doc = app.ActiveUIDocument.Document;
        try
        {
            parameter.SetRevitDocument(doc);
            parameter.Execute();
            return true;
        }
        catch
        {
            return false;
        }
    }
}