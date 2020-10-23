using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;
using System.Net.Http;
using log4net.Appender;
using RestSharp;


namespace VoterAnalysisParser
{
    public delegate void ListErr(string s);
    public delegate void TextWrite(string s);
    public delegate void TextWrite2(string s);

    public partial class frmMain : Form, IAppender
    {

        public string dbConn = "Data Source=enygdb1;Initial Catalog=ElectionProd;Persist Security Info=True;User ID=gfxuser;Password=elect2018";

        //public string baseUrl = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack";
        //public string apiKey = "SpKGNVC1zl8AUolCQu4Qx4khpZiZNreD5hME1gMS";

        public string stacktype = "&stack_type=";
        public string calltype = "&call_type=";


        public string baseUrl = "https://rghm0pgome.execute-api.us-east-1.amazonaws.com/prod?page_type=stack";
        public string apiKey = "lb2jKp3m621H2YMcZnFVG3FwZvyjOvcQ7HCZ12AT";

        public string[] stackTypes = new string[5] { "ticker_question", "ticker_answer", "fullscreen_question", "fullscreen_answer", "map" };
        public string[] callTypes = new string[4] { "updates", "data&id=", "receipt&id=", "refresh" };

        public string newapiKey = "u5HPTwm77q2VxbXRUS6KG3UiKlR0FJEd48CTCoJk";
        public string newbaseUrl = "https://y384o59d2e.execute-api.us-west-2.amazonaws.com/prod";

        public string testapiKey = "HVE4MZQcwV1EBUPL587re6ezLkFlmydW5kEVrAtU";
        public string testbaseUrl = "https://khy8ymj127.execute-api.us-east-1.amazonaws.com/test";

        bool prodMode = Properties.Settings.Default.ProdMode;
        public string electionEvent = Properties.Settings.Default.electionEvent;
        public string useURL = Properties.Settings.Default.useURL;


        public class StateInfo
        {
            public string stateAbbv { get; set; }
            public string stateName { get; set; }
            public int stateID { get; set; }
        }

        //public static HttpClient Client = new HttpClient();
        public static HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://y384o59d2e.execute-api.us-west-2.amazonaws.com/prod");


        public List<StateInfo> stateData = new List<StateInfo>();
        public List<RaceListModel> raceList = new List<RaceListModel>();

        public List<string> QuestionUpdates = new List<string>();
        public List<string> AnswerUpdates = new List<string>();
        public List<string> MapUpdates = new List<string>();
        public List<string> ManualUpdates = new List<string>();

        public List<string> QuestionDeletes = new List<string>();
        public List<string> AnswerDeletes = new List<string>();
        public List<string> MapDeletes = new List<string>();
        public List<string> ManualDeletes = new List<string>();

        public List<string> msgs = new List<string>();
        public string runStop = "Start";


        public int dataType = 0;
        public int cnt = 0;

        //private static HttpClient client = new HttpClient();
        public string token = "eyJraWQiOiJnSkZIN25vdjdKWDZmcHFqb1lpVnh1RG92azIzVXJTN0c1eFA0UHQ0VEk4PSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiJmZGRlOTc0Zi03N2RlLTRjNWMtYWZjOC00ZjhmNjY1YTQyNjYiLCJjb2duaXRvOmdyb3VwcyI6WyJTdGFja1B1c2giLCJTdGFja0VkaXQiLCJRdWFyYW50aW5lQWNjZXNzIiwiVGllcjIiLCJEeW5hbWljUXVlcmllcyIsIlZpZXdUb3RhbHMiLCJIaWRlUUNvZGVzIl0sImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAudXMtZWFzdC0xLmFtYXpvbmF3cy5jb21cL3VzLWVhc3QtMV9wWkpjbWUxeEYiLCJwaG9uZV9udW1iZXJfdmVyaWZpZWQiOmZhbHNlLCJjb2duaXRvOnVzZXJuYW1lIjoiZmRkZTk3NGYtNzdkZS00YzVjLWFmYzgtNGY4ZjY2NWE0MjY2IiwiYXVkIjoidmNncjQ2ajBzZWZpZGI2a212dHQyY2ZyNSIsImV2ZW50X2lkIjoiYTRjNmYyZWUtMmZkYi00ZDdmLWE3ZDEtM2QwZTY4ODVhNGE4IiwidG9rZW5fdXNlIjoiaWQiLCJhdXRoX3RpbWUiOjE1Njc3ODQ2OTQsImV4cCI6MTU2Nzc4ODI5NCwiaWF0IjoxNTY3Nzg0Njk0LCJlbWFpbCI6Im1hdHQuZmFyZ3Vzb25AZm94bmV3cy5jb20ifQ.CjeG05KxL1N-QyJqlOqoSwYG5nRT593vvn7qN-JsPB8nuenvOZSIC0h_G9-Y4nJq20DBpj6dMfBJSba836V_lDNnYbIWZ4vRljURK-wuBUD3FnPaarPD5jlfKrUtxyODW3kpRq7QUUvj1wGKnU3mgnKmTdxuA2pqR_5QR382J5yhNDtD_a_MMlD_iQP-9mc3nDf3r4AeMSArkKTZZ6VjdOFrJe1i4K8DgOtN7lzX9a1JnABku4HPl5JDkMnEaJJifqf6_DIn5ywMhS8KiOSwLX-vW9ET8oWtEuPg6iBj3WU0XpDs0HXpbslCEVIRb3JKH5VbvJj1J5oTe9l73j3jMw";
        public string posturl = "https://d7icrmqr5f.execute-api.us-west-2.amazonaws.com/prod/";

        #region Logger instantiation - uses reflection to get module name
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Logging & status setup
        // This method used to implement IAppender interface from log4net; to support custom appends to status strip
        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            // Set text on status bar only if logging level is DEBUG or ERROR
            if ((loggingEvent.Level.Name == "ERROR") | (loggingEvent.Level.Name == "DEBUG"))
            {
                //toolStripStatusLabel.BackColor = System.Drawing.Color.Red;
                //toolStripStatusLabel.Text = String.Format("Error Logging Message: {0}: {1}", loggingEvent.Level.Name, loggingEvent.MessageObject.ToString());
            }
            else
            {
                //toolStripStatusLabel.BackColor = System.Drawing.Color.SpringGreen;
                //toolStripStatusLabel.Text = String.Format("Status Logging Message: {0}: {1}", loggingEvent.Level.Name, loggingEvent.MessageObject.ToString());
            }
        }

        // Handler to clear status bar message and reset color
        #endregion


        public frmMain()
        {
            InitializeComponent();

            // Set version number
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = String.Format("Voter Analysis Parser  Version {0}", version);

            log.Info($" ********** VoterAnalysisParser Started {version} **********");

 
            if (useURL == "Prod")
            {
                baseUrl = Properties.Settings.Default.URL_Prod;
                apiKey = Properties.Settings.Default.api_key_Prod;
                dbConn = Properties.Settings.Default.dbConn_Prod;
                btnAPI.Text = "Switch to {useURL} API";
                lblBaseUrl.Text = $"Using Prod API: {baseUrl}";
            }
            else if (useURL == "Dev")
            {
                baseUrl = Properties.Settings.Default.URL_Dev;
                apiKey = Properties.Settings.Default.api_key_Dev;
                dbConn = Properties.Settings.Default.dbConn_Prod;
                btnAPI.Text = "Switch to Prod API";
                lblBaseUrl.Text = $"URL: {useURL}:   {baseUrl}";
            }
            else if (useURL == "Stg")
            {
                baseUrl = Properties.Settings.Default.URL_Stg;
                apiKey = Properties.Settings.Default.api_key_Stg;
                dbConn = Properties.Settings.Default.dbConn_Prod;
                btnAPI.Text = "Switch to Prod API";
                lblBaseUrl.Text = $"URL: {useURL}:   {baseUrl}";
            }

            var builder = new SqlConnectionStringBuilder(dbConn);
            var dataSource = builder.DataSource;
            var initCat = builder.InitialCatalog;
            var user = builder.UserID;
            var pw = builder.Password;

            //lblBaseUrl.Text = baseUrl;
            //lblBaseUrl.Text = newbaseUrl;
            lblDB.Text = $"{dataSource}  {initCat}";

            //client.DefaultRequestHeaders.Add("x-api-key", newapiKey);

        }

        public class Wrapper
        {
            public Object header { get; set; }
            public VADataModel[] data { get; set; }
        }

        public class Wrappers
        {
            public Wrapper[] wrapper { get; set; }
        }

        private void btnParse_Click(object sender, EventArgs e)
        {

            try
            {

                //string race = "IL-G-91912";
                //string raceID = "91912";
                //string state = "IL";
                //string ofc = "G";
                //int stateID = 14;

                //string race = "MD-H-888888";
                //string raceID = "888888";
                //string state = "MD";
                //string ofc = "H";
                //int stateID = 21;

                string race = tbRace.Text.Trim();

                // parse the header info
                string[] strSeparator = new string[] { "-" };
                string[] raceStrings;

                // this takes the header and splits it into key-value pairs
                raceStrings = race.Split(strSeparator, StringSplitOptions.None);

                string raceID = raceStrings[2];
                string stateAbbv = raceStrings[0];
                string ofc = raceStrings[1];
                int stateID = GetStateID(stateAbbv);

                int parsedResult;

                string json = textBox1.Text;
                //var json = File.ReadAllText(@"C:\X\VoterAnalysisParser\VoterAnalysisParser\test1.json");
                var array = JsonConvert.DeserializeObject(json) as JArray;
                var headr = (array[0] as JObject).ToObject<Header>();
                var questions = (array[1] as JArray).ToObject<List<VADataModel>>();

                string lu = array[0].First.ToString();

                int pos1 = lu.IndexOf("\"last_updated\": \"");
                lu = lu.Substring(pos1 + 17);

                int pos2 = lu.IndexOf("\"");
                lu = lu.Substring(0, pos2 - 1);

                int mxID = stateID * 10000;
                DateTime lastUpdated = DateTime.ParseExact(lu, "yyyy-MM-dd HH:mm:ss.fffff",
                                        System.Globalization.CultureInfo.InvariantCulture);

                switch (ofc)
                {
                    case "G":
                        mxID += 1000;
                        break;
                    case "S":
                        mxID += 2000;
                        break;
                    case "H":
                        mxID += 3000;
                        break;
                    case "P":
                        mxID += 4000;
                        break;
                    default:
                        mxID += 5000;
                        break;
                }



                List<deNorm_Xtabs> dnXt = new List<deNorm_Xtabs>();

                int cnt = 0;


                for (int i = 0; i < questions.Count(); i++)
                {
                    label1.Text = i.ToString();

                    for (int j = 0; j < questions[i].h_answers.Count<VADataModel.Answer>(); j++)
                    {

                        for (int ri = 0; ri < questions[i].h_answers[j].results.Count() + 1; ri++)
                        {
                            deNorm_Xtabs dn = new deNorm_Xtabs();
                            //if (i != 44 && i != 45)
                            {


                                cnt++;
                                label2.Text = cnt.ToString();
                                //if (cnt == 2406)
                                //label2.Text = "check";
                                dn.transSrc = "E";
                                dn.eDate = "20181106";
                                dn.st = stateID;
                                dn.jType = "NR";
                                dn.jCde = 0;
                                dn.eType = "G";
                                dn.ofc = ofc;
                                dn.subset = 0;
                                dn.numCol = questions[i].h_answers[j].results.Count() + 1;
                                dn.numQ = i;
                                dn.ts = DateTime.Now.ToString("yyyyMMdd HHmmss");
                                dn.sweep = 1;
                                dn.seq = 99;
                                dn.resp = 1234;
                                dn.postTime = dn.ts;
                                dn.qSeq = i;
                                dn.mxID = mxID + i;
                                dn.numRows = questions[i].h_answers.Count();
                                dn.shortMxLabel = questions[i].qcode.Trim();
                                dn.fullMxLabel = questions[i].question.Trim();
                                dn.sampleSize = 5678;
                                if (int.TryParse(questions[i].h_answers[j].order, out parsedResult))
                                {
                                    dn.xrow = Convert.ToInt32(questions[i].h_answers[j].order);
                                    dn.rowOrder = Convert.ToInt32(questions[i].h_answers[j].order);
                                }
                                else
                                {
                                    dn.xrow = -1;
                                    dn.rowOrder = -1;
                                }

                                //dn.rowLabel = questions[i].h_answers[j].answer.Trim();
                                dn.rowLabel = questions[i].questionId.Trim();
                                dn.rowText = questions[i].h_answers[j].answer.Trim();
                                if (dn.rowLabel.Length > 69)
                                {
                                    label1.Text = $"rowLabel.Length = {dn.rowLabel.Length}";
                                }
                                dn.pChg = 0;


                                dn.OTSMxLabel = " ";
                                dn.OTSRowText = " ";
                                dn.OTSRowQuestion = " ";
                                dn.OTSRowOrder = 0;
                                dn.OTSBaseQonAir = false;
                                dn.OTSRowQonAir = false;
                                dn.xpCall = 0;
                                dn.xpStage = 0;
                                dn.onAirTemplateID = 0;

                                if (ri == 0)
                                {
                                    dn.cID = 0;
                                    dn.candLastName = "Total";
                                    dn.outputOrder = 0;
                                    dn.FNCRowQuestion = " ";
                                    dn.cPct = 0;
                                    dn.hPct = Convert.ToInt32(questions[i].h_answers[j].variable_percent);
                                    dn.vPct = Convert.ToInt32(questions[i].h_answers[j].variable_percent);
                                    dn.updTime = lastUpdated;
                                    dn.baseQOnAir = true;
                                    dn.rowQOnAir = false;

                                }
                                else
                                {

                                    if (int.TryParse(questions[i].h_answers[j].results[ri - 1].id, out parsedResult))
                                        dn.cID = parsedResult;
                                    else
                                        dn.cID = -1;

                                    dn.candLastName = questions[i].h_answers[j].results[ri - 1].name.Trim();


                                    if (int.TryParse((questions[i].h_answers[j].results[ri - 1].result_percent), out parsedResult))
                                    {
                                        dn.cPct = 0;
                                        dn.hPct = parsedResult;
                                        dn.vPct = parsedResult;
                                    }
                                    else
                                    {
                                        dn.cPct = 0;
                                        dn.hPct = -1;
                                        dn.vPct = -1;
                                    }

                                    dn.updTime = DateTime.Now;

                                    if (int.TryParse((questions[i].h_answers[j].results[ri - 1].order), out parsedResult))
                                        dn.outputOrder = parsedResult;
                                    else
                                        dn.outputOrder = -1;


                                    dn.FNCRowQuestion = questions[i].question;
                                    dn.baseQOnAir = false;
                                    dn.rowQOnAir = true;
                                }
                                dn.FNCMxLabel = questions[i].h_answers[j].answer.Trim();
                                dn.FNCRowText = questions[i].h_answers[j].answer.Trim();

                                dnXt.Add(dn);
                            }
                        }
                    }
                }
                dataGridView1.DataSource = dnXt;
                DataTable dt = new DataTable();
                dt = ListToDataTable<deNorm_Xtabs>(dnXt);
                UpdateDB(dt);

            }
            catch (Exception ex)
            {
                var info = ex.Message;
            }

        }
        public void ProcessData(string race, string json)
        {
            try
            {

                //string race = tbRace.Text.Trim();

                // parse the header info
                string[] strSeparator = new string[] { "-" };
                string[] raceStrings;

                // this takes the header and splits it into key-value pairs
                raceStrings = race.Split(strSeparator, StringSplitOptions.None);

                string raceID = raceStrings[2];
                string stateAbbv = raceStrings[0];
                string ofc = raceStrings[1];
                int stateID = GetStateID(stateAbbv);

                int parsedResult;

                //string json = textBox1.Text;
                //var json = File.ReadAllText(@"C:\X\VoterAnalysisParser\VoterAnalysisParser\test1.json");
                var array = JsonConvert.DeserializeObject(json) as JArray;
                var headr = (array[0] as JObject).ToObject<Header>();
                var questions = (array[1] as JArray).ToObject<List<VADataModel>>();

                string lu = array[0].First.ToString();

                int pos1 = lu.IndexOf("\"last_updated\": \"");
                lu = lu.Substring(pos1 + 17);

                int pos2 = lu.IndexOf("\"");
                lu = lu.Substring(0, pos2 - 1);

                int mxID = stateID * 10000;
                DateTime lastUpdated = DateTime.ParseExact(lu, "yyyy-MM-dd HH:mm:ss.fffff",
                                        System.Globalization.CultureInfo.InvariantCulture);

                switch (ofc)
                {
                    case "G":
                        mxID += 1000;
                        break;
                    case "S":
                        mxID += 2000;
                        break;
                    case "H":
                        mxID += 3000;
                        break;
                    case "P":
                        mxID += 4000;
                        break;
                    default:
                        mxID += 5000;
                        break;
                }



                List<deNorm_Xtabs> dnXt = new List<deNorm_Xtabs>();

                int cnt = 0;


                for (int i = 0; i < questions.Count(); i++)
                {
                    label1.Text = i.ToString();

                    for (int j = 0; j < questions[i].h_answers.Count<VADataModel.Answer>(); j++)
                    {

                        for (int ri = 0; ri < questions[i].h_answers[j].results.Count() + 1; ri++)
                        {
                            deNorm_Xtabs dn = new deNorm_Xtabs();
                            //if (i != 44 && i != 45)
                            {


                                cnt++;
                                label2.Text = cnt.ToString();
                                //if (cnt == 2406)
                                //label2.Text = "check";
                                dn.transSrc = "E";
                                dn.eDate = "20181106";
                                dn.st = stateID;
                                dn.jType = "NR";
                                dn.jCde = 0;
                                dn.eType = "G";
                                dn.ofc = ofc;
                                dn.subset = 0;
                                dn.numCol = questions[i].h_answers[j].results.Count() + 1;
                                dn.numQ = i;
                                dn.ts = DateTime.Now.ToString("yyyyMMdd HHmmss");
                                dn.sweep = 1;
                                dn.seq = 99;
                                dn.resp = 1234;
                                dn.postTime = dn.ts;
                                dn.qSeq = i;
                                dn.mxID = mxID + i;
                                dn.numRows = questions[i].h_answers.Count();
                                dn.shortMxLabel = questions[i].qcode.Trim();
                                dn.fullMxLabel = questions[i].question.Trim();
                                dn.sampleSize = 5678;
                                if (int.TryParse(questions[i].h_answers[j].order, out parsedResult))
                                {
                                    dn.xrow = Convert.ToInt32(questions[i].h_answers[j].order);
                                    dn.rowOrder = Convert.ToInt32(questions[i].h_answers[j].order);
                                }
                                else
                                {
                                    dn.xrow = -1;
                                    dn.rowOrder = -1;
                                }

                                //dn.rowLabel = questions[i].h_answers[j].answer.Trim();
                                dn.rowLabel = questions[i].questionId.Trim();
                                dn.rowText = questions[i].h_answers[j].answer.Trim();
                                if (dn.rowLabel.Length > 69)
                                {
                                    label1.Text = $"rowLabel.Length = {dn.rowLabel.Length}";
                                }
                                dn.pChg = 0;


                                dn.OTSMxLabel = " ";
                                dn.OTSRowText = " ";
                                dn.OTSRowQuestion = " ";
                                dn.OTSRowOrder = 0;
                                dn.OTSBaseQonAir = false;
                                dn.OTSRowQonAir = false;
                                dn.xpCall = 0;
                                dn.xpStage = 0;
                                dn.onAirTemplateID = 0;

                                if (ri == 0)
                                {
                                    dn.cID = 0;
                                    dn.candLastName = "Total";
                                    dn.outputOrder = 0;
                                    dn.FNCRowQuestion = " ";
                                    dn.cPct = 0;
                                    dn.hPct = Convert.ToInt32(questions[i].h_answers[j].variable_percent);
                                    dn.vPct = Convert.ToInt32(questions[i].h_answers[j].variable_percent);
                                    dn.updTime = lastUpdated;
                                    dn.baseQOnAir = true;
                                    dn.rowQOnAir = false;

                                }
                                else
                                {

                                    if (int.TryParse(questions[i].h_answers[j].results[ri - 1].id, out parsedResult))
                                        dn.cID = parsedResult;
                                    else
                                        dn.cID = -1;

                                    dn.candLastName = questions[i].h_answers[j].results[ri - 1].name.Trim();


                                    if (int.TryParse((questions[i].h_answers[j].results[ri - 1].result_percent), out parsedResult))
                                    {
                                        dn.cPct = 0;
                                        dn.hPct = parsedResult;
                                        dn.vPct = parsedResult;
                                    }
                                    else
                                    {
                                        dn.cPct = 0;
                                        dn.hPct = -1;
                                        dn.vPct = -1;
                                    }

                                    dn.updTime = DateTime.Now;

                                    if (int.TryParse((questions[i].h_answers[j].results[ri - 1].order), out parsedResult))
                                        dn.outputOrder = parsedResult;
                                    else
                                        dn.outputOrder = -1;


                                    dn.FNCRowQuestion = questions[i].question;
                                    dn.baseQOnAir = false;
                                    dn.rowQOnAir = true;
                                }
                                dn.FNCMxLabel = questions[i].h_answers[j].answer.Trim();
                                dn.FNCRowText = questions[i].h_answers[j].answer.Trim();

                                dnXt.Add(dn);
                            }
                        }
                    }
                }
                dataGridView1.DataSource = dnXt;
                DataTable dt = new DataTable();
                dt = ListToDataTable<deNorm_Xtabs>(dnXt);
                UpdateDB(dt);

            }
            catch (Exception ex)
            {
                var info = ex.Message;
            }

        }

