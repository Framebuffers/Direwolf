namespace Direwolf.Definitions
{
    public readonly record struct DbConnectionString(string Host, int Port, string Username, string Password, string Database) { }
}
