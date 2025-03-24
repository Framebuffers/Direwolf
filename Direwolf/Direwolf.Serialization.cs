using Direwolf.Definitions;
using System.Text.Json;

namespace Direwolf
{
    public partial class Direwolf
    {
        private readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public void Serialize(WolfpackTarget where)
        {
            switch (where)
            {
                case WolfpackTarget.OnScreen:
                    SendAllToScreen();
                    return;
                case WolfpackTarget.Excel:
                    //stub
                    return;
                case WolfpackTarget.DB:
                    SendAllToDB();
                    return;
                case WolfpackTarget.JSONFile:
                    JsonSerializer.Serialize(Queries);
                    break;
                case WolfpackTarget.INVALID:
                default:
                    break;
            }
        }
    }
}
