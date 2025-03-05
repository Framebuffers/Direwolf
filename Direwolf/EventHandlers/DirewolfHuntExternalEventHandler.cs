using Autodesk.Revit.UI;
using Direwolf.Contracts;
using Direwolf.Contracts.Dynamics;
using Direwolf.Definitions;
using Direwolf.Definitions.Dynamics;
using Revit.Async.ExternalEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Direwolf.EventHandlers
{
    public class DirewolfHuntExternalEventHandler : SyncGenericExternalEventHandler<Wolfpack, bool>
    {
        public override object Clone()
        {
            return new Direwolf();
        }

        public override string GetName()
        {
            return "DirewolfHunter";
        }

        protected override bool Handle(UIApplication app, Wolfpack parameter)
        {
            TaskDialog t = new("Hunt Complete")
            {
                MainContent = $"The task has been ran successfully"
            };
            t.Show();
            return true;  
        }
    }
}
