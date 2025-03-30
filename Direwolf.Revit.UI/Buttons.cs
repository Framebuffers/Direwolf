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


        //public static PushButton? AddGetAnnotativeElementsButton(RibbonPanel? rp)
        //{
        // PushButtonData b = new("modelHealth", "Get Annotative Elements", References.DirewolfRevitLocation, "Direwolf.Revit.Benchmarking.DocumentExtensions");

        //}


    }
}
