using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueTesting;
using MultiQueueModels;

namespace MultiQueueSimulation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /* SimulationSystem system = new SimulationSystem();
             string result = TestingManager.Test(system, Constants.FileNames.TestCase1);
             MessageBox.Show(result);*/
           // Random rd = new Random();
           // MessageBox.Show(rd.Next(1,3).ToString());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HomePage());
           
        }
    }
}
