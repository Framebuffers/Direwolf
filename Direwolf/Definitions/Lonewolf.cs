using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Direwolf.Contracts;
using Direwolf.EventHandlers;

namespace Direwolf.Definitions;

/// <summary>
///     Lonewolf creates wolves, taking a prototype Lonewolf, attaching a Howls (an instruction) and itself as a callback.
///     Then, to dispatch wolves, it executes a function inside each Lonewolf.
/// </summary>
public class Lonewolf : Stack<Wolfpack>, IHowler
{
    /// <summary>
    ///     Factory of <see cref="IWolf" /> workers. Takes a <see cref="IHowl" /> instruction, attaches it to the Callback of a
    ///     given <see cref="IWolf" />, and enqueues the resulting Lonewolf to the WolfQueue.
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="instruction"></param>
    public Lonewolf(IHowl instruction, ILonewolfConnector destination) // wolf factory
    {
        Wolf w = new() { Instruction = instruction, Callback = this };
        _connector = destination;
        WolfQueue.Enqueue(w);
        HuntCompleted += OnHuntCompleted;
    }

    /// <summary>
    /// <inheritdoc cref="Lonewolf"/>
    /// </summary>
    /// <param name="instructions"><inheritdoc cref="IHowl"/></param>
    /// <param name="destination"><<inheritdoc cref="IHowler"/></param>
    public Lonewolf(IHowl[] instructions, ILonewolfConnector destination)
    {
        foreach (var i in instructions)
        {
            Wolf w = new() { Instruction = i, Callback = this };
            WolfQueue.Enqueue(w);
        }

        HuntCompleted += OnHuntCompleted;
        _connector = destination;
    }

    /// <summary>
    ///     Sends a signal to <see cref="Direwolf" /> that the process has been completed,
    ///     to pass control to the serialization routines.
    /// </summary>
    public event EventHandler<HuntCompletedEventArgs>? HuntCompleted;

    /// <summary>
    ///     Queue of workers to be deployed.
    /// </summary>
    [JsonIgnore] private Queue<IWolf> WolfQueue { get; } = [];

    [JsonIgnore] private readonly ILonewolfConnector? _connector;

    /// <summary>
    ///     Performs the query. Summons all the workers held in <see cref="WolfQueue" />,
    ///     executes the <see cref="IWolf.Run" /> method inside each other, and waits back for them to come back.
    ///     When the process is completed, the <see cref="HuntCompleted" /> event is invoked, signalling the
    ///     <see cref="Direwolf" /> that the process has been completed and that a Wolfpack has been generated.
    /// </summary>
    /// <param name="testName">Name of the query to perform</param>
    /// <returns>A record containing metadata about the query with the results serialized inside</returns>
    /// <exception cref="ApplicationException">
    ///     Thrown whenever a worker has encountered an issue and the execution returns
    ///     false.
    /// </exception>
    public virtual async Task Awoo()
    {
        try
        {
            Stopwatch s = new();
            s.Start();
            foreach (var wolf in WolfQueue) wolf.Run();
            HuntCompleted?.Invoke(this, new HuntCompletedEventArgs { IsSuccessful = true });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task Create()
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
    
    public virtual async Task<Wolfpack[]?> Read()
    {
        if (_connector == null) throw new NullReferenceException();
        try
        {
            return _connector.Read();
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

    private async void OnHuntCompleted(object? sender, HuntCompletedEventArgs e)
    {
        try
        {
            if (e.IsSuccessful)
            {
                await this.Create();
            }
            else
            {
                throw new NullReferenceException();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}