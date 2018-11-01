using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterAnalysisParser
{
    class VASQLDataModel
    {
        public string VA_Data_Id { get; set; }
        public string race_id { get; set; }
        public string r_type { get; set; }
        public string questionId { get; set; }
        public string qcode { get; set; }
        public string question { get; set; }
        public int q_order { get; set; }
        public string filter { get; set; }
        public string state { get; set; }
        public string ofc { get; set; }
        public string answer { get; set; }
        public string update_Time { get; set; }
        public DateTime f_update_time { get; set; }
        public string preface { get; set; }
        public string pk { get; set; }
        public bool def { get; set; }
        public int sample_size { get; set; }
        public float total_weight { get; set; }
        public int variable_count { get; set; }
        public float variable_weight { get; set; }
        public int variable_percent { get; set; }
        public int a_order { get; set; }
        public int answer_id { get; set; }

        public int result_order { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public float result_weight { get; set; }
        public string party { get; set; }
        public int result_percent { get; set; }
        public int result_count { get; set; }
        public int stateId { get; set; }

    }
}
