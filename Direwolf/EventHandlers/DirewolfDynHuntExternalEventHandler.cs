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
    public class DirewolfDynHuntExternalEventHandler : SyncGenericExternalEventHandler<IDynamicHowler, DynamicWolfpack>
    {
        public override object Clone()
        {
            return new Direwolf();
        }

        public override string GetName()
        {
            return "DynamicDirewolfHunter";
        }

        protected override DynamicWolfpack Handle(UIApplication app, IDynamicHowler parameter)
        {
            var result = parameter.Howl();
            TaskDialog t = new("Hunt Complete")
            {
                MainContent = $"The task has been ran successfully"
            };
            t.Show();
            return result;  
        }
    }
}
