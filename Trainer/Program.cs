using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Trainer
{
    /// <summary>
    /// The executable expects command line parameters in this format:
    /// Trainer.exe --name1 value1 --name2 value2 --name3 value3
    /// Trainer.exe --class MyClass --method MyMethod
    /// To keep this executable extensible, the arguments --class and --method are neccessary
    /// The method MyMethod of the MyClass will be executed
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureTracing();
            Trace.WriteLine("Displaying command line arguments:");
            foreach (string arg in args) Trace.WriteLine($"    {arg}");
            Dictionary<string, string> argsNameValuePairs = CreateDictionaryFromCommandlineArguments(args);
            if (argsNameValuePairs.ContainsKey("class") == false)
            {
                DisplayHelp();
                return;
            }
            if (argsNameValuePairs.ContainsKey("method") == false)
            {
                DisplayHelp();
                return;
            }
            string classname = argsNameValuePairs["class"];
            string methodname = argsNameValuePairs["method"];
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            Type tClassWithMethodFromCmdLine = thisAssembly.GetTypes().FirstOrDefault(t => t.Name.ToLower() == classname.ToLower());
            if (tClassWithMethodFromCmdLine == null)
            {
                throw new Exception($"The type:{classname} was not found");
            }
            MethodInfo methodFromCmdLine =
                tClassWithMethodFromCmdLine.GetMethod(methodname, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (methodFromCmdLine == null)
            {
                throw new Exception($"The method 'Run' was not found in the class:{methodname}");
            }
            var oTrainer = Activator.CreateInstance(tClassWithMethodFromCmdLine);
            methodFromCmdLine.Invoke(oTrainer, new object[] { argsNameValuePairs });
        }

        private static void DisplayHelp()
        {
            Trace.WriteLine("trainer.exe --class <your class>  --method <name of method in class> --arg1 --value1 --arg2 --value2");
        }

        private static Dictionary<string, string> CreateDictionaryFromCommandlineArguments(string[] args)
        {
            Dictionary<string, string> cmdparams = new Dictionary<string, string>();
            for (int argindex = 0; argindex < args.Length; argindex++)
            {
                string argName = args[argindex];
                if (argName.StartsWith("--"))
                {
                    string key = argName.ToLower();
                    while (key.StartsWith("-")) { key = key.Remove(0, 1); }
                    string value = args[argindex + 1];
                    cmdparams.Add(key, value);
                    argindex++;
                }
            }
            return cmdparams;
        }
        /// <summary>
        /// The trace logs will be created in a sub-folder Logs and will be time stamped for easy identification
        /// </summary>
        private static void ConfigureTracing()
        {
            string logFile = null;
            System.Diagnostics.Trace.AutoFlush = true;
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());
            string logfilename = "Trainer.exe." + DateTime.Now.ToString("dd-MMM-yyyy-HH-mm-ss") + ".log";
            string _executablefolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string folderLocal = System.IO.Path.Combine(_executablefolder, "Logs");
            if (System.IO.Directory.Exists(folderLocal) == false)
            {
                System.IO.Directory.CreateDirectory(folderLocal);
            }

            logFile = System.IO.Path.Combine(folderLocal, logfilename);
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(logFile));
            Trace.WriteLine("Tracing configured.");
            Trace.WriteLine($"Trace log file={logFile}");
        }
    }
}
