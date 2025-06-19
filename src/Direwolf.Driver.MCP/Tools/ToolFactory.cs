using Direwolf.Definitions.LLM;

namespace Direwolf.Driver.MCP.Tools;

public static class ToolFactory
{
    public static McpTool CreateWolfpack => new McpTool(
        "create_wolfpack",
        "Creates a new Wolfpack",
        new
        {
            type = "object",
            properties = new
            {
                name = new { type = "string", description = "Wolfpack name" },
                description = new { type = "string", description = "Wolfpack content's Description" },
                properties = new { type = "object", description = "Additional Properties" }
            },
            required = new[] { "name" }
        });

    public static McpTool GetWolfpack => new(
        "get_wolfpack",
        "Get a Wolfpack by CUID",
        null);
        
    public static McpTool RequestWolfpacks => new(
        "request_wolfpacks",
        "Requests a list of Wolfpacks",
        new
        {
            type = "object",
            properties = new
            {
                limit = new { type = "integer", description = "Maximum number of entities to return", @default = 100 },
                offset = new { type = "integer", description = "Number of entities to skip", @default = 0 }
            }
            
        });

    public static McpTool UpdateWolfpack => new(
        "update_wolfpack",
        "Updates an existing Wolfpacks",
        new
        {
            type = "object",
            properties = new
            {
                id = new { type = "string", description = "Wolfpack CUID" },
                name = new { type = "string", description = "Wolfpack name." },
                description = new { type = "string", description = "Wolfpack content's description." },
                properties = new { type = "object", description = "Additional properties." }
            },
            required = new[] { "id" }
        }
        
        );

    public static McpTool DeleteWolfpacks => new(
       "delete_wolfpack",
       "Deletes a Wolfpack",
       new
       {
           type = "object",
           properties = new
           {
               id = new { type = "string", description = "Wolfpack ID" }
           },
           required = new[] { "id" }
       }
    );

    public static McpTool AiAnalyzeWolfpack => new(
        "ai_analyze_wolfpack",
        "Analyze a Wolfpack and provide insights.",
        new
        {
            type = "object",
            properties = new
            {
                id = new { type = "string", description = "Wolfpack CUID" }
            },
            required = new[] { "id" }
        } 
    );
    
    public static McpTool AiGenerateWolfpack => new(
        "ai_generate_wolfpack",
        "Create a new Wolfpack using AI, based on a description.",
        new
        {
            type = "object",
            properties = new
            {
                prompt = new { type = "string", description = "Description of what Wolfpack to generate." }
            },
            required = new[] { "prompt" }
        } 
        );

}