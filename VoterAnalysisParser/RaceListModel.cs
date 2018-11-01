using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterAnalysisParser
{
    public class RaceListModel
    {

        //{"Status": " ", "T-Stat": "0.5", "Counties In": " ", "Precinct Total": "1238", "% of Precincts": "87.16%", "Race Name": "GA-G-12029", "Precincts In": "1079", "TLC": " ", "Best Estimate": " ", "State": "Georgia", "Poll Intvs": " ", "% of Expected Vote": " ", "race_id": "GA-G-12029", "Counties Total": " ", "Diff": "4.08%", "Order": "GOP,Dem", "State ID": "GA"}
        
        public string Status { get; set; }
        public string T_Stat { get; set; }
        public string Counties_In { get; set; }
        public string Precinct_Total { get; set; }
        public string Pct_of_Precincts { get; set; }
        public string Race_Name { get; set; }
        public string Precincts_In { get; set; }
        public string TLC { get; set; }
        public string Best_Estimate { get; set; }
        public string Poll_Intvs { get; set; }
        public string Pct_Expected_Vote { get; set; }
        public string race_id { get; set; }
        public string Counties_Total { get; set; }
        public string Diff { get; set; }
        public string Order { get; set; }
        public string State_ID { get; set; }
        
    }
}
