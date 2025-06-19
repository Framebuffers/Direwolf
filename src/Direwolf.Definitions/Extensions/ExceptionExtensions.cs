using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;

namespace Direwolf.Definitions.Extensions;

/// <summary>
///     Helper methods for <see cref="Exception" /> inside the Direwolf context.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    ///     Creates a special <see cref="Howl" /> that holds Exception data thrown whenever a
    ///     <see cref="CrudOperation" /> has failed.
    /// </summary>
    /// <param name="ex">Exception being thrown.</param>
    /// <param name="exceptionList">The list where the Exception Transaction list is being held.</param>
    public static void LogException(this Exception ex, List<Howl> exceptionList)
    {
        var payloadId = PayloadId.Create(DataType.DirewolfException, "string");

        var howl = Howl.Create(DataType.DirewolfException,
            RequestType.Put,
            new Dictionary<string, object>
            {
                [ex.GetType().Name] = ex!.Message
            },
            "Exception");
        switch (ex?.GetType()
                    ?.Namespace?.Contains
                        ("Autodesk.Revit"))
        {
            case false:
                exceptionList.Add(howl with
                {
                    DataType = DataType.Invalid,
                    Result = ResultType.Rejected
                });
                break;
            default:
                if (ex is null) break;
                exceptionList.Add(howl with
                {
                    Result = ResultType.Rejected
                });

                break;
        }
    }
}