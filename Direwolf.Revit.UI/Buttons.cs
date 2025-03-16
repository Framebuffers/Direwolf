using Autodesk.Private.Windows.ToolBars;
using Autodesk.Revit.UI;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.UI
{
    public static class Buttons
    {
        public static PushButton? AddGetModelHealthButton(RibbonPanel? rp)
        {
            PushButtonData b = new("modelHealth", "Get Model Health", References.DirewolfRevitLocation, "Direwolf.Revit.Benchmarking.BaseCommandSanityCheck");
            return rp?.AddItem(b) as PushButton;
        }

        //public static PushButton? AddGetAnnotativeElementsButton(RibbonPanel? rp)
        //{
        // PushButtonData b = new("modelHealth", "Get Annotative Elements", References.DirewolfRevitLocation, "Direwolf.Revit.Benchmarking.CheckHelpers");
    
        //}


    }
}
