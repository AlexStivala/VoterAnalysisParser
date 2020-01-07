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
    public partial class frmMain : Form , IAppender
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

        public List<string> QuestionDeletes = new List<string>();
        public List<string> AnswerDeletes = new List<string>();
        public List<string> MapDeletes = new List<string>();

        public List<string> msgs = new List<string>();


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

            log.Info($" ********** VoterAnalysisParser Started **********");

            bool prodMode = Properties.Settings.Default.ProdMode;
            if (prodMode)
            {
                baseUrl = Properties.Settings.Default.URL_Prod;
                apiKey = Properties.Settings.Default.api_key_Prod;
                dbConn = Properties.Settings.Default.dbConn_Prod;
            }
            else
            {
                baseUrl = Properties.Settings.Default.URL_QA;
                apiKey = Properties.Settings.Default.api_key_QA;
                dbConn = Properties.Settings.Default.dbConn_QA;
            }


            var builder = new SqlConnectionStringBuilder(dbConn);
            var dataSource = builder.DataSource;
            var initCat = builder.InitialCatalog;
            var user = builder.UserID;
            var pw = builder.Password;

            //lblBaseUrl.Text = baseUrl;
            lblBaseUrl.Text = newbaseUrl;
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
                catch(Exception ex)
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
                //log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);
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
                //log.Error("GetDBData Exception occurred: " + ex.Message);
                //log.Debug("GetDBData Exception occurred", ex);
            }

            return dataTable;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
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

                string racesWithData = jsonData.Replace("\"","");
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
            foreach(RaceListModel race in raceList)
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
            string url = newbaseUrl;

            try
            {
                HttpClient client = new HttpClient(); 
                
                var content = new StringContent(requestJson.ToString(), Encoding.UTF8, "application/json");

                requestMessage.Headers.Add("x-api-key", newapiKey);
                requestMessage.Content = new StringContent(requestJson.ToString(), Encoding.UTF8, "application/json");
                
                client.DefaultRequestHeaders.Add("x-api-key", newapiKey);


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
                //log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);
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
                                textBox1.Text = "Update Map Data - SQL Command Exception occurred: " + ex.Message;
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
                //log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);
                textBox1.Text = "Update Map Data - SQL Command Exception occurred: " + ex.Message;

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
                                            sq.party = sq.name.Substring(0,3);


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
                lblDataMode.Text  = $"Timer cnt: {cnt}";
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

            test.request_type = "stack";
            //test.stack_type = "fullscreen-answer";
            test.stack_type = "fullscreen-question";
            test.method = "updates";
            test.election_event = "2018_Midterms";

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
                    if (type == 0)
                    {
                        if (deleteStr == "delete")
                            QuestionDeletes.Add(Races[i]);
                        else if (deleteStr == "create")
                            QuestionUpdates.Add(Races[i]);
                    }
                    else if (type == 1)
                    {
                        if (deleteStr == "delete")
                            AnswerDeletes.Add(Races[i]);
                        else if (deleteStr == "create")
                            AnswerUpdates.Add(Races[i]);
                    }
                }
            }


        }

        private void button15_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < QuestionUpdates.Count; i++)
            {
                ProcessUpdate(QuestionUpdates[i]);
            }
            
        }

        public void ProcessUpdate(string update)
        {

            tbRace.Text = update;
            // parse the header info
            string[] strSeparator = new string[] { ":" };
            string[] Races;

            // this takes the header and splits it into key-value pairs
            Races = update.Split(strSeparator, StringSplitOptions.None);
            string method = Races[3];

            strSeparator = new string[] { "-" };
            string[] qa;
            qa = method.Split(strSeparator, StringSplitOptions.None);
            string fullTick = qa[0];
            string faq = qa[1];

            VAPostModel test = new VAPostModel();

            test.request_type = "stack";
            test.stack_type = method;
            test.method = "data";
            test.id = update;
            test.election_event = "2018_Midterms";

            string JSONrequest = JsonConvert.SerializeObject(test);

            string result = SendAPIPostRequest(JSONrequest);
            string jsonData = result;
            textBox1.Text = result;

            string err = "stackTrace";
            int pos = jsonData.IndexOf(err);
            if (pos >= 0)
            {
                // Error - send receipt
                SendReceipt(update); 
            }
            else
            {
                if (faq == "answer")
                    ProcessAnswerDataNew(jsonData, update, fullTick);
                else if (faq == "question")
                    ProcessQuestionDataNew(jsonData, update, fullTick);
            }
        }

        public void SendReceipt(string update)
        {
            // parse the header info
            string[] strSeparator = new string[] { ":" };
            string[] Races;

            // this takes the header and splits it into key-value pairs
            Races = update.Split(strSeparator, StringSplitOptions.None);
            string method = Races[3];

            VAPostModel test = new VAPostModel();

            test.request_type = "stack";
            test.stack_type = method;
            test.method = "receipt";
            test.id = update;
            test.election_event = "2018_Midterms";

            string JSONrequest = JsonConvert.SerializeObject(test);

            string result = SendAPIPostRequest(JSONrequest);
            string jsonData = result;
            textBox1.Text = result;
            
        }

        public void ProcessQuestionDataNew(string json, string update, string fullTick)
        {
            try
            {

                //string json = textBox1.Text;

                VAQuestionModelNew questions = new VAQuestionModelNew();

                string err = "stackTrace";
                int pos = json.IndexOf(err);
                if (pos == -1)
                {

                    questions = JsonConvert.DeserializeObject<VAQuestionModelNew>(json);


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


                                    sq.VA_Data_Id = update;
                                    sq.r_type = "Q";
                                    sq.q_order = Convert.ToInt32(questions.question_order);
                                    sq.questionId = questions.questionId;
                                    sq.state = questions.state;
                                    sq.race_id = questions.race_id;
                                    sq.question = questions.question;
                                    if (questions.race_type == "all")
                                        sq.ofc = "A";
                                    else
                                        sq.ofc = questions.race_type;
                                    sq.qcode = questions.qcode;
                                    sq.filter = questions.filter;
                                    sq.sample_size = Convert.ToInt32(questions.sample_size);
                                    sq.total_weight = Convert.ToSingle(questions.total_weight);
                                    sq.def = Convert.ToBoolean(questions.def);
                                    sq.preface = questions.preface;
                                    sq.pk = questions.header; // change

                                    sq.variable_weight = Convert.ToSingle(questions.h_answers[j].variable_weight);
                                    sq.variable_count = Convert.ToInt32(questions.h_answers[j].variable_count);
                                    sq.variable_percent = Convert.ToInt32(questions.h_answers[j].variable_percent);
                                    sq.a_order = Convert.ToInt32(questions.h_answers[j].original_order); // change
                                    sq.name = questions.h_answers[j].original_name; //

                                    //sq.answer = questions.h_answers[j].answer;
                                    //sq.answer_id = Convert.ToInt32(questions.h_answers[j].answer_id);
                                    sq.id = 0;
                                    sq.result_count = 0;
                                    sq.result_percent = 0;
                                    sq.result_weight = 0;
                                    sq.party = string.Empty;
                                    sq.stateId = GetStateID(questions.state);
                                    sq.update_Time = questions.update_Time;
                                    sq.f_update_time = fTime;


                                    sqm.Add(sq);
                                }
                            }
                        }
                    }


                    dataGridView1.DataSource = sqm;
                    DataTable dt = new DataTable();
                    dt = ListToDataTable<VASQLDataModel>(sqm);
                    UpdateVADDBNew(dt, fullTick);

                }
                else
                {
                    listBox2.Items.Add($"Data error for Q: {update}");
                    log.Error($"Data error for Q: {update}");
                }

                SendReceipt(update);
                
                
            }
            catch (Exception ex)
            {
                log.Error($"Error processing Question Data: {update}  {ex}");
            }
        }

        public void UpdateVADDBNew(DataTable dt, string fullTick)
        {
            string spName = "spUpdate_VoterAnalysisData_Ticker";

            if (fullTick == "fullscreen")
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
                //log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);
                textBox1.Text = "UpdateData - SQL Command Exception occurred: " + ex.Message;
            }

        }

        public void ProcessAnswerDataNew(string json, string update, string fullTick)
        {
            try
            {

                //string json = textBox1.Text;

                VAAnswerModelNew answers = new VAAnswerModelNew();

                string err = "stackTrace";
                int pos = json.IndexOf(err);
                if (pos == -1)
                {

                    answers = JsonConvert.DeserializeObject<VAAnswerModelNew>(json);

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

                                    sq.VA_Data_Id = update;
                                    sq.r_type = "A";
                                    sq.q_order = Convert.ToInt32(answers.question_order);
                                    sq.questionId = answers.questionId;
                                    sq.state = answers.state;
                                    sq.race_id = answers.race_id;
                                    sq.question = answers.question;
                                    if (answers.race_type == "all")
                                        sq.ofc = "A";
                                    else
                                        sq.ofc = answers.race_type;
                                    sq.qcode = answers.qcode;
                                    sq.filter = answers.filter;
                                    sq.sample_size = Convert.ToInt32(answers.sample_size);
                                    sq.total_weight = Convert.ToSingle(answers.total_weight);
                                    sq.def = Convert.ToBoolean(answers.def);
                                    sq.preface = answers.preface;
                                    sq.pk = answers.header; // change


                                    sq.variable_weight = Convert.ToSingle(answers.h_answers[j].variable_weight);
                                    sq.variable_count = Convert.ToInt32(answers.h_answers[j].variable_count);
                                    sq.variable_percent = Convert.ToInt32(answers.h_answers[j].variable_percent);
                                    sq.a_order = Convert.ToInt32(answers.h_answers[j].original_order);// change

                                    if (int.TryParse(answers.h_answers[j].results[ri - 1].order, out parsedResult))
                                        sq.result_order = Convert.ToInt32(answers.h_answers[j].results[ri - 1].order);
                                    else
                                        sq.result_order = cnt;




                                    sq.update_Time = answers.update_Time;
                                    //sq.f_update_time = DateTime.Now;
                                    sq.f_update_time = fTime;
                                    sq.answer = answers.h_answers[j].answer;
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
                                        sq.result_weight = Convert.ToSingle(answers.h_answers[j].results[ri - 1].result_weight);
                                        sq.result_count = Convert.ToInt32(answers.h_answers[j].results[ri - 1].result_count);
                                        sq.result_percent = Convert.ToInt32(answers.h_answers[j].results[ri - 1].result_percent);
                                        sq.name = answers.h_answers[j].results[ri - 1].original_name;
                                        sq.id = Convert.ToInt32(answers.h_answers[j].results[ri - 1].id);
                                        sq.party = answers.h_answers[j].results[ri - 1].party;

                                        if (sq.name == "Republican" || sq.name == "Democrat")
                                            sq.party = sq.name.Substring(0, 3);

                                        if (sq.party.Length > 3)
                                            sq.party = sq.party.Substring(0, 3);

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
                    UpdateVADDBNew(dt, fullTick);
                }
                else
                {
                    listBox2.Items.Add($"Data error for A: {update}");
                    log.Error($"Data error for A: {update}");

                }

                string url;

                //if (dataType == 1)
                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=fullscreen_answer&call_type=receipt&id={race}";
                //else
                //url = $"https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=stack&stack_type=ticker_answer&call_type=receipt&id={race}";

                int nn = dataType * 2 + 1;
                url = $"{baseUrl}{stacktype}{stackTypes[nn]}{calltype}{callTypes[2]}{update}";

                string jsonData = SendAPIRequest(url);



            }
            catch (Exception ex)
            {
                log.Error($"Error getting Answer Data: {update}  {ex}");

            }



        }



    }

}



