using Autodesk.Revit.UI;

namespace Direwolf.Revit.UI
{
    public static class Buttons
    {
        public static PushButton? AddModelSnapshotDBButton(RibbonPanel? rp)
        {
            PushButtonData b = new("modelToDB", "Send Model to DB", Libraries.References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetModelSnapshotToDB");
            return rp?.AddItem(b) as PushButton;
        }
        public static PushButton? AddModelSnapshotJsonButton(RibbonPanel? rp)
        {
            PushButtonData b = new("modelHealth", "Send info to JSON", Libraries.References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetModelSnapshotToJson");
            return rp?.AddItem(b) as PushButton;
        }
        public static PushButton? AddElementInformationButton(RibbonPanel? rp)
        {
            PushButtonData b = new("elementInfo", "Send Parameters to DB", Libraries.References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetSelectedElementInfo");
            return rp?.AddItem(b) as PushButton;
        }
    }

}
