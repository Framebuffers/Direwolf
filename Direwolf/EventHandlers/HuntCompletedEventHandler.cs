namespace Direwolf.EventHandlers;

public class HuntCompletedEventArgs : EventArgs
{
    public HuntCompletedEventArgs(bool isSuccessful, Direwolf dw)
    {
        IsSuccessful = isSuccessful;

        var processedResults = dw.ProcessedResults;
        foreach (var result in dw.WolfQueue)
        foreach (var connector in result.Destinations)
        {
            if (!processedResults.TryGetValue(connector, out var list))
            {
                list = [];
                processedResults[connector] = list;
            }

            if (result.Result is not null) processedResults[connector].Add(result.Result);
        }
    }

    public bool IsSuccessful { get; set; }
}