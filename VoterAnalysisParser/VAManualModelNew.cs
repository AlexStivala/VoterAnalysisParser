using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterAnalysisParser
{
    public class VAManualModelNew
    {
        public string VA_Data_Id { get; set; }
        public string r_type { get; set; }
        public string qcode { get; set; }
        public string question { get; set; }
        public string race { get; set; }
        public string st { get; set; }
        public string State { get; set; }
        public string edate { get; set; }
        public string active { get; set; }
        public string label { get; set; }
        public string timestamp { get; set; }
        public string status { get; set; }
        public string pk { get; set; }
        public string last_updated { get; set; }
        public string election_event { get; set; }
        public Answers[] answers { get; set; }

        public class Answers
        {
            public string value { get; set; }
            public string text { get; set; }
            
        }

    }
}
