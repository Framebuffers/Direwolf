using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Direwolf.Contracts;
using Direwolf.Definitions;
using Direwolf.EventHandlers;

namespace Direwolf;

/// <summary>
///     Direwolf creates wolves, taking a prototype Direwolf, attaching a Howls (an instruction) and itself as a callback.
///     Then, to dispatch wolves, it executes a function inside each Direwolf.
/// </summary>
public class Direwolf : Stack<Prey>, IHowler
{
    #region Hunting

    protected Stopwatch TimeTaken { get; } = new();

    /// <summary>
    ///     Performs the query. Summons all the workers held in <see cref="WolfQueue" />,
    ///     executes the <see cref="Wolf.Run" /> method inside each other, and waits back for them to come back.
    ///     When the process is completed, the <see cref="HuntCompleted" /> event is invoked, signalling the
    ///     <see cref="Direwolf" /> that the process has been completed and that <see cref="IDirewolfConnector.Create" />
    ///     will be called.
    /// </summary>
    public virtual async Task Awoo()
    {
        try
        {
            TimeTaken.Start();
            foreach (var wolf in WolfQueue) wolf.Run();
            HuntCompleted?.Invoke(this, new HuntCompletedEventArgs(isSuccessful: true));
            TimeTaken.Stop();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion

    #region Overrides

    public override string ToString()
    {
        return JsonSerializer.Serialize(this.Select(x => x.Result));
    }

    #endregion

    #region Constructors

    /// <summary>
    ///     Factory of <see cref="Wolf" /> workers. Takes a <see cref="IHowl" /> instruction, attaches it to the Callback of a
    ///     given <see cref="Wolf" />, and enqueues the resulting Direwolf to the WolfQueue.
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="instruction"></param>
    protected Direwolf(IHowl instruction, IDirewolfConnector destination) // wolf factory
    {
        Wolf w = new() { Instruction = instruction, Callback = this };
        _connector = destination;
        WolfQueue.Enqueue(w);
        HuntCompleted += OnHuntCompleted;
    }

    /// <summary>
    ///     <inheritdoc cref="Direwolf" />
    /// </summary>
    /// <param name="instructions">
    ///     <inheritdoc cref="IHowl" />
    /// </param>
    /// <param name="destination"><<inheritdoc cref="IHowler" />
    /// </param>
    protected Direwolf(IHowl[] instructions, IDirewolfConnector destination)
    {
        foreach (var i in instructions)
        {
            Wolf w = new() { Instruction = i, Callback = this };
            WolfQueue.Enqueue(w);
        }

        HuntCompleted += OnHuntCompleted;
        _connector = destination;
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Queue of workers to be deployed.
    /// </summary>
    [JsonIgnore] protected Queue<Wolf> WolfQueue { get; } = [];

    /// <summary>
    ///     Connection to an external source to offload generated data.
    /// </summary>
    [JsonIgnore] protected IDirewolfConnector? _connector;

    /// <summary>
    ///     Serialized and processed results from a Hunt. While the class itself holds the ray <see cref="Prey" />
    ///     records in a stack, a <see cref="Wolfpack" /> includes metadata about the hunting process and a
    ///     unique identifier for the query.
    /// </summary>
    public Wolfpack GenerateResults(string resultsName)
    {
        var g = Guid.NewGuid();
        
        return new Wolfpack
        {
            Name = resultsName,
            CreatedAt = DateTime.UtcNow,
            Guid = g,
            WasCompleted = true,
            TimeTaken = TimeTaken.Elapsed.TotalSeconds,
            Data = JsonSerializer.Serialize(this)
        };
    }

    #endregion

    #region CRUD

    public async Task Create()
    {
        try
        {
            if (_connector == null) throw new NullReferenceException();
            _connector.Create(this);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual async Task<Prey[]?> Read()
    {
        if (_connector == null) throw new NullReferenceException();
        try
        {
            return _connector.Read(this);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual async Task<bool> Update()
    {
        try
        {
            return _connector.Update(this);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual async Task<bool> Destroy()
    {
        try
        {
            return _connector.Destroy(this);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion

    #region Events

    /// <summary>
    ///     Sends a signal to <see cref="Direwolf" /> that the process has been completed,
    ///     to pass control to the serialization routines.
    /// </summary>
    public event EventHandler<HuntCompletedEventArgs>? HuntCompleted;

    protected void OnHuntCompleted(object? sender, HuntCompletedEventArgs e)
    {
        try
        {
            if (e.IsSuccessful)
                Create();
            else
                throw new NullReferenceException();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    #endregion
}