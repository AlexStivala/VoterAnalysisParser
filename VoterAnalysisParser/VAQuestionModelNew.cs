using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterAnalysisParser
{
    public class VAQuestionModelNew
    {

        public string VA_Data_Id { get; set; }
        public string r_type { get; set; }
        public string qcode { get; set; }
        public string filter { get; set; }
        public string questionId { get; set; }
        public int question_order { get; set; }
        public string question { get; set; }
        public string race_type { get; set; }
        public string st { get; set; }
        public string State { get; set; }
        public string race_id { get; set; }
        public string ofc { get; set; }
        public float sample_size { get; set; }
        public float total_weight { get; set; }
        public string preface { get; set; }
        public string header { get; set; }
        public string last_updated { get; set; }
        public string election_event { get; set; }
        public Question[] h_answers { get; set; }

        public class Question
        {
            public string variable_weight { get; set; }
            public string variable_count { get; set; }
            public string variable_percent { get; set; }
            public string original_order { get; set; }
            public string original_name { get; set; }
            public string alias_name { get; set; }
            public string new_order { get; set; }
        }


    }

}
