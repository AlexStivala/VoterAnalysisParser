using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterAnalysisParser
{
    public class deNorm_Xtabs
    {
        public string transSrc { get; set; }
        public string eDate { get; set; }
        public int st { get; set; }
        public string jType { get; set; }
        public int jCde { get; set; }
        public string eType { get; set; }
        public string ofc { get; set; }
        public int subset { get; set; }
        public int numCol { get; set; }
        public int numQ { get; set} 
        public string ts { get; set; }
        public int sweep { get; set; }
        public float seq { get; set; }
        public float resp { get; set; }
        public string postTime { get; set; }
        public int qSeq { get; set; }
        public float mxID { get; set; }
        public int numRows { get; set; }
        public string shortMxLabel { get; set; }
        public string fullMxLabel { get; set; }
        public float sampleSize { get; set; }
        public int xrow { get; set; }
        public string rowLabel { get; set; }
        public string rowText { get; set; }
        public int pChg { get; set; }
        public int cID { get; set; }
        public string candLastName { get; set; }
        public int cPct { get; set; }
        public int hPct { get; set; }
        public int vPct { get; set; }
        public DateTime updTime { get; set; }
        public string FNCMxLabel { get; set; }
        public string FNCRowText { get; set; }
        public bool baseQOnAir { get; set; }
        public int onAirTemplateID { get; set; }
        public string FNCRowQuestion { get; set; }
        public bool rowQOnAir { get; set; }
        public int outputOrder { get; set; }
        public int rowOrder { get; set; }
        public string OTSMxLabel { get; set; }
        public string OTSRowText { get; set; }
        public string OTSRowQuestion { get; set; }
        public int OTSRowOrder { get; set; }
        public bool OTSBaseQonAir { get; set; }
        public bool OTSRowQonAir { get; set; }
        public int xpCall { get; set; }
        public int xpStage { get; set; }

    }
}
