using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Events;
using Direwolf.Dto.InternalDb;
using Direwolf.Dto.RevitApi;
using Direwolf.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Direwolf.Sources.InternalDB;

public sealed class Database
{
    private static MemoryCache _cache = new(new MemoryCacheOptions());
    private readonly Application _application;
    private readonly List<RevitElement> _database = [];
    private readonly List<Transaction> _exceptions = [];

    public Database(Application application)
    {
        _application = application;
        application.DocumentOpened += ApplicationOnDocumentOpened;
    }

    public int GetDatabaseCount()
    {
        return _database.Count;
    }

    private void ApplicationOnDocumentOpened(object? sender, DocumentOpenedEventArgs e)
    {
        try
        {
            var doc = e.Document;
            if (doc is null) return;

            var result = doc.GetRevitDatabase();
            foreach (var element in result)
            {
                if (element is null) return;
                _database.Add(RevitElement.Create(doc, element));
                $"Added element: {element.Value}".ToConsole();
            }

            $"Final Count: {_database.Count}".ToConsole();
        }
        catch (Exception ex)
        {
            ex.LogException(_exceptions);
        }
    }
}