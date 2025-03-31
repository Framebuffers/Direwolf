using Direwolf.Definitions;

namespace Direwolf.Contracts
{
    public interface IDirewolf
    {
        event EventHandler? AsyncHuntCompletedEventHandler;
        event EventHandler? DatabaseConnectionEventHandler;

        static abstract void WriteDataToJson(object data, string filename, string path);
        string GetQueriesAsJson();
        string GetQueueInfo();
        void Hunt(IHowler dispatch, out Wolfpack result, string testName);
        void Hunt(string testName);
        void HuntAsync(string queryName = "query");
        void HuntAsync(IHowler howler, string queryName = "query");
        void QueueHowler(IHowler howler);
        void SendAllToDB();
        void WriteQueriesToJson();
    }
}