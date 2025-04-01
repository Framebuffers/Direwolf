using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace Direwolf.Revit.UI
{
    public static class Buttons
    {
        public static PushButton? AddModelSnapshotDBButton(RibbonPanel? rp)
        {
            PushButtonData b = new("modelToDB", "Send Model to DB", Libraries.References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetModelInfo");
            return rp?.AddItem(b) as PushButton;
        }
        public static PushButton? AddGetSelectedElementInfoButton(RibbonPanel? rp)
        {
            PushButtonData b = new("elementToDB", "Send Element to DB", Libraries.References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetSelectedElementInfo");
            return rp?.AddItem(b) as PushButton;
        }
        public static PushButton? AddGetInfoButton(RibbonPanel? rp)
        {
            PushButtonData b = new("direwolfInfo", "About", Libraries.References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetInfo");
            b.Image = new BitmapImage(new Uri("/about.png", UriKind.Relative));
            return rp?.AddItem(b) as PushButton;
        }
    }
}
