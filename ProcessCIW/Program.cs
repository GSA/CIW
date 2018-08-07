using ProcessCIW.Utilities;
using System;
using System.Configuration;
using System.Diagnostics;
using ProcessCIW.Process;
using ProcessCIW.Validation;
using MySql.Data.MySqlClient;
using ProcessCIW.Interface;

namespace ProcessCIW
{
    class Program
    {

        private static readonly ILogTool log;
        private static Stopwatch stopWatch;
        static Utilities.Utilities U;
        static ValidateCiw vc;
        static readonly MySqlConnection conn;
        static DataAccess da;
        static FileTool ft;
        static CiwEmails ce;
        static XmlTool xt;
        static ProcessDocuments pd;
        static DecryptFile df;
        static DeleteTool dt;
        private readonly static ProcessFiles pf;

        static Program()
        {
            log = LogTool.GetInstance();
            stopWatch = new Stopwatch();
            U = new Utilities.Utilities();
            vc = new ValidateCiw(da, U, log);
            conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString());
            da = DataAccess.GetInstance(conn, U, log);
            ft = new FileTool(log);
            ce = new CiwEmails(da, log);
            xt = new XmlTool(U, ce, /*da,*/ log);
            pd = new ProcessDocuments(da, ft, xt, U, dt, log);
            df = new DecryptFile(U, log);
            dt = new DeleteTool(log);
            pf = new ProcessFiles(da, /*U,*/ pd, vc, df, dt, log);
        }

        protected Program() { }

        static void Main(string[] args)
        {
            //Define unhandled exception delegate
            AppDomain.CurrentDomain.UnhandledException += UnhandledException.UnhandledExceptionTrap;

            //used during logging
            stopWatch.Start();

            log.Info("Application Started");

            //All action takes place here
            pf.PreProcessFiles();

            log.Info(string.Format("Processed Adjudications in {0} milliseconds", stopWatch.ElapsedMilliseconds));

            stopWatch.Stop();

            log.Info("Application Done");

            Console.WriteLine("Done! " + stopWatch.ElapsedMilliseconds);

            //End of program
        }
    }
}