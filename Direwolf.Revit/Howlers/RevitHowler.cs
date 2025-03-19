using Direwolf.Contracts;
using Direwolf.Definitions;
using System.Text.Json;
using Direwolf.Revit.Contracts;
using Direwolf.EventHandlers;
using Autodesk.Revit.DB;
using System.Diagnostics;

namespace Direwolf.Revit.Howlers
{
    /// <summary>
    /// Exactly the same as a regular Howler, except that it checks if the Howl implements IDynamicRevitHowl.
    /// A bit of a hack but works.
    /// </summary>
    public record class RevitHowler : Howler
    {
        public new event EventHandler<HuntCompletedEventArgs>? HuntCompleted;
        private Document? _doc;
        private readonly Stopwatch _timeTaken = new();
        public override void CreateWolf(IWolf runner, IHowl instruction) // wolf factory
        {
            if (instruction is IRevitHowl)
            {
                runner.Instruction = instruction;
                IRevitHowl? i = instruction as IRevitHowl;
                _doc = i.GetRevitDocument();
                runner.Callback = this;
                Wolfpack.Enqueue(runner);
            }
            else
            {
                throw new ArgumentException("Howl is not a valid Revit Howl.");
            }
        }
        
        public override Wolfpack Howl(string testName)
        {
            _timeTaken.Start();
            try
            {

                string title = _doc?.Title ?? string.Empty;
                string path = _doc?.PathName ?? string.Empty;
                string version = _doc?.ProjectInformation.VersionGuid.ToString() ?? Guid.NewGuid().ToString();

                foreach (var wolf in Wolfpack)
                {
                    wolf.Run();
                }
                _timeTaken.Stop();
                Wolfpack w = new(this, title, path, version, true, _timeTaken.Elapsed.TotalSeconds)
                {
                    TestName = testName
                };
                HuntCompleted?.Invoke(this, new HuntCompletedEventArgs() { IsSuccessful = true});
                return w;
            }
            catch
            {
                HuntCompleted?.Invoke(this, new HuntCompletedEventArgs() { IsSuccessful = false});
                throw new ApplicationException();
            }
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(Den);
        }
    }
}

