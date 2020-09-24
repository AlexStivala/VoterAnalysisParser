using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterAnalysisParser
{
    public class MapSQLModelNew
    {
        public string VA_Data_Id { get; set; }
        public string r_type { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
        public string qcode { get; set; }
        public string state { get; set; }
        public int percent { get; set; }
        public int breakpoint { get; set; }
        public int response_sum { get; set; }
        public string date { get; set; }
        public string stage { get; set; }

    }
}
