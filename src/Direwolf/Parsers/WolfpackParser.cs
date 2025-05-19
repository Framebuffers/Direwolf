using Direwolf.Dto.Parser;

using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Direwolf.Parsers;

/*
 * YAML parsing structure:
 * Wolfpacks are YAML files that define an operation to be performed by Direwolf (similar to a Docker Compose file).
 *
 * The Revit API holds a very small database of records for each Element object inside the Document, used to perform
 * fast queries. Slower queries go through the Revit API. For this reason, alongside the fact that
 * queries run faster on the unmanaged C++ side of the code rather than the managed .NET side, Autodesk recommends
 * running as many queries using the FilteredElementCollector API as possible.
 *
 * However, Direwolf works a bit differently. On startup, Direwolf creates a mirror dataset in RAM of the whole
 * Document, creating its own Record tree. It is shaped in such way, that it is optimised for raw data retrieval,
 * instead of transforming data. This is because Direwolf is meant to be used as a data retrieval tool, to only get data
 * in and out of the model. Transformations are meant to be done with external libraries meant to do these operations.
 *
 * Wolfpacks are queries done against *its own* dataset, that, whenever necessary, can use your average
 * FilteredElementCollector to perform a task; as well as a data-safe way to define data-ingress queries from a
 * data source.
 *
 * For storage, Direwolf creates a Binary Tree structure based on the hierarchical structure of the Revit data model:
 * Document -> Category -> Family -> Element -> Parameter. Elements can be both instances (using FamilyInstance)
 * or ElementTypes. This structure makes it easy to perform insert and remove operations whenever an edit is done.
 *
 * Upon saving, the in-memory database is updated and a new "DocumentEpisode" is created. Each Episode symbolises
 * the delta alterations between two save points, using the own RevitAPI to fetch that data. This data can be
 * dumped to a data destination, upon which any migrations are performed.
 *
 * This model allows Direwolf to use the API as little as possible, minding its own business in the background,
 * perform tasks asynchronously (bypassing the synchronous nature of Revit's DB), and make the data fetching and
 * exporting invisible to the user.
 *
 * The definition specify:
 *      - The type of Document (project/family) target.
 *      - If it's a read or write operation.
 *          - If it's a write operation, any parameters to be passed for the SQL migration/insert.
 *      - The platform on which this query should be executed [Desktop, Cloud, Workshared]
 *      - Conditions to perform the query [OnExecution, OnTrigger, WhenTrue, WhenFalse, WhenNull]
 *      - Logical operators for the query, if any.
 *      - Scope of the query [Document/Category/Element/Parameter/Type/Family/Instance/Annotative/Model/Schedules]
 *          - Each one is mapped to the definition of a FilteredElementCollector.
 *          - Queries can be united together (using .Union()) or manipulated using any LINQ operator.
 *      - Destination of the data [defined by a DriverCommonDto, taking the raw data as input]
 *
 * In the case of data input queries:
 *      - DataType of the parameters
 */

public class WolfpackParser
{
    public static Wolfpack Parse(string yamlStream)
    {
        using (var reader = new StreamReader(yamlStream))
        {
            var yaml = new YamlStream();
            yaml.Load(reader);

            var deserialize
                = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            return deserialize.Deserialize<Wolfpack>(yamlStream);
        }
    }
}