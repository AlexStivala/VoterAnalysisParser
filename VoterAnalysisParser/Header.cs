using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterAnalysisParser
{
    public class Header
    {
        public object id { get; set; }
        public string last_updated { get; set; }
        public string state { get; set; }
        public string race_type { get; set; }
        public candidates[] cand { get; set; }
        public string race_id { get; set; }
        
        
       public class candidates
        {
            public string party { get; set; }
            public string name { get; set; }
            public string id { get; set; }
        }
        
    }
}
