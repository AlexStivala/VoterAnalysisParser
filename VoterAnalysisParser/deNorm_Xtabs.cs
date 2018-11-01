using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VoterAnalysisParser
{
    public class deNorm_Xtabs
    {
        [MaxLength(1)]
        public string transSrc { get; set; }
        [MaxLength(8)]
        public string eDate { get; set; }
        public int st { get; set; }
        [MaxLength(2)]
        public string jType { get; set; }
        public int jCde { get; set; }
        [MaxLength(1)]
        public string eType { get; set; }
        [MaxLength(2)]
        public string ofc { get; set; }
        public int subset { get; set; }
        public int numCol { get; set; }
        public int numQ { get; set; }
        [MaxLength(15)]
        public string ts { get; set; }
        public int sweep { get; set; }
        public int seq { get; set; }
        public int resp { get; set; }
        [MaxLength(15)]
        public string postTime { get; set; }
        public int qSeq { get; set; }
        public int mxID { get; set; }
        public int numRows { get; set; }
        [MaxLength(50)]
        public string shortMxLabel { get; set; }
        [MaxLength(300)]
        public string fullMxLabel { get; set; }
        public int sampleSize { get; set; }
        public int xrow { get; set; }
        [MaxLength(100)]
        public string rowLabel { get; set; }
        [MaxLength(300)]
        public string rowText { get; set; }
        public int pChg { get; set; }
        public int cID { get; set; }
        [MaxLength(24)]
        public string candLastName { get; set; }
        public int cPct { get; set; }
        public int hPct { get; set; }
        public int vPct { get; set; }
        public DateTime updTime { get; set; }
        [MaxLength(1000)]
        public string FNCMxLabel { get; set; }
        [MaxLength(1000)]
        public string FNCRowText { get; set; }
        public bool baseQOnAir { get; set; }
        public int onAirTemplateID { get; set; }
        [MaxLength(1000)]
        public string FNCRowQuestion { get; set; }
        public bool rowQOnAir { get; set; }
        public int outputOrder { get; set; }
        public int rowOrder { get; set; }
        [MaxLength(1000)]
        public string OTSMxLabel { get; set; }
        [MaxLength(1000)]
        public string OTSRowText { get; set; }
        [MaxLength(1000)]
        public string OTSRowQuestion { get; set; }
        public int OTSRowOrder { get; set; }
        public bool OTSBaseQonAir { get; set; }
        public bool OTSRowQonAir { get; set; }
        public Int16 xpCall { get; set; }
        public Int16 xpStage { get; set; }

    }
}
