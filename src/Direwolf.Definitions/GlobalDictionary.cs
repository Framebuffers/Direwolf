namespace Direwolf.Definitions;

public class GlobalDictionary
{
    // protocol version 
    public const string DirewolfVersion = "1.0";
    
    // URIs
    public const string UriProtocol = "wolfpack://";
    public const string DirewolfPath = "wolfpack://direwolf";
    public const string WolfdenPath = "wolfpack://direwolf/wolfden";
    public const string HunterPath = "wolfpack://direwolf/hunter";
    public const string WolfpackCreate = "wolfpack://direwolf/wolfden/create";
    public const string WolfpackRead = "wolfpack://direwolf/wolfden/read";
    public const string WolfpackUpdate = "wolfpack://direwolf/wolfden/update";
    public const string WolfpackDelete = "wolfpack://direwolf/wolfden/delete";
    
    public const string DirewolfSelfTest = "wolfpack://direwolf/internal/testing";
    
    // hunter
    public const string HunterErrors = "wolfpack://direwolf/hunter/errors";
    public const string HunterTools = "wolfpack://direwolf/hunter/tools";
    public const string HunterResources = "wolfpack://direwolf/hunter/resources";
    public const string HunterCreate = "wolfpack://direwolf/hunter/create";
    public const string HunterRead = "wolfpack://direwolf/hunter/read";
    public const string HunterReadMany  = "wolfpack://direwolf/hunter/read-many";
    public const string HunterUpdate = "wolfpack://direwolf/hunter/update";
    public const string HunterDelete = "wolfpack://direwolf/hunter/delete";
    public const string HunterLlm = "wolfpack://direwolf/hunter/llm";
    public const string HunterLlmAnalyze = "wolfpack://direwolf/hunter/llm/analyze";
    public const string HunterLlmGenerate = "wolfpack://direwolf/hunter/llm/generate";
    
    // revit
    public const string RevitElement = "wolfpack://com.autodesk.revit-latest/document/elements";

}