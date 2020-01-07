using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterAnalysisParser
{
    public class VAAnswerModelNew
    {
        public string question_order { get; set; }
        public string questionId { get; set; }
        public string state { get; set; }
        public string race_id { get; set; }
        public string question { get; set; }
        public string race_type { get; set; }
        public string qcode { get; set; }
        public string filter { get; set; }
        public string sample_size { get; set; }
        public string total_weight { get; set; }
        public string def { get; set; }
        public string preface { get; set; }
        public string header { get; set; }
        public string formatted_update_time { get; set; }
        public string update_Time { get; set; }
        public Answer[] h_answers { get; set; }

        public class Answer
        {
            public string variable_weight { get; set; }
            public string variable_count { get; set; }
            public string variable_percent { get; set; }
            public string original_order { get; set; }
            public string original_name { get; set; }
            public string alias_name { get; set; }
            public Results[] results { get; set; }
        }

        public class Results
        {
            public string result_weight { get; set; }
            public string result_count { get; set; }
            public string result_percent { get; set; }
            public string original_name { get; set; }
            public string alias_name { get; set; }
            public string id { get; set; }
            public string party { get; set; }
        }
    }
}
