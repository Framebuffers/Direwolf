using Direwolf.Contracts;
using Direwolf.Definitions;

namespace Direwolf.Dynamo
{
    public class DirewolfDynamo
    {
        private readonly IDirewolf _direwolf;
        private readonly Stack<Wolfpack> _results = [];
        public DirewolfDynamo(IDirewolf direwolf, IEnumerable<Wolfpack> results)
        {
            _direwolf = direwolf;
            foreach (var r in results)
            {
                _results.Push(r);
            }
        }


    }
}
