using Direwolf.Contracts;
using Direwolf.Definitions;
using System.Diagnostics;

namespace Direwolf
{
    public partial class Direwolf
    {
        public void Hunt(string testName)
        {
            try
            {
                foreach (var howler in Howlers)
                {
                    Hunt(howler, out _, testName);
                    var h = new HowlId()
                    {
                        HowlIdentifier = new Guid(),
                        Name = howler.GetType().Name
                    };
                    PreviousHowls.Add(h);
                    Debug.Print("Added to queue");
                }
            }
            catch
            {
                throw new Exception();
            }
        }

        public void Hunt(IHowler dispatch, out Wolfpack result, string testName)
        {
            try
            {
                result = dispatch.Howl(testName);
                var h = new HowlId()
                {
                    HowlIdentifier = new Guid(),
                    Name = dispatch.GetType().Name
                };
                PreviousHowls.Add(h);
                Queries.Push(result);
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
