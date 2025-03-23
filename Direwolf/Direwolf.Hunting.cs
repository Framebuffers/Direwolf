using Direwolf.Contracts;
using Direwolf.Definitions;
using Direwolf.EventHandlers;
using Direwolf.Extensions;
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
        public void QueueHowler(IHowler howler)
        {
            ArgumentNullException.ThrowIfNull(howler);
            ArgumentNullException.ThrowIfNull(howler.FinalTarget);
            Howlers.Enqueue(howler);
            howler.HuntCompleted += OnHuntCompleted;
        }
        public void Hunt(string testName)
        {
            try
            {
                if (Howlers.Count > 0)
                {
                    foreach (var howler in Howlers)
                    {
                        $"Hunting: {howler.Wolfpack.Count}".ToConsole();
                        Hunt(howler, testName);
                        var h = new HowlId()
                        {
                            HowlIdentifier = new Guid(),
                            Name = howler.GetType().Name
                        };
                        PreviousHowls.Add(h);
                        $"Added to Wolfden: {h.Name}".ToConsole();
                    }
                }
                else
                {
                    throw new Exception("No Howlers queued for dispatch.");
                }
            }
            catch (Exception e)
            {
                $"Exception thrown: {e.Message}".ToConsole();
            }
        }
        public void Hunt(IHowler dispatch, string testName)
        {
            try
            {
                var query = dispatch.Howl(testName);
                var h = new HowlId()
                {
                    HowlIdentifier = new Guid(),
                    Name = dispatch.GetType().Name
                };
                PreviousHowls.Add(h);
                Queries.Push(query);
            }
            catch (Exception e)
            {
                $"Exception thrown: {e.Message}".ToConsole();
            }
        }
        private void OnHuntCompleted(object? sender, HuntCompletedEventArgs e)
        {
            Debug.Print($"Hunt result: {e.IsSuccessful}\nTarget:{e.Where}");
        }
    }
}
