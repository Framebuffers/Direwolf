using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Direwolf;
public sealed class Hunter
{
    //TODO: receive JSON from MCP source, use MCP JSON schema and JSON-RCP standards
    //
    
    // public static Howl Parse(string yamlStream)
    // {
    //     using var reader = new StreamReader
    //         (yamlStream);
    //     var yaml = new YamlStream();
    //     yaml.Load
    //         (reader);
    //
    //     var deserialize = new DeserializerBuilder().WithNamingConvention
    //             (CamelCaseNamingConvention.Instance)
    //         .Build();
    //
    //     return deserialize.Deserialize<Howl>
    //         (yamlStream);
    // }
}