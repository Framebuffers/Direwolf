using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.WolfInterfaces
{
    public interface IHowler
    {
        public List<IWolf> Wolves { get; set; }
        public Howl Dispatch();
        public Direwolf Origin { get; }
    }
}
