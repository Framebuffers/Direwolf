using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Dto;
using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;
using Direwolf.Dto.RevitApi;
using Microsoft.Extensions.Caching.Memory;
using Transaction = Direwolf.Dto.InternalDb.Transaction;

#pragma warning disable VISLIB0001
namespace Direwolf.RevitUI.Commands;

[Transaction(TransactionMode.Manual)]
public class DumpDatabase : IExternalCommand
{
    /*
     * IMPORTANT:
     *      CUID v1 are used instead of v2 because they're not security-critical.
     *      They're used to ensure uniqueness, encode a timestamp,
     *      and a session counter, all in a single string.
     *
     *      This makes it a very database-friendly way to create unique keys
     *      instead of using UUID's for everything. CUID's are only used inside
     *      Direwolf. Anything outside Direwolf's scope is parsed using Revit's UUID's.
     */

    private static MemoryCache _cache = new(new MemoryCacheOptions());
    private static List<RevitElement> _elements = [];


    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        return Result.Succeeded;
    }

    private static WolfDto CreateDto(Transaction t, string name, Realm realm, BuiltInCategory category, out Cuid id)
    {
        /*
         * CUID structure:
         * `clbvi4441000007ld63liebkf`
         *
         *  c           CUID v1 identifier.
         *  lbvi4441    UNIX Timestamp in milliseconds
         *  0000        Session counter
         *  07ld        Client fingerprint  (host process identifier + system hostname)
         *  63liebkf    Random data
         */

        var transactionId = Cuid.Create();
        id = transactionId;
        return new WolfDto(transactionId, name, realm, category) { Data = t };
    }
}