using Autodesk.Revit.UI;
using Direwolf.Contracts;
using Direwolf.Revit.Contracts;
using Direwolf.Revit.Definitions.Primitives;
using Revit.Async.ExternalEvents;

namespace Direwolf.Revit.UI.EventHandlers;

public class ExecuteHuntEventHandler : SyncGenericExternalEventHandler<IWolf, IRevitWolfpack>
{
    public override string GetName()
    {
        return "RevitHunter";
    }

    public override object Clone()
    {
        return MemberwiseClone();
    }

    protected override IRevitWolfpack Handle(UIApplication app, IWolf parameter)
    {
        parameter.Hunt();
        return parameter.Result as IRevitWolfpack ?? new RevitWolfpack();
    }
}