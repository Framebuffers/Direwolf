using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Extensions;
using Direwolf.Revit.Howls;
using Dynamo.PackageManager.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.UI.Commands
{
    public abstract partial class DirewolfRevitCommand : IExternalCommand
    {
        protected string? Descriptor { get; set; }

        protected void PrintAsmInfo()
        {
            $"GetType().Name: {this.GetType().FullName}\ntypeof.Asm.ProjectLocation:\n\t{typeof(DirewolfUIApp).Assembly.Location}\n\t{typeof(Direwolf).Assembly.Location}\n\t{typeof(RevitHowl).Assembly.Location}".ToConsole();
        }

        protected Stopwatch TimeTaken { get; set; } = new();
        protected void StartTime()
        {
            TimeTaken = new();
            TimeTaken.Start();
        }

        protected double StopTime()
        {
            TimeTaken.Stop();
            return TimeTaken.Elapsed.TotalSeconds;
        }

        public abstract Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements);
    }
}
