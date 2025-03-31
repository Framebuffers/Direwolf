using Autodesk.Revit.UI;

namespace Direwolf.Revit.UI
{
    public static class Buttons
    {
        public static PushButton? AddTrackThisElementButton(RibbonPanel? rp)
        {
            PushButtonData b = new("track", "Track this Element", References.AssemblyLocation, "Direwolf.Revit.UI.Commands.TrackThisElement");
            return rp?.AddItem(b) as PushButton;
        }
        public static PushButton? AddTrackThisDocumentButton(RibbonPanel? rp)
        {
            PushButtonData b = new("pushToDB", "Track this Document's Information", References.AssemblyLocation, "Direwolf.Revit.UI.Commands.TrackThisDocument");
            return rp?.AddItem(b) as PushButton;
        }
        public static PushButton? AddDocumentToJsonButton(RibbonPanel? rp)
        {
            PushButtonData b = new("documentToJson", "Extract Parameters to JSON", References.AssemblyLocation, "Direwolf.Revit.UI.Commands.DocumentToJson");
            return rp?.AddItem(b) as PushButton;
        }
        public static PushButton? AddDocumentSnapshotButton(RibbonPanel? rp)
        {
            PushButtonData b = new("documentSnapshot", "Extract Parameters to DB", References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetDocumentSnapshot");
            return rp?.AddItem(b) as PushButton;
        }
    }
}
