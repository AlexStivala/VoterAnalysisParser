using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterAnalysisParser
{
    public class MapSQLModel
    {
        public string VA_Data_Id { get; set; }
        public string r_type { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
        public string filter { get; set; }
        public string state { get; set; }
        public int percent { get; set; }
        public string formatted_update_time { get; set; }
    }
}
