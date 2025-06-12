using System.Collections.Concurrent;
using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.RevitApi;

namespace Direwolf;

public sealed class Wolfden(Document document) : ConcurrentDictionary<Guid, RevitElement?>
{
    private static Wolfden? _instance;
    private readonly Queue<Wolfpack?> _operationQueue = [];

    private readonly Stack<RevitElement?> _transactionStack = [];

    public static Wolfden CreateInstance(Document document)
    {
        var wolfden = new Wolfden(document);
        _instance = wolfden;
        if (!wolfden.PopulateDatabase()) throw new InvalidOperationException(nameof(Wolfden));
        return new Wolfden(document);
    }

    public Wolfden GetInstance()
    {
        return _instance ?? throw new NullReferenceException(nameof(Wolfden));
    }

    private bool PopulateDatabase()
    {
        try
        {
            var revitDb = document.GetRevitDatabase().ToList();
            foreach (var cacheItem in revitDb)
            {
                if (cacheItem is null || !Guid.TryParse(cacheItem.Value.ElementUniqueId, out var elementUniqueId))
                    continue;
                GetOrAdd(elementUniqueId, RevitElement.Create(document, elementUniqueId.ToString()));
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public Response AddOrUpdateElements(Howl h, out IEnumerable<RevitElement?>? elements)
    {
        try
        {
            if (h.Payload is null)
            {
                elements = null;
                return Response.Error;
            }

            var results = Extract(h) ?? throw new NullReferenceException(nameof(RevitElement));
            foreach (var element in results)
            {
                if (element is null) continue;
                AddOrUpdate(Guid.Parse(element.Value.ElementUniqueId), guid =>
                {
                    // ReSharper disable once ConvertToLambdaExpression
                    return RevitElement.Create(document, guid.ToString());
                }, (guid, revitElement) =>
                {
                    _transactionStack.Push(this[guid]); // save the old value
                    return this[guid] = revitElement;
                });
            }

            elements = results;
            return Response.Result;
        }
        catch
        {
            elements = null;
            return Response.Error;
        }
    }

    public Response RemoveElements(Howl h, out RevitElement?[]? element)
    {
        if (h.Payload is null)
        {
            element = null;
            return Response.Error;
        }

        var results = Extract(h) ?? throw new NullReferenceException(nameof(RevitElement));
        List<RevitElement?> output = [];
        foreach (var result in results)
        {
            if (result?.ElementUniqueId is null) continue;
            TryRemove(Guid.Parse(result.Value.ElementUniqueId), out var extracted);
            output.Add(extracted);
        }

        element = output.ToArray();
        return Response.Result;
    }

    public bool PopTransaction(out RevitElement? element)
    {
        return _transactionStack.TryPop(out element);
    }

    public void SpoolWolfpack(Wolfpack? w)
    {
        _operationQueue.Enqueue(w);
    }

    private static RevitElement?[]? Extract(Howl h)
    {
        return h.Payload?.Where(x => x.Value is RevitElement)
            .Select(x => (RevitElement?)x.Value).ToArray();
    }
}