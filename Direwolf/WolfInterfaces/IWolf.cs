using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.WolfInterfaces
{
    public interface IWolf
    {
        public IHowler Origin { get; }
        public IHowl Execute(IWolf yourself);
        public List<Catch> Catches { get; set; }
    }
}
