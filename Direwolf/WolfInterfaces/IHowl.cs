using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.WolfInterfaces
{
    public interface IHowl
    {
        public IWolf Runner { get; set; }
        public Type Target { get; }
        public void LoadCatches();
    }
}
