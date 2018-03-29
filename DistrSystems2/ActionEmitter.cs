using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrSystems2
{
    public class ActionEmitter
    {
        public string Id { get; }
        public string Name { get; }
        public Dictionary<long, TimeSpanAction> TimeSpanActions { get; set; }



        public ActionEmitter(string id,string name)
        {
            Id = id;
            Name = name;
            TimeSpanActions = new Dictionary<long, TimeSpanAction>();
        }
    }
}
