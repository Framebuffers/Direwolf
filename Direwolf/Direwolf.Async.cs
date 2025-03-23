using Direwolf.Contracts;
using Direwolf.Definitions;
using Direwolf.EventHandlers;
using Direwolf.Extensions;
using Revit.Async;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf
{
    public partial class Direwolf
    {
        public async void HuntAsync(string queryName = "query")
        {
            HuntCompletedEventHandler += OnHuntCompleted;
            Revit.Async.RevitTask.Initialize(_app);
            
            foreach (var howler in Howlers)
            {
                await HuntTask(howler, queryName);
            }
        }
        public async void HuntAsync(IHowler howler, string queryName = "query")
        {
            Revit.Async.RevitTask.Initialize(_app);
            await HuntTask(howler, queryName);
        }
        private async Task HuntTask(IHowler howler, string queryName = "query")
        {
            //try
            //{
                Revit.Async.RevitTask.Initialize(_app);
                await RevitTask.RunAsync(() =>
                {
                    var results = howler.Howl(queryName);
                    var h = new HowlId()
                    {
                        HowlIdentifier = new Guid(),
                        Name = howler.GetType().Name
                    };
                    PreviousHowls.Add(h);
                    Queries.Push(results);
                });
                HuntCompletedEventHandler?.Invoke(this, new HuntCompletedEventArgs() { IsSuccessful = true, Where = howler.FinalTarget });
            //}
            //catch (Exception e) { Debug.WriteLine(e.Message); }
        }
    }
}
