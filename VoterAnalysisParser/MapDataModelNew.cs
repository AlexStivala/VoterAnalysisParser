using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterAnalysisParser
{
    public class MapDataModelNew
    {

        public string pk { get; set; }
        public string palette { get; set; }
        public int[] breakpoints { get; set; }
        public int upper_limit { get; set; }
        public int lower_limit { get; set; }
        public StateData[] data { get; set; }
        
        public class StateData
        {
            public string answer { get; set; }
            public string qcode { get; set; }
            public int response_sum { get; set; }
            public int percent { get; set; }
            public string question { get; set; }
            public string state { get; set; }
            public string date { get; set; }
            public int breakpoint { get; set; }
            public string stage { get; set; }

        }

    }
}