        public static DataTable ListToDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();

            //var pInfo = typeof(T).GetProperty("transSrc").GetCustomAttribute<MaxLengthAttribute>();
            //var maxLen = pInfo.Length;

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                var col = new DataColumn(info.Name, info.PropertyType);
                try
                {
                    var pInfo = typeof(T).GetProperty(info.Name).GetCustomAttribute<MaxLengthAttribute>();
                    if (pInfo != null)
                    {
                        var maxLen = pInfo.Length;
                        col.MaxLength = maxLen;
                    }
                    dt.Columns.Add(col);
                }
                catch (Exception ex)
                {
                    var err = ex.Message;

                }

            }
            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        public void UpdateDB(DataTable dt)
        {
            string spName = "spUpdate_XTabs";
            string cmdStr = $"{spName} ";

            //Save out the top-level metadata
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConn))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update Xtabs data");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                            try
                            {
                                //Specify base command
                                cmd.CommandText = cmdStr;

                                cmd.Parameters.Add("@tblXtabs", SqlDbType.Structured).Value = dt;

                                sqlDataAdapter.SelectCommand = cmd;
                                sqlDataAdapter.SelectCommand.Connection = connection;
                                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                                // Execute stored proc 
                                sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                //Attempt to commit the transaction
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                textBox1.Text = "UpdateData - SQL Command Exception occurred: " + ex.Message;
                                log.Error("UpdateData- SQL Command Exception occurred: " + ex.Message);
                                //log.Debug("UpdateData- SQL Command Exception occurred", ex);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);
                textBox1.Text = "UpdateData - SQL Command Exception occurred: " + ex.Message;

            }

        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            string jsonData = GetVAData(tbRace.Text.Trim());
            textBox1.Text = jsonData;

        }

        private string GetVAData(string race)
        {

            Stopwatch stopWatch = new Stopwatch();
            TimeSpan ts;

            //string race = tbRace.Text.Trim();
            var jsonResponse = "";
            string WEBSERVICE_URL = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=xtab&race={race}&list_results=true2&list_answers=true";

            try
            {
                stopWatch.Start();
                var webRequest = System.Net.WebRequest.Create(WEBSERVICE_URL);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 20000;
                    webRequest.ContentType = "application/json";
                    webRequest.Headers.Add("x-api-key", "SpKGNVC1zl8AUolCQu4Qx4khpZiZNreD5hME1gMS");
                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            jsonResponse = sr.ReadToEnd();
                            //Console.WriteLine(String.Format("Response: {0}", jsonResponse));
                        }
                    }

                }
                ts = stopWatch.Elapsed;
                //label2.Text = $"Time to read dataset: {ts.Milliseconds} msec";
                stopWatch.Stop();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return jsonResponse;

        }
        private string GetRaceList()
        {

            Stopwatch stopWatch = new Stopwatch();
            TimeSpan ts;

            string race = tbRace.Text.Trim();

            var jsonResponse = "";
            //string WEBSERVICE_URL = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=xtab&race={race}&list_results=true2&list_answers=true";
            //string WEBSERVICE_URL = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=summary&race=all";
            string WEBSERVICE_URL = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=metadata&type=xtab&attribute=race_id";

            try
            {
                stopWatch.Start();
                var webRequest = System.Net.WebRequest.Create(WEBSERVICE_URL);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 20000;
                    webRequest.ContentType = "application/json";
                    webRequest.Headers.Add("x-api-key", "SpKGNVC1zl8AUolCQu4Qx4khpZiZNreD5hME1gMS");
                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            jsonResponse = sr.ReadToEnd();
                            Console.WriteLine(String.Format("Response: {0}", jsonResponse));
                        }
                    }

                }
                ts = stopWatch.Elapsed;
                //label2.Text = $"Time to read dataset: {ts.Milliseconds} msec";
                stopWatch.Stop();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return jsonResponse;

        }

        public static DataTable GetDBData(string cmdStr, string dbConnection)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    // Create the command and set its properties
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                        {
                            cmd.CommandText = cmdStr;
                            //cmd.Parameters.Add("@StackID", SqlDbType.Float).Value = stackID;
                            sqlDataAdapter.SelectCommand = cmd;
                            sqlDataAdapter.SelectCommand.Connection = connection;
                            sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;

                            // Fill the datatable from adapter
                            sqlDataAdapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                log.Error("GetDBData Exception occurred: " + ex.Message);
                //log.Debug("GetDBData Exception occurred", ex);
            }

            return dataTable;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            btnRunStop.Text = runStop;
            getStateInfo();
        }

        public void getStateInfo()
        {
            DataTable dt = new DataTable();
            string cmd = "SELECT State_ID, State_Abbv, State_Name FROM VDS_ElectionStateInfo";
            dt = GetDBData(cmd, dbConn);
            foreach (DataRow row in dt.Rows)
            {
                var sd = new StateInfo()
                {
                    stateAbbv = row["State_Abbv"].ToString() ?? "",
                    stateName = row["State_Name"].ToString() ?? "",
                    stateID = Convert.ToInt32(row["State_ID"] ?? "")

                };

                stateData.Add(sd);
            }

        }
        public int GetStateID(string stAbbv)
        {
            for (int i = 0; i < stateData.Count; i++)
            {
                if (stateData[i].stateAbbv == stAbbv)
                    return stateData[i].stateID;
            }
            return -1;
        }
        public string GetStateAbbv(string state)
        {
            for (int i = 0; i < stateData.Count; i++)
            {
                if (stateData[i].stateName == state)
                    return stateData[i].stateAbbv;
            }
            return "";
        }

        public string GetStateName(string st)
        {
            for (int i = 0; i < stateData.Count; i++)
            {
                if (stateData[i].stateAbbv == st)
                    return stateData[i].stateName;
            }
            return "";
        }

        public int getPrevNumBands(string pk)
        {
            DataTable dt = new DataTable();
            string cmd = $"SELECT * FROM FE_VoterAnalysis_Map_Defs_New Where VA_Data_ID = '{pk}'";
            dt = GetDBData(cmd, dbConn);

            int numBands = dt.Rows.Count;
            return numBands;

        }

        public void deleteMapMetaDataRecords(string pk, int index)
        {
            string delCmd = $"DELETE FROM FE_VoterAnalysis_Map_Defs_New WHERE VA_Data_Id = '{pk}' and Band >= {index}";
            IssueSQLCmd(delCmd);
        }

        public List<MapMetaDataModelNew> getMapMetaData(string pk)
        {
            DataTable dt = new DataTable();
            string cmd = $"SELECT * FROM FE_VoterAnalysis_Map_Defs_New Where VA_Data_ID = '{pk}'";
            dt = GetDBData(cmd, dbConn);

            int numBands = dt.Rows.Count;

            List<MapMetaDataModelNew> mmData = new List<MapMetaDataModelNew>();

            foreach (DataRow row in dt.Rows)
            {
                MapMetaDataModelNew mmd = new MapMetaDataModelNew();

                mmd.VA_Data_Id = row["VA_Data_Id"].ToString();
                mmd.color = row["Color"].ToString();
                mmd.colorIndex = Convert.ToInt32(row["ColorIndex"]);
                mmd.colorValue = Convert.ToInt32(row["ColorValue"]);
                mmd.numColorBands = Convert.ToInt32(row["NumBands"]);
                mmd.bandLo = Convert.ToInt32(row["BandLo"]);
                mmd.bandHi = Convert.ToInt32(row["BandHi"]);
                mmd.bandLabel = row["BandLbl"].ToString();
                mmd.Title = row["Title"].ToString();

                mmData.Add(mmd);
            }

            return mmData;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetRaces();
        }

        public void GetRaces()
        {
            try
            {
                textBox1.Clear();
                string jsonData = GetRaceList();
                textBox1.Text = jsonData;

                string racesWithData = jsonData.Replace("\"", "");
                jsonData = racesWithData;
                racesWithData = jsonData.Replace("[", "");
                jsonData = racesWithData;
                racesWithData = jsonData.Replace("]", "");
                jsonData = racesWithData;
                racesWithData = jsonData.Replace(" ", "");


                // parse the header info
                string[] strSeparator = new string[] { "," };
                string[] Races;

                // this takes the header and splits it into key-value pairs
                Races = racesWithData.Split(strSeparator, StringSplitOptions.None);
                for (int i = 0; i < Races.Length; i++)
                {
                    listBox1.Items.Add(Races[i]);
                }



                /*
                string json = textBox1.Text;
                var array = JsonConvert.DeserializeObject(json) as JArray;
                var races = array.ToObject<List<RaceListModel>>();

                foreach (RaceListModel race in races)
                {
                    listBox1.Items.Add(race.race_id);
                    raceList.Add(race);
                }
                */

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbRace.Text = listBox1.GetItemText(listBox1.SelectedItem);
        }

        public void GetAllRaces()
        {
            string VAData;
            GetRaces();
            foreach (RaceListModel race in raceList)
            {
                VAData = GetVAData(race.race_id);
                ProcessData(race.race_id, VAData);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetAllRaces();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            TimeSpan ts;
            stopWatch.Start();
            textBox1.Clear();
            listBox1.Items.Clear();

            //string url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_question&call_type=updates";

            //if (rbFS.Checked)
            //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_question&call_type=updates";

            // sample  https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_question&call_type=data&id=Voters:sex:CA-S-8619

            int n;
            if (rbFS.Checked)
                n = 2;
            else
                n = 0;

            string url = $"{baseUrl}{stacktype}{stackTypes[n]}{calltype}{callTypes[0]}";


            string jsonData = SendAPIRequest(url);

            textBox1.Text = jsonData;

            string racesWithData = jsonData.Replace("\"", "");
            jsonData = racesWithData;
            racesWithData = jsonData.Replace("[", "");
            jsonData = racesWithData;
            racesWithData = jsonData.Replace("]", "");
            jsonData = racesWithData;
            racesWithData = jsonData.Replace(" ", "");


            // parse the header info
            string[] strSeparator = new string[] { "," };
            string[] Races;

            // this takes the header and splits it into key-value pairs
            Races = racesWithData.Split(strSeparator, StringSplitOptions.None);
            for (int i = 0; i < Races.Length; i++)
            {
                listBox1.Items.Add(Races[i]);
            }


            ts = stopWatch.Elapsed;
            //label2.Text = $"Time to read dataset: {ts.Milliseconds} msec";
            stopWatch.Stop();

        }

        private string SendAPIRequest(string url)
        {

            string race = tbRace.Text.Trim();

            var jsonResponse = "";
            //string WEBSERVICE_URL = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=metadata&type=xtab&attribute=race_id";
            string WEBSERVICE_URL = url;

            try
            {
                var webRequest = System.Net.WebRequest.Create(WEBSERVICE_URL);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 20000;
                    webRequest.ContentType = "application/json";
                    //webRequest.Headers.Add("x-api-key", "SpKGNVC1zl8AUolCQu4Qx4khpZiZNreD5hME1gMS");
                    webRequest.Headers.Add("x-api-key", apiKey);
                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            jsonResponse = sr.ReadToEnd();
                            //Console.WriteLine(String.Format("Response: {0}", jsonResponse));
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return jsonResponse;

        }

        private string SendAPIPostRequest(string requestJson)
        {
            var jsonResponse = "";
            //string url = newbaseUrl;
            string url = baseUrl;

            try
            {
                HttpClient client = new HttpClient();

                var content = new StringContent(requestJson.ToString(), Encoding.UTF8, "application/json");

                //requestMessage.Headers.Add("x-api-key", newapiKey);
                requestMessage.Headers.Add("x-api-key", apiKey);
                requestMessage.Content = new StringContent(requestJson.ToString(), Encoding.UTF8, "application/json");

                //client.DefaultRequestHeaders.Add("x-api-key", newapiKey);
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);


                //HttpResponseMessage result = client.PostAsync(url, requestMessage.Content).Result;
                HttpResponseMessage result = client.PostAsync(url, content).Result;

                jsonResponse = result.Content.ReadAsStringAsync().Result;


                //var client = new RestClient(newbaseUrl);
                //client.Timeout = -1;
                //var request = new RestRequest(Method.POST);
                //request.AddHeader("x-api-key", newapiKey);
                //request.AddHeader("Content-Type", "application/json");
                //request.AddParameter("application/json", requestJson, ParameterType.RequestBody);
                //IRestResponse response = client.Execute(request);
                //jsonResponse = response.Content;
                //    //Console.WriteLine(response.Content);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            return jsonResponse;

        }


        /*
        static readonly HttpClient client = new HttpClient();
        static async Task<int> GetPageLengthAsync(string url)
        {
            Task<string> fetchTextTask = client.GetStringAsync(url);
            int length = (await fetchTextTask).Length;
            return length;
        }
        static void PrintPageLength()
        {
            Task<int> lengthTask =
            GetPageLengthAsync("http://csharpindepth.com");
            Console.WriteLine(lengthTask.Result);
        }
        */



        /*
        static async Task<string> GetAPIDataAsync(string url, string race)
        {
            Task<string> GetAPIData = client.GetStringAsync(url);
            string jsondata = await GetAPIData;
            
            return jsondata;
            
        }
        
        public string SendAPIRequest1(string url, string race)
        {
            string str = "";
            try
            {
                Task<string> jsonResponse = GetAPIDataAsync(url, race);
                str = jsonResponse.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return str;
            

        }
        */

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {

                string json = textBox1.Text;

                VAQuestionModel questions = new VAQuestionModel();
                questions = JsonConvert.DeserializeObject<VAQuestionModel>(json);


                List<VASQLDataModel> sqm = new List<VASQLDataModel>();

                int cnt = 0;
                int n = questions.h_answers.Count() + 1;


                //for (int i = 0; i < questions.Count(); i++)
                {
                    //label1.Text = i.ToString();

                    for (int j = 0; j < questions.h_answers.Count(); j++)
                    {

                        //for (int ri = 0; ri < questions[i].h_answers[j].results.Count() + 1; ri++)
                        {
                            VASQLDataModel sq = new VASQLDataModel();
                            string fTimeStr = questions.formatted_update_time;
                            DateTime fTime = Convert.ToDateTime(fTimeStr);

                            {
                                cnt++;
                                label2.Text = cnt.ToString();

                                //sq.VA_Data_Id = race;
                                sq.race_id = questions.race_id;
                                sq.r_type = "Q";
                                sq.questionId = questions.questionId;
                                sq.qcode = questions.qcode;
                                sq.question = questions.question;
                                sq.q_order = Convert.ToInt32(questions.question_order);
                                sq.filter = questions.filter;
                                sq.state = questions.state;

                                if (questions.race_type == "all")
                                    sq.ofc = "A";
                                else
                                    sq.ofc = questions.race_type;

                                sq.update_Time = questions.update_Time;
                                sq.f_update_time = fTime;
                                sq.preface = questions.preface;
                                sq.pk = questions.pk;
                                sq.def = Convert.ToBoolean(questions.def);
                                sq.sample_size = Convert.ToInt32(questions.sample_size);
                                sq.total_weight = Convert.ToSingle(questions.total_weight);
                                sq.variable_count = Convert.ToInt32(questions.h_answers[j].variable_count);
                                sq.variable_weight = Convert.ToSingle(questions.h_answers[j].variable_weight);
                                sq.variable_percent = Convert.ToInt32(questions.h_answers[j].variable_percent);
                                sq.answer = questions.h_answers[j].answer;
                                sq.a_order = Convert.ToInt32(questions.h_answers[j].order);
                                sq.answer_id = Convert.ToInt32(questions.h_answers[j].answer_id);
                                sq.name = string.Empty;
                                sq.id = 0;
                                sq.result_count = 0;
                                sq.result_percent = 0;
                                sq.result_weight = 0;
                                sq.party = string.Empty;

                                sq.stateId = GetStateID(questions.state);

                                sqm.Add(sq);
                            }
                        }
                    }
                }
                dataGridView1.DataSource = sqm;
                DataTable dt = new DataTable();
                dt = ListToDataTable<VASQLDataModel>(sqm);
                UpdateVADDB(dt);

            }
            catch (Exception ex)
            {
                var info = ex.Message;
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            string raceID = tbRace.Text.Trim();
            int pos = raceID.IndexOf("|");
            string deleteStr = raceID.Substring(pos + 1);
            string race = raceID.Substring(0, pos);

            dataType = 0;
            //string url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_question&call_type=data&id={race}";
            //if (rbFS.Checked)
            //{
            //dataType = 1;
            //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_question&call_type=data&id={race}";
            //}
            //string jsonData = SendAPIRequest1(url, race);

            if (rbFS.Checked)
            {
                dataType = 1;
                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_question&call_type=data&id={race}";
            }

            int n = dataType * 2;
            string url = $"{baseUrl}{stacktype}{stackTypes[n]}{calltype}{callTypes[1]}{race}";


            string jsonData = SendAPIRequest(url);
            textBox1.Text = jsonData;

        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            listBox1.Items.Clear();

            //string url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_answer&call_type=updates";

            //if (rbFS.Checked)
            //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_answer&call_type=updates";

            int n;
            if (rbFS.Checked)
                n = 3;
            else
                n = 1;

            string url = $"{baseUrl}{stacktype}{stackTypes[n]}{calltype}{callTypes[0]}";





            string jsonData = SendAPIRequest(url);

            textBox1.Text = jsonData;

            string racesWithData = jsonData.Replace("\"", "");
            jsonData = racesWithData;
            racesWithData = jsonData.Replace("[", "");
            jsonData = racesWithData;
            racesWithData = jsonData.Replace("]", "");
            jsonData = racesWithData;
            racesWithData = jsonData.Replace(" ", "");


            // parse the header info
            string[] strSeparator = new string[] { "," };
            string[] Races;

            // this takes the header and splits it into key-value pairs
            Races = racesWithData.Split(strSeparator, StringSplitOptions.None);
            for (int i = 0; i < Races.Length; i++)
            {
                listBox1.Items.Add(Races[i]);
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            string raceID = tbRace.Text.Trim();
            int pos = raceID.IndexOf("|");
            string deleteStr = raceID.Substring(pos + 1);
            string race = raceID.Substring(0, pos);

            //dataType = 0;
            //string url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_answer&call_type=data&id={race}";
            //if (rbFS.Checked)
            //{
            //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_answer&call_type=data&id={race}";
            //dataType = 1;
            //}


            dataType = 0;

            if (rbFS.Checked)
            {
                dataType = 1;

            }

            int n = dataType * 2 + 1;
            string url = $"{baseUrl}{stacktype}{stackTypes[n]}{calltype}{callTypes[1]}{race}";


            string jsonData = SendAPIRequest(url);
            textBox1.Text = jsonData;

        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {

                string json = textBox1.Text;

                VAAnswerModel answers = new VAAnswerModel();
                answers = JsonConvert.DeserializeObject<VAAnswerModel>(json);


                List<VASQLDataModel> sqm = new List<VASQLDataModel>();

                int cnt = 0;
                int n = answers.h_answers.Count() + 1;
                int parsedResult;


                //for (int i = 0; i < questions.Count(); i++)
                {
                    //label1.Text = i.ToString();

                    for (int j = 0; j < answers.h_answers.Count(); j++)
                    {

                        for (int ri = 0; ri < answers.h_answers[j].results.Count() + 1; ri++)
                        {
                            VASQLDataModel sq = new VASQLDataModel();
                            string fTimeStr = answers.formatted_update_time;
                            DateTime fTime = Convert.ToDateTime(fTimeStr);

                            {
                                cnt++;
                                label2.Text = cnt.ToString();

                                //sq.VA_Data_Id = race;
                                sq.race_id = answers.race_id;
                                sq.r_type = "A";
                                sq.questionId = answers.questionId;
                                sq.qcode = answers.qcode;
                                sq.question = answers.question;
                                sq.q_order = Convert.ToInt32(answers.question_order);
                                sq.filter = answers.filter;
                                sq.state = answers.state;

                                if (answers.race_type == "all")
                                    sq.ofc = "A";
                                else
                                    sq.ofc = answers.race_type;


                                sq.update_Time = answers.update_Time;
                                sq.f_update_time = DateTime.Now;
                                //sq.f_update_time = fTime;
                                sq.preface = answers.preface;
                                sq.pk = answers.pk;
                                sq.def = Convert.ToBoolean(answers.def);
                                sq.sample_size = Convert.ToInt32(answers.sample_size);
                                sq.total_weight = Convert.ToSingle(answers.total_weight);
                                sq.variable_count = Convert.ToInt32(answers.h_answers[j].variable_count);
                                sq.variable_weight = Convert.ToSingle(answers.h_answers[j].variable_weight);
                                sq.variable_percent = Convert.ToInt32(answers.h_answers[j].variable_percent);
                                sq.answer = answers.h_answers[j].answer;
                                sq.a_order = Convert.ToInt32(answers.h_answers[j].order);
                                sq.answer_id = Convert.ToInt32(answers.h_answers[j].answer_id);

                                if (ri == 0)
                                {
                                    sq.name = string.Empty;
                                    sq.id = 0;
                                    sq.result_count = 0;
                                    sq.result_percent = 0;
                                    sq.result_weight = 0;
                                    sq.party = string.Empty;
                                    sq.result_order = 0;

                                }
                                else
                                {
                                    sq.name = answers.h_answers[j].results[ri - 1].name;
                                    sq.id = Convert.ToInt32(answers.h_answers[j].results[ri - 1].id);
                                    sq.result_count = Convert.ToInt32(answers.h_answers[j].results[ri - 1].result_count);
                                    sq.result_percent = Convert.ToInt32(answers.h_answers[j].results[ri - 1].result_percent);
                                    sq.result_weight = Convert.ToSingle(answers.h_answers[j].results[ri - 1].result_weight);
                                    sq.party = answers.h_answers[j].results[ri - 1].party;

                                    if (int.TryParse(answers.h_answers[j].results[ri - 1].order, out parsedResult))
                                        sq.result_order = Convert.ToInt32(answers.h_answers[j].results[ri - 1].order);
                                    else
                                        sq.result_order = cnt;

                                }

                                sq.stateId = GetStateID(answers.state);

                                sqm.Add(sq);
                            }
                        }
                    }
                }
                dataGridView1.DataSource = sqm;
                DataTable dt = new DataTable();
                dt = ListToDataTable<VASQLDataModel>(sqm);
                UpdateVADDB(dt);

            }
            catch (Exception ex)
            {
                var info = ex.Message;
            }

        }

        public void UpdateVADDB(DataTable dt)
        {
            string spName = "spUpdate_VoterAnalysisData_Ticker";

            if (dataType == 1)
                spName = "spUpdate_VoterAnalysisData_FullScreen";


            string cmdStr = $"{spName} ";

            //Save out the top-level metadata
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConn))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update Voter Analysis Data");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                            try
                            {
                                //Specify base command
                                cmd.CommandText = cmdStr;

                                cmd.Parameters.Add("@tblVAD", SqlDbType.Structured).Value = dt;

                                sqlDataAdapter.SelectCommand = cmd;
                                sqlDataAdapter.SelectCommand.Connection = connection;
                                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                                // Execute stored proc 
                                sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                //Attempt to commit the transaction
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                textBox1.Text = "UpdateData - SQL Command Exception occurred: " + ex.Message;
                                //log.Error("UpdateData- SQL Command Exception occurred: " + ex.Message);
                                //log.Debug("UpdateData- SQL Command Exception occurred", ex);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);
                textBox1.Text = "UpdateData - SQL Command Exception occurred: " + ex.Message;

            }

        }

        public void UpdateVAMapData(DataTable dt)
        {
            string spName = "spUpdate_VoterAnalysisData_Map";



            string cmdStr = $"{spName} ";

            //Save out the top-level metadata
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConn))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update Voter Analysis Map Data");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                            try
                            {
                                //Specify base command
                                cmd.CommandText = cmdStr;

                                cmd.Parameters.Add("@tblVAMD", SqlDbType.Structured).Value = dt;

                                sqlDataAdapter.SelectCommand = cmd;
                                sqlDataAdapter.SelectCommand.Connection = connection;
                                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                                // Execute stored proc 
                                sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                //Attempt to commit the transaction
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                //textBox1.Text = "Update Map Data - SQL Command Exception occurred: " + ex.Message;
                                log.Error("UpdateData- SQL Command Exception occurred: " + ex.Message);
                                //log.Debug("UpdateData- SQL Command Exception occurred", ex);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);
                //textBox1.Text = "Update Map Data - SQL Command Exception occurred: " + ex.Message;

            }

        }

        public void UpdateVAMapMetaDataNew(MapMetaDataModelNew mmd)
        {
            string spName = "spUpsert_VoterAnalysis_Map_Defs_New ";
            string cmdStr = $"{spName} ";

            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConn))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update Voter Analysis Map Data");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                            try
                            {
                                //Specify base command
                                cmd.CommandText = cmdStr;

                                cmd.Parameters.Add("@VA_Data_ID", SqlDbType.NVarChar).Value = mmd.VA_Data_Id;
                                cmd.Parameters.Add("@Color", SqlDbType.NVarChar).Value = mmd.color;
                                cmd.Parameters.Add("@ColorIndex", SqlDbType.Int).Value = mmd.colorIndex;
                                cmd.Parameters.Add("@ColorValue", SqlDbType.Int).Value = mmd.colorValue;
                                cmd.Parameters.Add("@NumBands", SqlDbType.Int).Value = mmd.numColorBands;
                                cmd.Parameters.Add("@Band", SqlDbType.Int).Value = mmd.band;
                                cmd.Parameters.Add("@BandLo", SqlDbType.Int).Value = mmd.bandLo;
                                cmd.Parameters.Add("@BandHi", SqlDbType.Int).Value = mmd.bandHi;
                                cmd.Parameters.Add("@BandLbl", SqlDbType.NVarChar).Value = mmd.bandLabel;
                                cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = mmd.Title;

                                sqlDataAdapter.SelectCommand = cmd;
                                sqlDataAdapter.SelectCommand.Connection = connection;
                                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                                // Execute stored proc 
                                sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                //Attempt to commit the transaction
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                //textBox1.Text = "Update Map Data - SQL Command Exception occurred: " + ex.Message;
                                log.Error("UpdateData- SQL Command Exception occurred: " + ex.Message);
                                //log.Debug("UpdateData- SQL Command Exception occurred", ex);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);
                //textBox1.Text = "Update Map Data - SQL Command Exception occurred: " + ex.Message;

            }

        }


        public void UpdateVAMapDataNew(DataTable dt)
        {
            string spName = "spUpdate_VoterAnalysisData_Map_New";
            string cmdStr = $"{spName} ";

            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConn))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update Voter Analysis Map Data");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                            try
                            {
                                //Specify base command
                                cmd.CommandText = cmdStr;

                                cmd.Parameters.Add("@tblVAMD", SqlDbType.Structured).Value = dt;

                                sqlDataAdapter.SelectCommand = cmd;
                                sqlDataAdapter.SelectCommand.Connection = connection;
                                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                                // Execute stored proc 
                                sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                //Attempt to commit the transaction
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                //textBox1.Text = "Update Map Data - SQL Command Exception occurred: " + ex.Message;
                                log.Error("UpdateData- SQL Command Exception occurred: " + ex.Message);
                                //log.Debug("UpdateData- SQL Command Exception occurred", ex);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);
                //textBox1.Text = "Update Map Data - SQL Command Exception occurred: " + ex.Message;

            }

        }

        public void GetQuestionUpdates()
        {
            textBox1.Clear();
            listBox1.Items.Clear();
            //string url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_question&call_type=updates";

            //if (rbFS.Checked)
            //if (dataType == 1)
            //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_question&call_type=updates";

            // sample  https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_question&call_type=data&id=Voters:sex:CA-S-8619

            int n = dataType * 2;
            string url = $"{baseUrl}{stacktype}{stackTypes[n]}{calltype}{callTypes[0]}";

            string jsonData = SendAPIRequest(url);

            textBox1.Text = jsonData;

            if (jsonData.Length > 1 && jsonData != "[]")
            {
                string racesWithData = jsonData.Replace("\"", "");
                jsonData = racesWithData;
                racesWithData = jsonData.Replace("[", "");
                jsonData = racesWithData;
                racesWithData = jsonData.Replace("]", "");
                jsonData = racesWithData;
                racesWithData = jsonData.Replace(" ", "");


                // parse the header info
                string[] strSeparator = new string[] { "," };
                string[] Races;

                // this takes the header and splits it into key-value pairs
                Races = racesWithData.Split(strSeparator, StringSplitOptions.None);
                int pos;
                string deleteStr;


                for (int i = 0; i < Races.Length; i++)
                {
                    listBox1.Items.Add(Races[i]);
                    pos = Races[i].IndexOf("|");
                    deleteStr = Races[i].Substring(pos + 1);
                    Races[i] = Races[i].Substring(0, pos);
                    if (deleteStr == "delete")
                        QuestionDeletes.Add(Races[i]);
                    else
                        QuestionUpdates.Add(Races[i]);
                }
            }
        }

        public void GetAnswerUpdates()
        {
            try
            {

                textBox1.Clear();
                listBox1.Items.Clear();

                //string url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_answer&call_type=updates";

                //if (rbFS.Checked)
                //if (dataType == 1)
                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_answer&call_type=updates";

                int n = dataType * 2 + 1;
                string url = $"{baseUrl}{stacktype}{stackTypes[n]}{calltype}{callTypes[0]}";

                string jsonData = SendAPIRequest(url);

                textBox1.Text = jsonData;

                //if (jsonData != "[]")
                if (jsonData.Length > 1 && jsonData != "[]")

                {
                    string racesWithData = jsonData.Replace("\"", "");
                    jsonData = racesWithData;
                    racesWithData = jsonData.Replace("[", "");
                    jsonData = racesWithData;
                    racesWithData = jsonData.Replace("]", "");
                    jsonData = racesWithData;
                    racesWithData = jsonData.Replace(" ", "");


                    // parse the header info
                    string[] strSeparator = new string[] { "," };
                    string[] Races;

                    // this takes the header and splits it into key-value pairs
                    Races = racesWithData.Split(strSeparator, StringSplitOptions.None);
                    int pos;
                    string deleteStr;

                    for (int i = 0; i < Races.Length; i++)
                    {
                        listBox1.Items.Add(Races[i]);
                        pos = Races[i].IndexOf("|");
                        deleteStr = Races[i].Substring(pos + 1);
                        Races[i] = Races[i].Substring(0, pos);
                        if (deleteStr == "delete")
                            AnswerDeletes.Add(Races[i]);
                        else
                            AnswerUpdates.Add(Races[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error getting Answer Updates: {ex}");

            }


        }
        public void GetQuestionData(string race)
        {
            try
            {
                textBox1.Clear();
                //string url;

                //if (dataType == 1)
                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_question&call_type=data&id={race}";
                //else
                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_question&call_type=data&id={race}";

                int n = dataType * 2;
                string url = $"{baseUrl}{stacktype}{stackTypes[n]}{calltype}{callTypes[1]}{race}";

                string jsonData = SendAPIRequest(url);
                textBox1.Text = jsonData;
                if (jsonData.Length > 10)
                    ProcessQuestionData(jsonData, race);
                else
                {
                    //if (dataType == 1)
                    //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_question&call_type=receipt&id={race}";
                    //else
                    //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_question&call_type=receipt&id={race}";


                    url = $"{baseUrl}{stacktype}{stackTypes[n]}{calltype}{callTypes[2]}{race}";


                    jsonData = SendAPIRequest(url);
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error getting Question Data: {ex}");

            }

        }

        public void ProcessQuestionData(string json, string race)
        {
            try
            {

                //string json = textBox1.Text;

                VAQuestionModel questions = new VAQuestionModel();

                string err = "stackTrace";
                int pos = json.IndexOf(err);
                if (pos == -1)
                {

                    questions = JsonConvert.DeserializeObject<VAQuestionModel>(json);


                    List<VASQLDataModel> sqm = new List<VASQLDataModel>();

                    int cnt = 0;
                    int n = questions.h_answers.Count() + 1;


                    //for (int i = 0; i < questions.Count(); i++)
                    {
                        //label1.Text = i.ToString();

                        for (int j = 0; j < questions.h_answers.Count(); j++)
                        {

                            //for (int ri = 0; ri < questions[i].h_answers[j].results.Count() + 1; ri++)
                            {
                                VASQLDataModel sq = new VASQLDataModel();
                                string fTimeStr = questions.formatted_update_time;
                                DateTime fTime = Convert.ToDateTime(fTimeStr);

                                {
                                    cnt++;
                                    label2.Text = cnt.ToString();


                                    sq.VA_Data_Id = race;
                                    sq.race_id = questions.race_id;
                                    sq.r_type = "Q";
                                    sq.questionId = questions.questionId;
                                    sq.q_order = Convert.ToInt32(questions.question_order);
                                    sq.qcode = questions.qcode;
                                    sq.question = questions.question;
                                    sq.filter = questions.filter;
                                    sq.state = questions.state;

                                    if (questions.race_type == "all")
                                        sq.ofc = "A";
                                    else
                                        sq.ofc = questions.race_type;

                                    sq.update_Time = questions.update_Time;
                                    sq.f_update_time = fTime;
                                    sq.preface = questions.preface;
                                    sq.pk = questions.pk;
                                    sq.def = Convert.ToBoolean(questions.def);
                                    sq.sample_size = Convert.ToInt32(questions.sample_size);
                                    sq.total_weight = Convert.ToSingle(questions.total_weight);
                                    sq.variable_count = Convert.ToInt32(questions.h_answers[j].variable_count);
                                    sq.variable_weight = Convert.ToSingle(questions.h_answers[j].variable_weight);
                                    sq.variable_percent = Convert.ToInt32(questions.h_answers[j].variable_percent);
                                    sq.answer = questions.h_answers[j].answer;
                                    sq.a_order = Convert.ToInt32(questions.h_answers[j].order);
                                    sq.answer_id = Convert.ToInt32(questions.h_answers[j].answer_id);
                                    sq.name = string.Empty;
                                    sq.id = 0;
                                    sq.result_count = 0;
                                    sq.result_percent = 0;
                                    sq.result_weight = 0;
                                    sq.party = string.Empty;
                                    sq.stateId = GetStateID(questions.state);


                                    sqm.Add(sq);
                                }
                            }
                        }
                    }


                    dataGridView1.DataSource = sqm;
                    DataTable dt = new DataTable();
                    dt = ListToDataTable<VASQLDataModel>(sqm);
                    UpdateVADDB(dt);

                }
                else
                {
                    listBox2.Items.Add($"Data error for Q: {race}");
                    log.Error($"Data error for Q: {race}");
                }

                string url;

                //if (dataType == 1)
                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_question&call_type=receipt&id={race}";
                //else
                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_question&call_type=receipt&id={race}";

                int nn = dataType * 2;
                url = $"{baseUrl}{stacktype}{stackTypes[nn]}{calltype}{callTypes[2]}{race}";


                string jsonData = SendAPIRequest(url);


            }
            catch (Exception ex)
            {
                log.Error($"Error processing Question Data: {race}  {ex}");

            }
        }

        public void GetAnswerData(string race)
        {
            try
            {

                textBox1.Clear();
                //string url;

                //if (dataType == 1)
                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_answer&call_type=data&id={race}";
                //else
                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_answer&call_type=data&id={race}";

                int n = dataType * 2 + 1;
                string url = $"{baseUrl}{stacktype}{stackTypes[n]}{calltype}{callTypes[1]}{race}";

                string jsonData = SendAPIRequest(url);
                textBox1.Text = jsonData;
                if (jsonData.Length > 10)
                    ProcessAnswerData(jsonData, race);
                else
                {
                    //if (dataType == 1)
                    //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_answer&call_type=receipt&id={race}";
                    //else
                    //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_answer&call_type=receipt&id={race}";

                    url = $"{baseUrl}{stacktype}{stackTypes[n]}{calltype}{callTypes[2]}{race}";

                    jsonData = SendAPIRequest(url);

                }
            }
            catch (Exception ex)
            {
                log.Error($"Error getting Answer Data: {race}  {ex}");

            }


        }

        public void ProcessAnswerData(string json, string race)
        {
            try
            {

                //string json = textBox1.Text;

                VAAnswerModel answers = new VAAnswerModel();

                string err = "stackTrace";
                int pos = json.IndexOf(err);
                if (pos == -1)
                {



                    answers = JsonConvert.DeserializeObject<VAAnswerModel>(json);


                    List<VASQLDataModel> sqm = new List<VASQLDataModel>();

                    int cnt = 0;
                    int n = answers.h_answers.Count() + 1;
                    int parsedResult;


                    //for (int i = 0; i < questions.Count(); i++)
                    {
                        //label1.Text = i.ToString();

                        for (int j = 0; j < answers.h_answers.Count(); j++)
                        {

                            for (int ri = 0; ri < answers.h_answers[j].results.Count() + 1; ri++)
                            {
                                VASQLDataModel sq = new VASQLDataModel();
                                string fTimeStr = answers.formatted_update_time;
                                DateTime fTime = Convert.ToDateTime(fTimeStr);

                                {
                                    cnt++;
                                    label2.Text = cnt.ToString();

                                    sq.VA_Data_Id = race;
                                    sq.race_id = answers.race_id;
                                    sq.r_type = "A";
                                    sq.questionId = answers.questionId;
                                    sq.qcode = answers.qcode;
                                    sq.question = answers.question;
                                    sq.q_order = Convert.ToInt32(answers.question_order);
                                    sq.filter = answers.filter;
                                    sq.state = answers.state;

                                    if (answers.race_type == "all")
                                        sq.ofc = "A";
                                    else
                                        sq.ofc = answers.race_type;

                                    sq.update_Time = answers.update_Time;
                                    //sq.f_update_time = DateTime.Now;
                                    sq.f_update_time = fTime;
                                    sq.preface = answers.preface;
                                    sq.pk = answers.pk;
                                    sq.def = Convert.ToBoolean(answers.def);
                                    sq.sample_size = Convert.ToInt32(answers.sample_size);
                                    sq.total_weight = Convert.ToSingle(answers.total_weight);
                                    sq.variable_count = Convert.ToInt32(answers.h_answers[j].variable_count);
                                    sq.variable_weight = Convert.ToSingle(answers.h_answers[j].variable_weight);
                                    sq.variable_percent = Convert.ToInt32(answers.h_answers[j].variable_percent);
                                    sq.answer = answers.h_answers[j].answer;
                                    sq.a_order = Convert.ToInt32(answers.h_answers[j].order);
                                    sq.answer_id = Convert.ToInt32(answers.h_answers[j].answer_id);

                                    if (ri == 0)
                                    {
                                        sq.name = string.Empty;
                                        sq.id = 0;
                                        sq.result_count = 0;
                                        sq.result_percent = 0;
                                        sq.result_weight = 0;
                                        sq.party = string.Empty;
                                        sq.result_order = 0;

                                    }
                                    else
                                    {
                                        sq.name = answers.h_answers[j].results[ri - 1].name;
                                        sq.id = Convert.ToInt32(answers.h_answers[j].results[ri - 1].id);
                                        sq.result_count = Convert.ToInt32(answers.h_answers[j].results[ri - 1].result_count);
                                        sq.result_percent = Convert.ToInt32(answers.h_answers[j].results[ri - 1].result_percent);
                                        sq.result_weight = Convert.ToSingle(answers.h_answers[j].results[ri - 1].result_weight);
                                        sq.party = answers.h_answers[j].results[ri - 1].party;

                                        if (sq.name == "Republican" || sq.name == "Democrat")
                                            sq.party = sq.name.Substring(0, 3);


                                        if (sq.party.Length > 3)
                                            sq.party = sq.party.Substring(0, 3);



                                        if (int.TryParse(answers.h_answers[j].results[ri - 1].order, out parsedResult))
                                            sq.result_order = Convert.ToInt32(answers.h_answers[j].results[ri - 1].order);
                                        else
                                            sq.result_order = cnt;

                                    }
                                    sq.stateId = GetStateID(answers.state);


                                    if (ri > 0)
                                        sqm.Add(sq);
                                }
                            }
                        }
                    }

                    dataGridView1.DataSource = sqm;
                    DataTable dt = new DataTable();
                    dt = ListToDataTable<VASQLDataModel>(sqm);
                    UpdateVADDB(dt);
                }
                else
                {
                    listBox2.Items.Add($"Data error for A: {race}");
                    log.Error($"Data error for A: {race}");

                }

                string url;

                //if (dataType == 1)
                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_answer&call_type=receipt&id={race}";
                //else
                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_answer&call_type=receipt&id={race}";

                int nn = dataType * 2 + 1;
                url = $"{baseUrl}{stacktype}{stackTypes[nn]}{calltype}{callTypes[2]}{race}";

                string jsonData = SendAPIRequest(url);



            }
            catch (Exception ex)
            {
                log.Error($"Error getting Answer Data: {race}  {ex}");

            }



        }

        private void btnGetAllUpdates_Click(object sender, EventArgs e)
        {
            string modeStr;
            string modeCode;
            for (int n = 0; n < 2; n++)
            {
                dataType = n;
                if (n == 1)
                {
                    modeStr = "Getting Fullscreen ";
                    modeCode = "FS";
                }
                else
                {
                    modeStr = "Getting Ticker ";
                    modeCode = "Tkr";

                }

                lblDataMode.Text = modeStr + "Questions";
                msgs.Add(modeStr + "Questions");
                GetQuestionUpdates();
                for (int i = 0; i < QuestionUpdates.Count; i++)
                {
                    GetQuestionData(QuestionUpdates[i].Trim());
                    msgs.Add(modeCode + "Q: " + QuestionUpdates[i].Trim());
                }
                QuestionUpdates.Clear();

                for (int i = 0; i < QuestionDeletes.Count; i++)
                {
                    GetQuestionData(QuestionDeletes[i].Trim());
                    msgs.Add($"Deleting {modeCode} Q: {QuestionDeletes[i].Trim()}");
                }
                QuestionDeletes.Clear();

                lblDataMode.Text = modeStr + "Answers";
                msgs.Add(modeStr + "Answers");
                GetAnswerUpdates();
                for (int i = 0; i < AnswerUpdates.Count; i++)
                {
                    GetAnswerData(AnswerUpdates[i].Trim());
                    msgs.Add(modeCode + "A: " + AnswerUpdates[i].Trim());
                }
                AnswerUpdates.Clear();

                for (int i = 0; i < AnswerDeletes.Count; i++)
                {
                    GetAnswerData(AnswerDeletes[i].Trim());
                    msgs.Add($"Deleting {modeCode} A: {AnswerDeletes[i].Trim()}");
                }
                AnswerDeletes.Clear();

            }
            lblDataMode.Text = "Done";
        }

        public void GetAll()
        {
            string modeStr;
            string modeCode;

            for (int n = 0; n < 2; n++)
            {
                dataType = n;
                if (n == 1)
                {
                    modeStr = "Getting Fullscreen ";
                    modeCode = "FS";
                }
                else
                {
                    modeStr = "Getting Ticker ";
                    modeCode = "Tkr";

                }
                msgs.Add(DateTime.Now.ToString());
                lblDataMode.Text = modeStr + "Questions";
                msgs.Add(modeStr + "Questions");
                GetQuestionUpdates();
                for (int i = 0; i < QuestionUpdates.Count; i++)
                {
                    GetQuestionData(QuestionUpdates[i].Trim());
                    label5.Text = $"Last Data Received: {DateTime.Now}";
                    msgs.Add(modeCode + "Q: " + QuestionUpdates[i].Trim());
                    log.Debug($"{modeStr} Q: {QuestionUpdates[i].Trim()}");
                }
                QuestionUpdates.Clear();

                for (int i = 0; i < QuestionDeletes.Count; i++)
                {
                    DeleteData(QuestionDeletes[i].Trim(), dataType, "Q");
                    msgs.Add($"Deleting {modeCode} Q: {QuestionDeletes[i].Trim()}");
                    log.Debug($"Deleting Q: {QuestionDeletes[i].Trim()}");
                }
                QuestionDeletes.Clear();

                lblDataMode.Text = modeStr + "Answers";
                msgs.Add(modeStr + "Answers");
                GetAnswerUpdates();
                for (int i = 0; i < AnswerUpdates.Count; i++)
                {
                    GetAnswerData(AnswerUpdates[i].Trim());
                    label5.Text = $"Last Data Received: {DateTime.Now}";
                    msgs.Add(modeCode + "A: " + AnswerUpdates[i].Trim());
                    log.Debug($"{modeCode} A: {AnswerUpdates[i].Trim()}");
                }
                AnswerUpdates.Clear();

                for (int i = 0; i < AnswerDeletes.Count; i++)
                {
                    DeleteData(AnswerDeletes[i].Trim(), dataType, "A");
                    msgs.Add($"Deleting {modeCode} A: {AnswerDeletes[i].Trim()}");
                    log.Debug($"Deleting A: {AnswerDeletes[i].Trim()}");
                }
                AnswerDeletes.Clear();


            }
            msgs.Add(DateTime.Now.ToString());

            modeStr = "Getting Maps ";
            modeCode = "Map";

            lblDataMode.Text = modeStr + "Map";
            msgs.Add(modeStr + "Map");
            GetMapUpdates();
            for (int i = 0; i < MapUpdates.Count; i++)
            {
                GetMapData(MapUpdates[i].Trim());
                label5.Text = $"Last Data Received: {DateTime.Now}";
                msgs.Add(modeCode + " M: " + MapUpdates[i].Trim());
                log.Debug($"{modeStr} M: {MapUpdates[i].Trim()}");
            }
            MapUpdates.Clear();

            for (int i = 0; i < MapDeletes.Count; i++)
            {
                DeleteData(MapDeletes[i].Trim(), dataType, "M");
                msgs.Add($"Deleting {modeCode}  M: {MapDeletes[i].Trim()}");
                log.Debug($"Deleting M: {MapDeletes[i].Trim()}");
            }
            MapDeletes.Clear();


            msgs.Add(DateTime.Now.ToString());

            lblDataMode.Text = "Done";
            timer2.Enabled = true;

        }

        public void DeleteData(string VA_Data, int dataType, string QnA)
        {

            try
            {

                string tblName = "FE_VoterAnalysisData_Ticker";

                if (dataType == 1)
                    tblName = "FE_VoterAnalysisData_FullScreen";

                if (QnA == "M")
                    tblName = "FE_VoterAnalysisData_Map";


                string delCmd = $"DELETE FROM {tblName} WHERE VA_Data_Id = '{VA_Data}'";
                IssueSQLCmd(delCmd);

                string url = "";

                int n;

                if (QnA == "A")
                {
                    n = dataType * 2 + 1;
                    url = $"{baseUrl}{stacktype}{stackTypes[n]}{calltype}{callTypes[2]}{VA_Data}";

                    //if (dataType == 1)
                    //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_answer&call_type=receipt&id={VA_Data}";
                    //else
                    //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_answer&call_type=receipt&id={VA_Data}";
                }


                if (QnA == "Q")
                {
                    n = dataType * 2;
                    url = $"{baseUrl}{stacktype}{stackTypes[n]}{calltype}{callTypes[2]}{VA_Data}";

                    //if (dataType == 1)
                    //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_question&call_type=receipt&id={VA_Data}";
                    //else
                    //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_question&call_type=receipt&id={VA_Data}";
                }


                if (QnA == "M")
                {
                    //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=map&call_type=receipt&id={VA_Data}";
                    url = $"{baseUrl}{stacktype}{stackTypes[4]}{calltype}{callTypes[2]}{VA_Data}";

                }

                string jsonData = SendAPIRequest(url);

            }
            catch (Exception ex)
            {
                log.Error($"Delete Error: {VA_Data} {ex}");

            }

        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            listBox1.Items.Clear();
            string url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=map&call_type=updates";


            string jsonData = SendAPIRequest(url);

            textBox1.Text = jsonData;

            string racesWithData = jsonData.Replace("\"", "");
            jsonData = racesWithData;
            racesWithData = jsonData.Replace("[", "");
            jsonData = racesWithData;
            racesWithData = jsonData.Replace("]", "");
            jsonData = racesWithData;
            racesWithData = jsonData.Replace(" ", "");


            // parse the header info
            string[] strSeparator = new string[] { "," };
            string[] Races;

            // this takes the header and splits it into key-value pairs
            Races = racesWithData.Split(strSeparator, StringSplitOptions.None);
            for (int i = 0; i < Races.Length; i++)
            {
                listBox1.Items.Add(Races[i]);
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            string raceID = tbRace.Text.Trim();
            int pos = raceID.IndexOf("|");
            string deleteStr = raceID.Substring(pos + 1);
            string race = raceID.Substring(0, pos);

            string url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=map&call_type=data&id={race}";
            string jsonData = SendAPIRequest(url);
            textBox1.Text = jsonData;

        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {

                string race = tbRace.Text.Trim();
                int pos = race.IndexOf("|");
                string deleteStr = race.Substring(pos + 1);
                string VA_Data_Id = race.Substring(0, pos);

                string json = textBox1.Text;

                MapDataModel mdat = new MapDataModel();
                mdat = JsonConvert.DeserializeObject<MapDataModel>(json);

                List<MapSQLModel> mData = new List<MapSQLModel>();

                int cnt = 0;
                int n = mdat.answers.Count();

                for (int j = 0; j < n; j++)
                {
                    MapSQLModel mdm = new MapSQLModel();
                    cnt++;
                    label2.Text = cnt.ToString();
                    mdm.VA_Data_Id = VA_Data_Id;
                    mdm.r_type = "M";
                    mdm.answer = mdat.answer;
                    mdm.filter = mdat.filter;
                    mdm.question = mdat.question;
                    mdm.state = mdat.answers[j].state;
                    mdm.percent = Convert.ToInt32(mdat.answers[j].percent);

                    mData.Add(mdm);

                }

                dataGridView1.DataSource = mData;
                DataTable dt = new DataTable();
                dt = ListToDataTable<MapSQLModel>(mData);
                UpdateVAMapData(dt);

            }
            catch (Exception ex)
            {
                var info = ex.Message;
            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            lblDataMode.Text = "Starting Refresh";

            string url;
            string jsonData;


            for (int i = 0; i < 5; i++)
            {
                url = $"{baseUrl}{stacktype}{stackTypes[i]}{calltype}{callTypes[3]}";
                jsonData = SendAPIRequest(url);
            }
            //string url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_answer&call_type=refresh";
            //string jsonData = SendAPIRequest(url);

            //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_question&call_type=refresh";
            //jsonData = SendAPIRequest(url);

            //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_question&call_type=refresh";
            //jsonData = SendAPIRequest(url);

            //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_answer&call_type=refresh";
            //jsonData = SendAPIRequest(url);

            //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=map&call_type=refresh";
            //jsonData = SendAPIRequest(url);

            lblDataMode.Text = "Refresh Complete";

        }

        public void IssueSQLCmd(string cmdStr)
        {
            try
            {
                SqlConnection conn = new SqlConnection(dbConn);
                conn.Open();
                SqlCommand cmd = new SqlCommand(cmdStr, conn);
                int aSum = Convert.ToInt32(cmd.ExecuteScalar());

                //dispose iDisposable objects:
                conn.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                var info = ex.Message;
            }

        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < msgs.Count; i++)
            {
                listBox2.Items.Add(msgs[i]);
            }
            msgs.Clear();
            listBox2.SelectedIndex = listBox2.Items.Count - 1;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            cnt++;
            if (cnt == 1)
            {
                timer2.Enabled = false;
                lblDataMode.Text = "Running...";
                GetAll();
                cnt = 0;
            }
            else
                lblDataMode.Text = $"Timer cnt: {cnt}";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            lblDataMode.Text = "Stopped";

        }

        private void button13_Click(object sender, EventArgs e)
        {
            lblDataMode.Text = "Running...";
            GetAll();
        }

        public void GetMapUpdates()
        {
            try
            {
                textBox1.Clear();
                listBox1.Items.Clear();
                //string url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=map&call_type=updates";

                int n = dataType * 2 + 1;
                string url = $"{baseUrl}{stacktype}{stackTypes[4]}{calltype}{callTypes[0]}";

                string jsonData = SendAPIRequest(url);

                textBox1.Text = jsonData;

                //if (jsonData != "[]")
                if (jsonData.Length > 1 && jsonData != "[]")
                {
                    string racesWithData = jsonData.Replace("\"", "");
                    jsonData = racesWithData;
                    racesWithData = jsonData.Replace("[", "");
                    jsonData = racesWithData;
                    racesWithData = jsonData.Replace("]", "");
                    jsonData = racesWithData;
                    racesWithData = jsonData.Replace(" ", "");


                    // parse the header info
                    string[] strSeparator = new string[] { "," };
                    string[] Races;

                    // this takes the header and splits it into key-value pairs
                    Races = racesWithData.Split(strSeparator, StringSplitOptions.None);
                    int pos;
                    string deleteStr;


                    for (int i = 0; i < Races.Length; i++)
                    {
                        listBox1.Items.Add(Races[i]);
                        pos = Races[i].IndexOf("|");
                        deleteStr = Races[i].Substring(pos + 1);
                        Races[i] = Races[i].Substring(0, pos);
                        if (deleteStr == "delete")
                            MapDeletes.Add(Races[i]);
                        else
                            MapUpdates.Add(Races[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error getting Map Updates  {ex}");

            }

        }

        public void GetMapData(string race)
        {
            try
            {

                textBox1.Clear();

                //string raceID = tbRace.Text.Trim();
                //int pos = raceID.IndexOf("|");
                //string deleteStr = raceID.Substring(pos + 1);
                //string race = raceID.Substring(0, pos);

                //string url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=map&call_type=data&id={race}";

                //int n = dataType * 2 + 1;
                string url = $"{baseUrl}{stacktype}{stackTypes[4]}{calltype}{callTypes[1]}{race}";

                string jsonData = SendAPIRequest(url);
                textBox1.Text = jsonData;

                if (jsonData.Length > 10)
                    ProcessMapData(jsonData, race);
                else
                {
                    //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=map&call_type=receipt&id={race}";
                    url = $"{baseUrl}{stacktype}{stackTypes[4]}{calltype}{callTypes[2]}{race}";
                    jsonData = SendAPIRequest(url);
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error getting Map Data: {race}  {ex}");

            }

        }
        public void ProcessMapDataNew(string json, string race)
        {
            try
            {
                string VA_Data_Id = race;
                string title = "";

                MapDataModelNew mdat = new MapDataModelNew();

                string err = "stackTrace";
                int pos = json.IndexOf(err);
                if (pos != -1)
                {
                    err = "something went wrong";
                    pos = json.IndexOf(err);
                }

                if (pos == -1)
                {

                    mdat = JsonConvert.DeserializeObject<MapDataModelNew>(json);

                    List<MapSQLModelNew> mData = new List<MapSQLModelNew>();

                    int cnt = 0;
                    int n = mdat.data.Count();

                    for (int j = 0; j < n; j++)
                    {
                        MapSQLModelNew mdm = new MapSQLModelNew();
                        cnt++;
                        //label2.Text = cnt.ToString();
                        mdm.VA_Data_Id = VA_Data_Id;
                        mdm.r_type = "M";
                        mdm.answer = mdat.data[j].answer;
                        mdm.qcode = mdat.data[j].qcode;
                        mdm.response_sum = Convert.ToInt32(mdat.data[j].response_sum);
                        mdm.percent = Convert.ToInt32(mdat.data[j].percent);
                        mdm.question = mdat.data[j].question;
                        mdm.state = mdat.data[j].state;
                        mdm.date = mdat.data[j].date;
                        mdm.breakpoint = Convert.ToInt32(mdat.data[j].breakpoint);
                        mdm.stage = mdat.data[j].stage;

                        mData.Add(mdm);

                        if (j == 0)
                            title = mdm.answer;

                    }

                    dataGridView1.DataSource = mData;
                    DataTable dt = new DataTable();
                    dt = ListToDataTable<MapSQLModelNew>(mData);
                    UpdateVAMapDataNew(dt);


                    int numBands = mdat.breakpoints.Length + 1;
                    int numRec = getPrevNumBands(VA_Data_Id);
                    if (numRec > numBands)
                        deleteMapMetaDataRecords(VA_Data_Id, numBands);

                    List<MapMetaDataModelNew> mmData = new List<MapMetaDataModelNew>();

                    for (int i = 0; i < numBands; i++)
                    {
                        MapMetaDataModelNew mmd = new MapMetaDataModelNew();
                        mmd.VA_Data_Id = VA_Data_Id;
                        mmd.band = i;
                        mmd.numColorBands = numBands;
                        mmd.Title = title;
                        mmd.color = mdat.palette;

                        if (mdat.palette == "purple")
                            mmd.colorIndex = 0;
                        if (mdat.palette == "green")
                            mmd.colorIndex = 1;
                        if (mdat.palette == "orange")
                            mmd.colorIndex = 2;

                        int colorVal = (mmd.colorIndex * 4) + 1;

                        switch (i)
                        {
                            case 0:
                                mmd.bandLo = mdat.lower_limit;
                                mmd.bandHi = mdat.breakpoints[i] - 1;
                                break;

                            case 1:
                                mmd.bandLo = mdat.breakpoints[i - 1];
                                mmd.bandHi = mdat.breakpoints[i] - 1;
                                break;

                            case 2:
                                mmd.bandLo = mdat.breakpoints[i - 1];
                                if (numBands == 3)
                                    mmd.bandHi = mdat.upper_limit;
                                else
                                    mmd.bandHi = mdat.breakpoints[i] - 1;
                                break;

                            case 3:
                                mmd.bandLo = mdat.breakpoints[i - 1];
                                mmd.bandHi = mdat.upper_limit;
                                break;

                        }
                        mmd.bandLabel = $"{mmd.bandLo}% - {mmd.bandHi}%";
                        mmd.colorValue = colorVal + i;

                        mmData.Add(mmd);
                        UpdateVAMapMetaDataNew(mmd);


                    }
                    SendReceipt(VA_Data_Id);

                    string s = $"X: {VA_Data_Id}  OK";
                    if (this.InvokeRequired)
                        this.Invoke(new ListErr(writeListbox2), s);
                    else
                        listBox2.Items.Add(s);

                }
                else
                {
                    log.Error($"Data error for M: {race}");

                    string s = $"Data error for Q: {VA_Data_Id}";
                    if (this.InvokeRequired)
                        this.Invoke(new ListErr(writeListbox2), s);
                    else
                        listBox2.Items.Add(s);

                }


                //url = $"{baseUrl}{stacktype}{stackTypes[4]}{calltype}{callTypes[2]}{race}";
                //jsonData = SendAPIRequest(url);

            }
            catch (Exception ex)
            {
                log.Error($"Error processing Map Data: {race}  {ex}");
                string s = $"Error processing Map Data: {race}";
                if (this.InvokeRequired)
                    this.Invoke(new ListErr(writeListbox2), s);
                else
                    listBox2.Items.Add(s);
            }

        }


        public void ProcessMapData(string json, string race)
        {
            try
            {

                //int pos = race.IndexOf("|");
                //string deleteStr = race.Substring(pos + 1);
                //string VA_Data_Id = race.Substring(0, pos);
                string VA_Data_Id = race;

                string url;
                string jsonData;
                string timeStr;


                MapDataModel mdat = new MapDataModel();

                string err = "stackTrace";
                int pos = json.IndexOf(err);
                if (pos == -1)
                {
                    err = "something went wrong";
                    pos = json.IndexOf(err);
                }

                if (pos == -1)
                {

                    mdat = JsonConvert.DeserializeObject<MapDataModel>(json);

                    List<MapSQLModel> mData = new List<MapSQLModel>();

                    //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=map&call_type=receipt&id={race}";
                    //jsonData = SendAPIRequest(url);

                    int cnt = 0;
                    int n = mdat.answers.Count();

                    for (int j = 0; j < n; j++)
                    {
                        MapSQLModel mdm = new MapSQLModel();
                        cnt++;
                        label2.Text = cnt.ToString();
                        mdm.VA_Data_Id = VA_Data_Id;
                        mdm.r_type = "M";
                        mdm.answer = mdat.answer;
                        mdm.filter = mdat.filter;
                        mdm.question = mdat.question;
                        mdm.formatted_update_time = mdat.formatted_update_time;
                        mdm.state = mdat.answers[j].state;
                        mdm.percent = Convert.ToInt32(mdat.answers[j].percent);

                        mData.Add(mdm);

                    }

                    dataGridView1.DataSource = mData;
                    DataTable dt = new DataTable();
                    dt = ListToDataTable<MapSQLModel>(mData);
                    UpdateVAMapData(dt);

                }
                else
                {
                    listBox2.Items.Add($"Data error for M: {race}");
                    log.Error($"Data error for M: {race}");
                }

                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=map&call_type=receipt&id={race}";

                url = $"{baseUrl}{stacktype}{stackTypes[4]}{calltype}{callTypes[2]}{race}";
                jsonData = SendAPIRequest(url);

            }
            catch (Exception ex)
            {
                log.Error($"Error processing Map Data: {race}  {ex}");
            }

        }

        private void button14_Click(object sender, EventArgs e)
        {
            int type = 0;
            listBox1.Items.Clear();
            VAPostModel test = new VAPostModel();

            if (rbA.Checked)
            {
                test.stack_type = "fullscreen-answer";
                test.request_type = "stack";
            }
            if (rbQ.Checked)
            {
                test.stack_type = "fullscreen-question";
                test.request_type = "stack";
            }
            if (rbMan.Checked)
            {
                test.stack_type = "";
                test.request_type = "manual";
            }
            if (rbMaps.Checked)
            {
                test.request_type = "map";
                test.stack_type = "";
            }


            // clear updates lists
            QuestionDeletes.Clear();
            AnswerDeletes.Clear();
            ManualDeletes.Clear();
            MapDeletes.Clear();

            QuestionUpdates.Clear();
            AnswerUpdates.Clear();
            ManualUpdates.Clear();
            MapUpdates.Clear();

            test.method = "updates";
            //test.election_event = "2018_Midterms";
            test.election_event = electionEvent;

            string JSONrequest = JsonConvert.SerializeObject(test);

            string result = SendAPIPostRequest(JSONrequest);

            textBox1.Text = result;

            string jsonData = result;

            if (jsonData.Length > 1 && jsonData != "[]")
            {
                string racesWithData = jsonData.Replace("\"", "");
                jsonData = racesWithData;
                racesWithData = jsonData.Replace("[", "");
                jsonData = racesWithData;
                racesWithData = jsonData.Replace("]", "");
                jsonData = racesWithData;
                //racesWithData = jsonData.Replace(" ", "");


                // parse the header info
                string[] strSeparator = new string[] { "," };
                string[] Races;

                // this takes the header and splits it into key-value pairs
                Races = racesWithData.Split(strSeparator, StringSplitOptions.None);
                int pos;
                string deleteStr;


                for (int i = 0; i < Races.Length; i++)
                {
                    listBox1.Items.Add(Races[i]);
                    log.Info($"Updates[{i}]: {Races[i]}");
                    pos = Races[i].IndexOf("|");
                    deleteStr = Races[i].Substring(pos + 1);
                    Races[i] = Races[i].Substring(0, pos);
                    if (rbQ.Checked)
                    {
                        if (deleteStr == "delete")
                            QuestionDeletes.Add(Races[i]);
                        else if (deleteStr == "create")
                            QuestionUpdates.Add(Races[i]);
                    }
                    else if (rbA.Checked)
                    {
                        if (deleteStr == "delete")
                            AnswerDeletes.Add(Races[i]);
                        else if (deleteStr == "create")
                            AnswerUpdates.Add(Races[i]);
                    }
                    else if (rbMan.Checked)
                    {
                        if (deleteStr == "delete")
                            ManualDeletes.Add(Races[i]);
                        else if (deleteStr == "create")
                            ManualUpdates.Add(Races[i]);
                    }
                    else if (rbMaps.Checked)
                    {
                        if (deleteStr == "delete")
                            MapDeletes.Add(Races[i]);
                        else if (deleteStr == "create")
                            MapUpdates.Add(Races[i]);
                    }
                }
            }


        }

        private void button15_Click(object sender, EventArgs e)
        {

            int n = 0;
            if (rbQ.Checked)
                n = QuestionUpdates.Count;
            if (rbA.Checked)
                n = AnswerUpdates.Count;
            if (rbMan.Checked)
                n = ManualUpdates.Count;
            if (rbMaps.Checked)
                n = MapUpdates.Count;


            for (int i = 0; i < n; i++)
            {
                if (rbA.Checked)
                    ProcessUpdate(AnswerUpdates[i]);
                if (rbQ.Checked)
                    ProcessUpdate(QuestionUpdates[i]);
                if (rbMan.Checked)
                    ProcessUpdate(ManualUpdates[i]);
                if (rbMaps.Checked)
                    ProcessUpdate(MapUpdates[i]);

            }

        }
        public void ProcessUpdate(string update)
        {
            string fullTick = "";
            string faq = "";
            //tbRace.Text = update;
            // parse the header info
            string[] strSeparator = new string[] { ":" };
            string[] Races;

            // this takes the header and splits it into key-value pairs
            Races = update.Split(strSeparator, StringSplitOptions.None);
            int iSplit = Races.Length;
            string method = "";

            if (iSplit >= 4)
                method = Races[3];
            else
                method = "manual";

            strSeparator = new string[] { "-" };
            string[] qa;
            qa = method.Split(strSeparator, StringSplitOptions.None);
            if (qa[0] == "manual")
            {
                fullTick = "manual";
                faq = "manual";
            }
            else if (qa[0] == "map")
            {
                fullTick = "map";
                faq = "map";
            }
            else
            {
                fullTick = qa[0];
                faq = qa[1];
            }

            VAPostModel test = new VAPostModel();

            if (faq == "manual")
            {
                test.request_type = "manual";
                test.stack_type = "";
                test.method = "data";
                test.id = update;
                test.election_event = electionEvent;

            }
            else if (faq == "map")
            {
                test.request_type = "map";
                test.stack_type = "";
                test.method = "data";
                test.id = update;
                test.election_event = electionEvent;
            }
            else
            {
                test.request_type = "stack";
                test.stack_type = method;
                test.method = "data";
                test.id = update;
                test.election_event = electionEvent;
            }

            log.Info($"");
            log.Info($"  Processing:    {update}");
            //DeleteDataNew(update, false);

            string JSONrequest = JsonConvert.SerializeObject(test);
            log.Info(JSONrequest);

            string result = SendAPIPostRequest(JSONrequest);
            string jsonData = result;
            if (this.InvokeRequired)
                this.Invoke(new TextWrite(writeTextbox), result);
            else
                textBox1.Text = result;

            log.Info($" Data received:  {result}");
            string err = "stackTrace";
            int pos = jsonData.IndexOf(err);
            if (pos >= 0)
            {
                // Error - send receipt
                SendReceipt(update);

                string s = $"Data Error for: {update}";
                if (this.InvokeRequired)
                    this.Invoke(new ListErr(writeListbox2), s);
                else
                    listBox2.Items.Add(s);

            }
            else
            {
                if (faq == "answer")
                    ProcessAnswerDataNew(jsonData, update, fullTick);
                else if (faq == "question")
                    ProcessQuestionDataNew(jsonData, update, fullTick);
                else if (faq == "manual")
                    ProcessManualDataNew(jsonData, update, fullTick);
                else if (faq == "map")
                    ProcessMapDataNew(jsonData, update);

            }

        }

        public void SendReceipt(string update)
        {
            VAPostModel test = new VAPostModel();
            // parse the header info
            string[] strSeparator = new string[] { ":" };
            string[] Races;

            // this takes the header and splits it into key-value pairs
            Races = update.Split(strSeparator, StringSplitOptions.None);
            if (Races.Length > 1)
            {
                string method = Races[3];
                if (method == "map")
                {
                    //{'request_type': 'map', 'method': 'receipt', 'id': '2020_Primaries:Voters:age:2', 'election_event': '2020_Primaries'}
                    test.request_type = "map";
                    test.stack_type = "";
                    test.method = "receipt";
                    test.id = update;
                    test.election_event = electionEvent;
                }
                else
                {
                    test.request_type = "stack";
                    test.stack_type = method;
                    test.method = "receipt";
                    test.id = update;
                    test.election_event = electionEvent;
                }
            }
            else
            {
                test.request_type = "manual";
                test.stack_type = "";
                test.method = "receipt";
                test.id = update;
                test.election_event = electionEvent;

            }

            string JSONrequest = JsonConvert.SerializeObject(test);

            string result = "";
            result = SendAPIPostRequest(JSONrequest);

            log.Info($"SendReceipt: {result}");
            log.Info($"");

            string jsonData = result;
            if (this.InvokeRequired)
                this.Invoke(new TextWrite(writeTextbox), result);
            else
                textBox1.Text = result;

        }

        public void SendManualReceipt(string update)
        {
            // parse the header info
            string[] strSeparator = new string[] { ":" };
            string[] Races;

            // this takes the header and splits it into key-value pairs
            //Races = update.Split(strSeparator, StringSplitOptions.None);
            //string method = Races[3];

            VAPostModel test = new VAPostModel();

            test.request_type = "manual";
            test.stack_type = "";
            test.method = "receipt";
            test.id = update;
            test.election_event = electionEvent;


            string JSONrequest = JsonConvert.SerializeObject(test);

            string result = SendAPIPostRequest(JSONrequest);
            log.Info($"SendReceipt: {result}");
            log.Info($"");

            string jsonData = result;
            if (this.InvokeRequired)
                this.Invoke(new TextWrite(writeTextbox), result);
            else
                textBox1.Text = result;

        }


        public void ProcessQuestionDataNew(string json, string update, string fullTick)
        {
            try
            {
                VAQuestionModelNew questions = new VAQuestionModelNew();

                string err = "stackTrace";
                int pos = json.IndexOf(err);
                if (pos == -1)
                {
                    questions = JsonConvert.DeserializeObject<VAQuestionModelNew>(json);

                    List<VASQLDataModelNew> sqm = new List<VASQLDataModelNew>();

                    int cnt = 0;
                    int n = questions.h_answers.Count() + 1;

                    for (int j = 0; j < questions.h_answers.Count(); j++)
                    {

                        VASQLDataModelNew sq = new VASQLDataModelNew();

                        cnt++;
                        //label2.Text = cnt.ToString();

                        sq.VA_Data_Id = update;
                        sq.r_type = "Q";
                        sq.q_order = questions.question_order;
                        sq.questionId = questions.questionId;
                        sq.st = questions.State;
                        sq.State = GetStateName(sq.st);
                        sq.race_id = questions.race_id;

                        if (questions.question.Length > 120)
                            sq.question = questions.question.Substring(0, 120);
                        else
                            sq.question = questions.question;

                        sq.qcode = questions.qcode;
                        sq.filter = questions.filter;
                        sq.sample_size = (int)Convert.ToSingle(questions.sample_size);
                        sq.total_weight = Convert.ToSingle(questions.total_weight);

                        if (questions.race_type == "all")
                        {
                            sq.ofc = "A";
                            sq.race_type = questions.race_type.Substring(0, 2);
                        }
                        else
                        {
                            sq.ofc = questions.race_type;
                            sq.race_type = questions.race_type;

                        }


                        sq.preface = questions.preface;
                        if (sq.ofc == "S")
                            sq.preface = questions.preface + " SENATE";
                        else if (sq.ofc == "G")
                            sq.preface = questions.preface + "GOVERNOR";
                        
                        sq.header = questions.header;
                        sq.updated = questions.last_updated;
                        sq.election_event = questions.election_event;


                        sq.variable_weight = (int)Convert.ToSingle(questions.h_answers[j].variable_weight);
                        sq.variable_count = (int)Convert.ToSingle(questions.h_answers[j].variable_count);
                        sq.variable_percent = Convert.ToInt32(questions.h_answers[j].variable_percent);
                        sq.original_order = Convert.ToInt32(questions.h_answers[j].original_order);

                        sq.name = questions.h_answers[j].original_name;
                        if (sq.name.Length > 50)
                            sq.name = sq.name.Substring(0, 50);


                        sq.alias = questions.h_answers[j].alias_name;
                        sq.new_order = Convert.ToInt32(questions.h_answers[j].new_order);

                        sq.id = 0;
                        sq.result_count = 0;
                        sq.result_percent = 0;
                        sq.result_weight = 0;
                        sq.party = string.Empty;

                        sqm.Add(sq);

                    }

                    //dataGridView1.DataSource = sqm;

                    DeleteDataNew(update, false);

                    DataTable dt = new DataTable();
                    dt = ListToDataTable(sqm);
                    UpdateVADDBNew(dt, fullTick);

                    SendReceipt(update);

                    string s = $"Q: {update}  OK";
                    if (this.InvokeRequired)
                        this.Invoke(new ListErr(writeListbox2), s);
                    else
                        listBox2.Items.Add(s);

                }
                else
                {
                    //listBox2.Items.Add($"Data error for Q: {update}");
                    log.Error($"Data error for Q: {update}");

                    string s = $"Data error for Q: {update}";
                    if (this.InvokeRequired)
                        this.Invoke(new ListErr(writeListbox2), s);
                    else
                        listBox2.Items.Add(s);

                }
            }
            catch (Exception ex)
            {
                log.Info($"");
                log.Error($"Error processing Question Data: {update}  {ex}");
                string s = $"Error processing Question Data: {update}";
                if (this.InvokeRequired)
                    this.Invoke(new ListErr(writeListbox2), s);
                else
                    listBox2.Items.Add(s);

            }
        }

        public void ProcessManualDataNew(string json, string update, string fullTick)
        {
            try
            {
                VAManualModelNew manual = new VAManualModelNew();


                if (json == "[]")
                {
                    SendManualReceipt(update);
                    return;
                }

                string err = "stackTrace";
                int pos = json.IndexOf(err);
                if (pos == -1)
                {
                    manual = JsonConvert.DeserializeObject<VAManualModelNew>(json);

                    List<VASQLManualDataModelNew> sqm = new List<VASQLManualDataModelNew>();

                    int cnt = 0;
                    int n = manual.answers.Count() + 1;

                    for (int j = 0; j < manual.answers.Count(); j++)
                    {

                        VASQLManualDataModelNew sq = new VASQLManualDataModelNew();

                        cnt++;
                        //label2.Text = cnt.ToString();

                        sq.VA_Data_Id = update;
                        sq.r_type = "M";
                        sq.qcode = manual.qcode;
                        sq.question = manual.question;
                        sq.st = GetStateAbbv(manual.State.ToUpper());
                        sq.State = manual.State.ToUpper();
                        sq.race = manual.race;
                        sq.race_type = "P";

                        if (sq.race_type == "all")
                            sq.ofc = "A";
                        else
                            sq.ofc = sq.race_type;

                        sq.label = manual.label;
                        sq.text = manual.answers[j].text;
                        sq.value = manual.answers[j].value;
                        sq.order = j + 1;

                        sq.updated = manual.last_updated;
                        sq.election_event = manual.election_event;


                        sq.active = manual.active;
                        sq.status = manual.status;

                        sqm.Add(sq);

                    }

                    DeleteDataNew(update, false);


                    DataTable dt = new DataTable();
                    dt = ListToDataTable(sqm);
                    UpdateVADDBNew(dt, fullTick);

                    SendManualReceipt(update);

                    string s = $"M: {update}  OK";
                    if (this.InvokeRequired)
                        this.Invoke(new ListErr(writeListbox2), s);
                    else
                        listBox2.Items.Add(s);
                }
                else
                {
                    listBox2.Items.Add($"Data error for MAN: {update}");
                    log.Error($"Data error for MAN: {update}");
                    string s = $"Data error for MAN: {update}";
                    if (this.InvokeRequired)
                        this.Invoke(new ListErr(writeListbox2), s);
                    else
                        listBox2.Items.Add(s);
                }



            }
            catch (Exception ex)
            {
                log.Info($"");
                log.Error($"Error processing Question Data: {update}  {ex}");
            }
        }



        public void UpdateVADDBNew(DataTable dt, string fullTick)
        {

            string paramName = "@tblVADNew";
            string spName = "spUpdate_VoterAnalysisData_Ticker_New";

            if (fullTick == "fullscreen")
                spName = "spUpdate_VoterAnalysisData_Fullscreen_New";

            if (fullTick == "manual")
            {
                spName = "spUpdate_VoterAnalysisManualData_Fullscreen_New";
                paramName = "@tblVADManualNew";
            }

            string cmdStr = $"{spName} ";

            //Save out the top-level metadata
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConn))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update Voter Analysis Data");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                            try
                            {
                                //Specify base command
                                cmd.CommandText = cmdStr;

                                cmd.Parameters.Add(paramName, SqlDbType.Structured).Value = dt;

                                sqlDataAdapter.SelectCommand = cmd;
                                sqlDataAdapter.SelectCommand.Connection = connection;
                                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                                // Execute stored proc 
                                sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                //Attempt to commit the transaction
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                string msg = "UpdateData - SQL Command Exception occurred: " + ex.Message;
                                if (this.InvokeRequired)
                                    this.Invoke(new TextWrite(writeTextbox), msg);
                                else
                                    textBox1.Text = msg;
                                //textBox1.Text = "UpdateData - SQL Command Exception occurred: " + ex.Message;
                                log.Error("UpdateData- SQL Command Exception occurred: " + ex.Message);
                                //log.Debug("UpdateData- SQL Command Exception occurred", ex);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);
                //textBox1.Text = "UpdateData - SQL Command Exception occurred: " + ex.Message;
                string msg = "UpdateData - SQL Command Exception occurred: " + ex.Message;
                if (this.InvokeRequired)
                    this.Invoke(new TextWrite(writeTextbox), msg);
                else
                    textBox1.Text = msg;

            }

        }

        public void ProcessAnswerDataNew(string json, string update, string fullTick)
        {
            try
            {
                VAAnswerModelNew answers = new VAAnswerModelNew();

                string err = "stackTrace";
                int pos = json.IndexOf(err);
                if (pos == -1)
                {

                    answers = JsonConvert.DeserializeObject<VAAnswerModelNew>(json);

                    List<VASQLDataModelNew> sqm = new List<VASQLDataModelNew>();

                    int cnt = 0;
                    int n = answers.h_answers.Count() + 1;


                    for (int j = 0; j < answers.h_answers.Count(); j++)
                    {

                        for (int ri = 0; ri < answers.h_answers[j].results.Count() + 1; ri++)
                        {
                            VASQLDataModelNew sq = new VASQLDataModelNew();
                            string fTimeStr = answers.last_updated.Trim();
                            DateTime fTime = Convert.ToDateTime(fTimeStr);

                            sq.VA_Data_Id = update;
                            sq.r_type = "A";
                            sq.qcode = answers.qcode;
                            sq.filter = answers.filter;
                            sq.questionId = answers.questionId;
                            sq.q_order = Convert.ToInt32(answers.question_order);

                            if (answers.question.Length > 120)
                                sq.question = answers.question.Substring(0, 120);
                            else
                                sq.question = answers.question;

                            sq.st = answers.state;
                            sq.State = GetStateName(sq.st);
                            sq.race_id = answers.race_id;
                            if (answers.race_type == "all")
                            {
                                sq.ofc = "A";
                                sq.race_type = answers.race_type.Substring(0, 2);
                            }
                            else
                            {
                                sq.ofc = answers.race_type;
                                sq.race_type = answers.race_type;
                            }

                            sq.sample_size = (int)Convert.ToSingle(answers.sample_size);
                            sq.total_weight = Convert.ToSingle(answers.total_weight);

                            sq.preface = answers.preface;
                            if (sq.ofc == "S")
                                sq.preface = answers.preface + " SENATE";
                            else if (sq.ofc == "G")
                                sq.preface = answers.preface + "GOVERNOR";

                            sq.header = answers.header;
                            sq.updated = answers.last_updated;
                            sq.election_event = answers.election_event;

                            sq.variable_weight = Convert.ToSingle(answers.h_answers[j].variable_weight);
                            sq.variable_count = (int)Convert.ToSingle(answers.h_answers[j].variable_count);
                            sq.variable_percent = Convert.ToInt32(answers.h_answers[j].variable_percent);
                            sq.original_order = Convert.ToInt32(answers.h_answers[j].original_order);
                            sq.original_name = answers.h_answers[j].original_name;
                            if (sq.original_name.Length > 50)
                                sq.original_name = sq.original_name.Substring(0, 50);


                            cnt++;
                            //label2.Text = cnt.ToString();

                            if (ri == 0)
                            {
                                sq.name = string.Empty;
                                sq.id = 0;
                                sq.result_count = 0;
                                sq.result_percent = 0;
                                sq.result_weight = 0;
                                sq.party = string.Empty;

                            }
                            else
                            {
                                sq.name = answers.h_answers[j].results[ri - 1].original_name;
                                if (sq.name.Length > 50)
                                    sq.name = sq.name.Substring(0, 50);

                                string temp = sq.name;
                                int p1 = sq.name.IndexOf(")");

                                if (p1 > 0)
                                {
                                    string candId = temp.Substring(1, p1 - 1);
                                    sq.id = Convert.ToInt32(candId);
                                }
                                sq.alias = answers.h_answers[j].results[ri - 1].alias_name;
                                sq.new_order = ri;
                                sq.result_weight = Convert.ToSingle(answers.h_answers[j].results[ri - 1].result_weight);
                                sq.result_count = (int)Convert.ToSingle(answers.h_answers[j].results[ri - 1].result_count);
                                sq.result_percent = Convert.ToInt32(answers.h_answers[j].results[ri - 1].result_percent);

                                sq.party = answers.h_answers[j].results[ri - 1].party;

                                if (sq.name == "Republican" || sq.name == "Democrat")
                                    sq.party = sq.name.Substring(0, 3);

                                if (sq.party.Length > 3)
                                    sq.party = sq.party.Substring(0, 3);

                            }
                            if (ri > 0)
                                sqm.Add(sq);
                        }
                    }

                    //dataGridView1.DataSource = sqm;
                    DeleteDataNew(update, false);

                    DataTable dt = new DataTable();
                    dt = ListToDataTable(sqm);
                    UpdateVADDBNew(dt, fullTick);

                    SendReceipt(update);

                    string s = $"A: {update}  OK";
                    if (this.InvokeRequired)
                        this.Invoke(new ListErr(writeListbox2), s);
                    else
                        listBox2.Items.Add(s);

                }
                else
                {
                    //listBox2.Items.Add($"Data error for A: {update}");
                    log.Error($"Data error for A: {update}");
                    string s = $"Data error for A: {update}";
                    if (this.InvokeRequired)
                        this.Invoke(new ListErr(writeListbox2), s);
                    else
                        listBox2.Items.Add(s);

                }



            }
            catch (Exception ex)
            {
                log.Error($"Error getting Answer Data: {update}  {ex}");
                string s = $"Error getting Answer Data: {update}";
                if (this.InvokeRequired)
                    this.Invoke(new ListErr(writeListbox2), s);
                else
                    listBox2.Items.Add(s);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            int type = 0;
            listBox1.Items.Clear();
            VAPostModel test = new VAPostModel();

            for (int i = 0; i < 6; i++)
            {
                bool rf = false;
                test.request_type = "stack";
                if (i == 0 && cbFSA.Checked)
                {
                    test.stack_type = "fullscreen-answer";
                    rf = true;
                }
                if (i == 1 && cbFSQ.Checked)
                {
                    test.stack_type = "fullscreen-question";
                    rf = true;
                }
                if (i == 2 && cbTKA.Checked)
                {
                    test.stack_type = "ticker-answer";
                    rf = true;
                }
                if (i == 3 && cbTKQ.Checked)
                {
                    test.stack_type = "ticker-question";
                    rf = true;
                }
                if (i == 4 && cbM.Checked)
                {
                    //{ 'request_type': 'map', 'method': 'refresh', 'election_event': '2020_General'}
                    test.request_type = "map";
                    test.stack_type = "";
                    rf = true;
                }
                if (i == 5 && cbMan.Checked)
                {
                    //{ 'request_type': 'map', 'method': 'refresh', 'election_event': '2020_General'}
                    //{ "request_type":"manual","method":"refresh", "election_event":"2020_General"}
                    test.request_type = "manual";
                    test.stack_type = "";
                    rf = true;
                }

                
                if (rf)
                {
                    test.method = "refresh";
                    test.election_event = electionEvent;

                    string JSONrequest = JsonConvert.SerializeObject(test);
                    string result = SendAPIPostRequest(JSONrequest);

                    string s = $"Refreshed {test.stack_type}";
                    if (i == 4)
                        s = $"Refreshed {"map"}";
                    if (this.InvokeRequired)
                        this.Invoke(new ListErr(writeListbox2), s);
                    else
                        listBox2.Items.Add(s);

                    textBox1.Text = result;
                    string jsonData = result;
                }
            }
        }



        private void button17_Click(object sender, EventArgs e)
        {

            if (runStop == "Start")
            {
                runStop = "Stop";
                btnRunStop.Text = runStop;
                Task.Run(() => GetAllNew());
            }
            else
            {
                runStop = "Start";
                btnRunStop.Text = runStop;
            }
        }

        public void GetAllNew()
        {
            int type = 0;
            string typeStr = "";

            while (runStop == "Stop")
            {
                if (this.InvokeRequired)
                    this.Invoke(new ListErr(writeListbox), "!#");
                else
                    listBox1.Items.Add("!#");

                if (this.InvokeRequired)
                    this.Invoke(new TextWrite(writeTextbox), "Checking for data.......");
                else
                    textBox1.Text = "Checking for data.......";

                VAPostModel test = new VAPostModel();

                if (type > 5)
                    type = 0;

                switch (type)
                {
                    case 0:
                        test.stack_type = "fullscreen-question";
                        test.request_type = "stack";
                        typeStr = "fullscreen-question";
                        break;
                    case 1:
                        test.stack_type = "fullscreen-answer";
                        test.request_type = "stack";
                        typeStr = "fullscreen-answer";
                        break;
                    case 2:
                        test.stack_type = "";
                        test.request_type = "manual";
                        typeStr = "manual";
                        break;
                    case 3:
                        test.stack_type = "ticker-question";
                        test.request_type = "stack";
                        break;
                    case 4:
                        test.stack_type = "ticker-answer";
                        test.request_type = "stack";
                        break;
                    case 5:
                        test.stack_type = "";
                        test.request_type = "map";
                        test.id = "";
                        break;

                }

                //test.request_type = "stack";
                test.method = "updates";
                test.election_event = electionEvent;

                string JSONrequest = JsonConvert.SerializeObject(test);
                //{'request_type': 'map', 'method': 'updates', 'election_event': '2020_Primaries'}
                //{"request_type":"map","stack_type":"","method":"updates","id":null,"election_event":"2020_Primaries"}

                string result = SendAPIPostRequest(JSONrequest);

                if (this.InvokeRequired)
                    this.Invoke(new TextWrite(writeTextbox), result);
                else
                    textBox1.Text = result;

                string jsonData = result;

                if (jsonData.Length > 1 && jsonData != "[]")
                {
                    string racesWithData = jsonData.Replace("\"", "");
                    jsonData = racesWithData;
                    racesWithData = jsonData.Replace("[", "");
                    jsonData = racesWithData;
                    racesWithData = jsonData.Replace("]", "");
                    jsonData = racesWithData;
                    //racesWithData = jsonData.Replace(" ", "");


                    // parse the header info
                    string[] strSeparator = new string[] { "," };
                    string[] Races;

                    // this takes the header and splits it into key-value pairs
                    Races = racesWithData.Split(strSeparator, StringSplitOptions.None);
                    int pos;
                    string deleteStr;
                    string updateType = "";

                    // clear updates lists
                    QuestionDeletes.Clear();
                    AnswerDeletes.Clear();
                    ManualDeletes.Clear();
                    MapDeletes.Clear();

                    QuestionUpdates.Clear();
                    AnswerUpdates.Clear();
                    ManualUpdates.Clear();
                    MapUpdates.Clear();


                    for (int i = 0; i < Races.Length; i++)
                    {


                        Races[i] = Races[i].Trim();
                        if (this.InvokeRequired)
                            this.Invoke(new ListErr(writeListbox), Races[i]);
                        else
                            listBox1.Items.Add(Races[i]);

                        log.Info($"Updates[{i}]: {Races[i]}");


                        updateType = "0";
                        //pos = Races[i].IndexOf("|");
                        pos = Races[i].LastIndexOf("|");
                        if (pos >= 0)
                        {
                            deleteStr = Races[i].Substring(pos + 1);
                            Races[i] = Races[i].Substring(0, pos);


                            //if (type % 2 == 0)
                            //{
                            //    if (deleteStr == "delete")
                            //        QuestionDeletes.Add(Races[i]);
                            //    else if (deleteStr == "create")
                            //        QuestionUpdates.Add(Races[i]);
                            //    updateType = "Q";
                            //}
                            //else
                            //{
                            //    if (deleteStr == "delete")
                            //        AnswerDeletes.Add(Races[i]);
                            //    else if (deleteStr == "create")
                            //        AnswerUpdates.Add(Races[i]);
                            //    updateType = "A";
                            //}

                            switch (type)
                            {
                                case 0:
                                    //fullscreen-question
                                    if (deleteStr == "delete")
                                        QuestionDeletes.Add(Races[i]);
                                    else if (deleteStr == "create")
                                        QuestionUpdates.Add(Races[i]);
                                    updateType = "Q";
                                    break;

                                case 1:
                                    //fullscreen-answer
                                    if (deleteStr == "delete")
                                        AnswerDeletes.Add(Races[i]);
                                    else if (deleteStr == "create")
                                        AnswerUpdates.Add(Races[i]);
                                    updateType = "A";
                                    break;

                                case 2:
                                    //fullscreen-manual
                                    if (deleteStr == "delete")
                                        ManualDeletes.Add(Races[i]);
                                    else if (deleteStr == "create")
                                        ManualUpdates.Add(Races[i]);
                                    updateType = "M";
                                    break;

                                case 3:
                                    //ticker-question
                                    if (deleteStr == "delete")
                                        QuestionDeletes.Add(Races[i]);
                                    else if (deleteStr == "create")
                                        QuestionUpdates.Add(Races[i]);
                                    updateType = "Q";
                                    break;

                                case 4:
                                    //ticker-answer
                                    if (deleteStr == "delete")
                                        AnswerDeletes.Add(Races[i]);
                                    else if (deleteStr == "create")
                                        AnswerUpdates.Add(Races[i]);
                                    updateType = "A";
                                    break;

                                //case 5:
                                //    //ticker-manual
                                //    if (deleteStr == "delete")
                                //        ManualDeletes.Add(Races[i]);
                                //    else if (deleteStr == "create")
                                //        ManualUpdates.Add(Races[i]);
                                //    updateType = "M";
                                //    break;

                                case 5:
                                    //Maps
                                    if (deleteStr == "delete")
                                        MapDeletes.Add(Races[i]);
                                    else if (deleteStr == "create")
                                        MapUpdates.Add(Races[i]);
                                    updateType = "X";
                                    break;

                            }

                        }
                        else
                            Races[i] = "";

                    }

                    if (Races.Length > 0)
                    {
                        log.Info($"");
                        log.Info($" {type} {typeStr}: {Races.Length}");
                        if (updateType != "0")
                            ProcessUpdatesNew(updateType);
                    }

                }
                else
                {
                    string stacktype = test.stack_type;
                    if (stacktype == "")
                        stacktype = "manual";
                    if (this.InvokeRequired)
                        this.Invoke(new TextWrite(writeTextbox), $"No new {stacktype}s.");
                    else
                        textBox1.Text = $"No new {test.stack_type}s.";

                }
                //Thread.Sleep(2000);
                Thread.Sleep(100);
                type++;

            }
        }

        public void ProcessUpdatesNew(string updateType)
        {
            int n = 0;
            log.Info($"{updateType}");
            if (updateType == "Q")
            {
                // process all questions
                n = QuestionUpdates.Count;
                for (int i = 0; i < n; i++)
                    ProcessUpdate(QuestionUpdates[i]);

                // process all question deletes
                n = QuestionDeletes.Count;
                for (int i = 0; i < n; i++)
                    DeleteDataNew(QuestionDeletes[i], true);
            }
            else if (updateType == "A")
            {
                // process all answers
                n = AnswerUpdates.Count;
                for (int i = 0; i < n; i++)
                    ProcessUpdate(AnswerUpdates[i]);

                // process all answer deletes
                n = AnswerDeletes.Count;
                for (int i = 0; i < n; i++)
                    DeleteDataNew(AnswerDeletes[i], true);
            }
            else if (updateType == "M")
            {
                // process all manual
                n = ManualUpdates.Count;
                for (int i = 0; i < n; i++)
                    ProcessUpdate(ManualUpdates[i]);

                // process all manual deletes
                n = ManualDeletes.Count;
                for (int i = 0; i < n; i++)
                    DeleteDataNew(ManualDeletes[i], true);
            }
            else if (updateType == "X")
            {
                // process all maps
                n = MapUpdates.Count;
                for (int i = 0; i < n; i++)
                    ProcessUpdate(MapUpdates[i]);

                // process all map deletes
                n = MapDeletes.Count;
                for (int i = 0; i < n; i++)
                    DeleteDataNew(MapDeletes[i], true);
            }


        }

        public void writeListbox(string s)
        {
            if (s == "!#")
                listBox1.Items.Clear();
            else
                listBox1.Items.Add(s);
        }

        public void writeTextbox(string s)
        {
            textBox1.Text = s;
        }

        public void writeListbox2(string s)
        {
            string tn = DateTime.Now.ToString("hh:mm:ss");
            if (s == "!#")
                listBox2.Items.Clear();
            else
                listBox2.Items.Add($"{tn}  {s}");
        }

        public void DeleteDataNew(string update, bool sendReceipt)
        {

            try
            {
                string delCmd;

                string tblName = "FE_VoterAnalysisData_FS_New";
                string[] strSeparator = new string[] { ":" };
                string[] Races;

                // this takes the header and splits it into key-value pairs
                Races = update.Split(strSeparator, StringSplitOptions.None);
                if (Races.Length > 1)
                {
                    string method = Races[3];
                    if (method != "map")
                    {
                        strSeparator = new string[] { "-" };
                        string[] qa;
                        qa = method.Split(strSeparator, StringSplitOptions.None);
                        string fullTick = qa[0];
                        string faq = qa[1];


                        if (fullTick == "ticker")
                            tblName = "FE_VoterAnalysisData_TKR_New";
                        else
                            tblName = "FE_VoterAnalysisData_FS_New";

                        delCmd = $"DELETE FROM {tblName} WHERE VA_Data_Id = '{update}'";
                        IssueSQLCmd(delCmd);
                    }
                    else
                    {
                        tblName = "FE_VoterAnalysisData_Map_New";
                        delCmd = $"DELETE FROM {tblName} WHERE VA_Data_Id = '{update}'";
                        IssueSQLCmd(delCmd);

                        tblName = "FE_VoterAnalysis_Map_Defs_New";
                        delCmd = $"DELETE FROM {tblName} WHERE VA_Data_Id = '{update}'";
                        IssueSQLCmd(delCmd);
                    }

                }
                else
                {
                    tblName = "FE_VoterAnalysisData_MAN_New";
                    delCmd = $"DELETE FROM {tblName} WHERE VA_Data_Id = '{update}'";
                    IssueSQLCmd(delCmd);
                }


                if (sendReceipt)
                {
                    SendReceipt(update);
                    log.Info($"  SendReceipt:    {update}");

                    string s = $"Deleted: {update}";
                    if (this.InvokeRequired)
                        this.Invoke(new ListErr(writeListbox2), s);
                    else
                        listBox2.Items.Add(s);
                }

            }
            catch (Exception ex)
            {
                log.Error($"Delete Error: {update} {ex}");

            }

        }

        private void rbQ_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnAPI_Click(object sender, EventArgs e)
        {
            prodMode = !prodMode;
            if (prodMode)
            {
                baseUrl = Properties.Settings.Default.URL_Prod;
                apiKey = Properties.Settings.Default.api_key_Prod;
                dbConn = Properties.Settings.Default.dbConn_Prod;
                btnAPI.Text = "Switch to Stage API";
                lblBaseUrl.Text = $"URL: Prod:   {baseUrl}";
            }
            else
            {
                baseUrl = Properties.Settings.Default.URL_Stg;
                apiKey = Properties.Settings.Default.api_key_Stg;
                dbConn = Properties.Settings.Default.dbConn_Prod;
                btnAPI.Text = "Switch to Prod API";
                lblBaseUrl.Text = $"URL: {useURL}:   {baseUrl}";
            }
        }

 
        private void btnRunStop_Click(object sender, EventArgs e)
        {
            if (runStop == "Start")
            {
                runStop = "Stop";
                btnRunStop.Text = runStop;
                Task.Run(() => GetAllNew());
            }
            else
            {
                runStop = "Start";
                btnRunStop.Text = runStop;
            }
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            string delCmd;
            string tableName;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        if (cbDelFS.Checked)
                        {
                            tableName = "FE_VoterAnalysisData_FS_New";
                            delCmd = $"DELETE FROM {tableName}";
                            IssueSQLCmd(delCmd);
                        }
                        break;

                    case 1:
                        if (cbDelTKR.Checked)
                        {
                            tableName = "FE_VoterAnalysisData_TKR_New";
                            delCmd = $"DELETE FROM {tableName}";
                            IssueSQLCmd(delCmd);
                        }
                        break;

                    case 2:
                        if (cbDelMan.Checked)
                        {
                            tableName = "FE_VoterAnalysisData_MAN_New";
                            delCmd = $"DELETE FROM {tableName}";
                            IssueSQLCmd(delCmd);
                        }
                        break;

                    case 3:
                        if (cbDelMaps.Checked)
                        {
                            tableName = "FE_VoterAnalysisData_Map_New";
                            delCmd = $"DELETE FROM {tableName}";
                            IssueSQLCmd(delCmd);

                            tableName = "FE_VoterAnalysis_Map_Defs_New";
                            delCmd = $"DELETE FROM {tableName}";
                            IssueSQLCmd(delCmd);
                        }
                        break;

                }
            }
        }

        private void button17_Click_1(object sender, EventArgs e)
        {
            string Race = tbRace.Text;
            int pos = Race.LastIndexOf("|");
            if (pos >= 0)
            {
                string deleteStr = Race.Substring(pos + 1);
                Race = Race.Substring(0, pos);
                if (deleteStr == "delete")
                    DeleteDataNew(Race, true);
                else
                    ProcessUpdate(Race);
            }
        }
    }

}



