using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrSystems2
{
    public class TimeSpanAction
    {
        public TimeSpanAction(long timeSpan, Action action)
        {
            TimeSpan = timeSpan;
            Action = action;
        }

        public long TimeSpan { get; set; }
        public Action Action{get;set;}

    }
}
