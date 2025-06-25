using Direwolf.Definitions.LLM;

namespace Direwolf.Driver.MCP.Tools;

//TODO: update tasks to reflect new tools.
public static class ResourceDefinitions
{
    public static McpTool CreateWolfpack => new McpTool(
        "create",
        "Creates a new Wolfpack",
        new
        {
            type = "object",
            properties = new
            {
                name = new { type = "string", description = "Wolfpack name" },
                description = new { type = "string", description = "Wolfpack content's Description" },
                payload = new { type = "object", description = "Properties to add to the DataCache." }
            },
            required = new[] { "name", "payload" }
        });

    public static McpTool ReadWolfpack => new(
        "get",
        "Get a Wolfpack by its ID",
        new
        {
            type = "object",
            properties = new
            {
                key = new {type = "object", description = "Key to look for." }
            }
        });
    
    public static McpTool GetWolfpackMany => new(
        "get-many",
        "Gets a series of Wolfpacks by their IDs",
        new
        {
            type = "object",
            properties = new
            {
                key = new {type = "object", description = "Keys to look for." }
            }
        }); 

    public static McpTool UpdateWolfpack => new(
        "update",
        "Updates an existing Wolfpack",
        new
        {
            type = "object",
            properties = new
            {
                key = new { type = "string", description = "Key for the element to be updated." },
                payload = new { type = "object", description = "New value for that element." }
            },
            required = new[] { "key", "payload" }
        });

    public static McpTool DeleteWolfpack => new(
       "delete",
       "Deletes a Wolfpack",
       new
       {
           type = "object",
           properties = new
           {
               key = new { type = "string", description = "Key of the element to delete." }
           },
           required = new[] { "key" }
       });

    public static McpTool AiAnalyzeWolfpack => new(
        "ai-analyze",
        "Analyze a Wolfpack and provide insights.",
        new
        {
            type = "object",
            properties = new
            {
                key = new { type = "string", description = "Key of the element to pass as argument to the AI agent." },
                options = new { type = "object", description = "Configuration options and parameters to pass onto the AI agent."}
            },
            required = new[] { "key" }
        });
    
    public static McpTool AiGenerateWolfpack => new(
        "ai-generate",
        "Create a new Wolfpack using AI, based on a description.",
        new
        {
            type = "object",
            properties = new
            {
                prompt = new { type = "string", description = "Prompt to pass to the AI agent." },
                options = new { type = "object", description = "Configuration options and parameters to pass onto the AI agent."}
            },
            required = new[] { "prompt" }
        });

}