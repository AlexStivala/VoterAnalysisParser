using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterAnalysisParser
{
    public class VAPostModel
    {
        public string request_type { get; set; }
        public string stack_type { get; set; }
        public string method { get; set; }
        public string id { get; set; }
        public string election_event { get; set; }
    }
}
