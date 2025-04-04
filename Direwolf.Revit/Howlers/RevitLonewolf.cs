using System.Diagnostics;
using System.Text.Json;
using Autodesk.Revit.DB;
using Direwolf.Contracts;
using Direwolf.Definitions;
using Direwolf.EventHandlers;
using Direwolf.Revit.Contracts;

namespace Direwolf.Revit.Howlers;

/// <summary>
///     Exactly the same as a regular Lonewolf, except that it checks if the Howl implements IDynamicRevitHowl.
/// </summary>
public record RevitLonewolf : Lonewolf
{
    private readonly Stopwatch _timeTaken = new();
    private Document? _doc;
    public new event EventHandler<HuntCompletedEventArgs>? HuntCompleted;

    /// <summary>
    ///     Create a runner that holds a Revit Document.
    /// </summary>
    /// <param name="runner">Runner</param>
    /// <param name="instruction">Query to be executed</param>
    /// <exception cref="ArgumentException">Thrown when a given Instruction is not a valid Revit Instruction</exception>
    public override void CreateWolf(IWolf runner, IHowl instruction) // wolf factory
    {
        if (instruction is IRevitHowl)
        {
            runner.Instruction = instruction;
            var i = instruction as IRevitHowl;
            _doc = i.GetRevitDocument();
            runner.Callback = this;
            WolfQueue.Enqueue(runner);
        }
        else
        {
            throw new ArgumentException("Howl is not a valid Revit Howl.");
        }
    }

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <param name="testName">
    ///     <inheritdoc />
    /// </param>
    /// <returns>
    ///     <inheritdoc />
    /// </returns>
    /// <exception cref="ApplicationException">
    ///     <inheritdoc />
    /// </exception>
    public override Wolfpack Howl(string testName)
    {
        _timeTaken.Start();
        try
        {
            var title = _doc?.Title ?? string.Empty;
            var path = _doc?.PathName ?? string.Empty;
            var version = _doc?.ProjectInformation.VersionGuid.ToString() ?? Guid.NewGuid().ToString();

            foreach (var wolf in WolfQueue) wolf.Run();
            _timeTaken.Stop();
            Wolfpack w = new(this, title, path, version, true, _timeTaken.Elapsed.TotalSeconds)
            {
                TestName = testName
            };
            HuntCompleted?.Invoke(this, new HuntCompletedEventArgs { IsSuccessful = true });
            return w;
        }
        catch
        {
            HuntCompleted?.Invoke(this, new HuntCompletedEventArgs { IsSuccessful = false });
            throw new ApplicationException();
        }
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(Den);
    }
}