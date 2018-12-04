using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VoterAnalysisParser
{
    static class Program
    {
        #region Logger instantiation - uses reflection to get module name; needs to be done for each class
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Need to call this in the main loop to init the logger
            log4net.Config.XmlConfigurator.Configure();

            var mainForm = new frmMain();

            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(mainForm);

            Application.Run(new frmMain());
        }
    }
}
