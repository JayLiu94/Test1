using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tracer
{
    public class CommandHandle
    {
        public static void HandleCommandLineArguments(string[] args)
        {
           // string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                string command = args[1];

                switch (command.ToLower())
                {
                    case "-h":
                        ShowHelp();
                        break;

                    case "-save":
                        SaveData();
                        break;

                    default:
                        MessageBox.Show($"Unknown command: {command}");
                        break;
                }
            }
        }




        private static void ShowHelp()
        {
            Console.WriteLine("Help Information:\nUsage: Tracer.exe -h");
            //MessageBox.Show("Help Information:\nUsage: trace.exe -h");
        }

        private static void SaveData()
        {
            MessageBox.Show("Saving Data...");
        }




    }
}
