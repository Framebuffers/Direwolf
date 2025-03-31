using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;

namespace Direwolf.Revit.UI.Commands
{
    public abstract class DirewolfRevitCommand : IExternalCommand
    {
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