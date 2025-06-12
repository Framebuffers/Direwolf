using Direwolf.Definitions.Drivers.JSON;
using Direwolf.Definitions.Internal;
using Direwolf.Definitions.Internal.Enums;

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
        
         var howl = (Howl.Create(DataType.DirewolfException, 
                                        Method.Put, 
                                        new Dictionary<PayloadId, object>()
                                        {
                                            [payloadId with
                                            {
                                                DataType = DataType.String,
                                                Annotations = new Dictionary<string, object>()
                                                {
                                                    [nameof(ex.Message)] = ex.Message,
                                                    [nameof(ex.StackTrace)] = ex.StackTrace!
                                                }
                                            }] = ex!.Message
                                        },
                                        RevitElementJsonSchema.RevitElement,
                                        "Exception"));
        switch (ex?.GetType()
                    ?.Namespace?.Contains
                        ("Autodesk.Revit"))
        {
            case false:
                exceptionList.Add(howl with
                {
                    DataType = DataType.Invalid,
                    Result = Result.Rejected
                });
                break;
            default:
                if (ex is null) break;
                exceptionList.Add(howl with
                {
                    Result = Result.Rejected
                });

                break;
        }
    }
}