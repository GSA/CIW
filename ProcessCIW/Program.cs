using ProcessCIW.Utilities;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using ProcessCIW.Process;
namespace ProcessCIW
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Stopwatch stopWatch = new Stopwatch();
        private readonly static ProcessFiles pf = new ProcessFiles();

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