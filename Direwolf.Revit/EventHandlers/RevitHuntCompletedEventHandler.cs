using System.Diagnostics;

namespace Direwolf.Revit.EventHandlers;

public class RevitHuntCompletedEventHandler : EventArgs
{
    public RevitHuntCompletedEventHandler(bool isSuccessful, Direwolf? dw)
    {
        if (dw is null) return;
        var processedResults = dw.ProcessedResults;
        IsSuccessful = isSuccessful;
        try
        {
            foreach (var wolf in dw.WolfQueue)
            foreach (var connector in wolf.Destinations)
            {
                if (processedResults is not null && !processedResults.TryGetValue(connector, out var connectors))
                {
                    connectors = [];
                    processedResults[connector] = connectors;
                }

                if (wolf.Result is not null) processedResults?[connector].Add(wolf.Result);
            }
        }
        catch (Exception e)
        {
            Debug.Print(e.Message);
            throw;
        }
    }

    public bool IsSuccessful { get; set; }
}