namespace Direwolf.Contracts
{
    public interface IWolfpackDB
    {
        public Task Send();
        public event EventHandler DatabaseConnectedEventHandler;
    }
}