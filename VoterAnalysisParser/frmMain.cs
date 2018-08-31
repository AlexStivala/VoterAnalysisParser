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


namespace VoterAnalysisParser
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
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
                string json = textBox1.Text;

                //var json = File.ReadAllText(@"C:\X\VoterAnalysisParser\VoterAnalysisParser\test1.json");
                var array = JsonConvert.DeserializeObject(json) as JArray;
                var models = (array[1] as JArray).ToObject<List<VADataModel>>();
                

            }
            catch (Exception ex)
            {
                var info = ex.Message;
            }

        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            string jsonData = GetVAData();
            textBox1.Text = jsonData;
        }

        private static string GetVAData()
        {
            var jsonResponse = "";
            string WEBSERVICE_URL = "https://xa1faa0ebb.execute-api.us-east-1.amazonaws.com/prod/?page_type=xtab&race=IL-G-91912&list_results=true2&list_answers=true";
            try
            {
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
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return jsonResponse;

        }
    }
}
