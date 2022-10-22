using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueModels;

using MultiQueueTesting;


namespace MultiQueueSimulation
{
    public partial class SimulatinTableForm : Form
    {
        private  SimulationSystem simSys;
        public Random rd;
        public SimulatinTableForm()
        {
            InitializeComponent();
        }

        public SimulatinTableForm(SimulationSystem simSys)
        {
            InitializeComponent();
            this.simSys = simSys;
            rd = new Random();
        }
        private void simTable_Paint(object sender, PaintEventArgs e)
        {

        }


        private void SimulatinTableForm_Load(object sender, EventArgs e)
        {
            int number = simSys.StoppingNumber;
            SimulationCase user = new SimulationCase();


            if (simSys.SelectionMethod == Enums.SelectionMethod.HighestPriority)
            {
                methodSelection(number,user,Enums.SelectionMethod.HighestPriority);
                
            }
            else if(simSys.SelectionMethod==Enums.SelectionMethod.Random)
            {
                methodSelection(number, user, Enums.SelectionMethod.Random);
            }

            string result = TestingManager.Test(simSys, Constants.FileNames.TestCase1);
            MessageBox.Show(result); 
        }

        public void methodSelection(int number,SimulationCase user,Enums.SelectionMethod method)
        {
           

            user.CustomerNumber = 1;
            user.RandomInterArrival = 1;
            user.InterArrival = 0;
            user.ArrivalTime = 0;
            user.RandomService = rd.Next(1, 101);
            user.ServiceTime = getServiceTime(user.RandomService, simSys.Servers[0]);
            user.StartTime = 0;
            user.EndTime = user.StartTime + user.ServiceTime;
            simSys.Servers[0].FinishTime = user.EndTime;
            user.AssignedServer = simSys.Servers[0];
            user.TimeInQueue = 0;

            simSys.SimulationTable.Add(user);

            int row = 1;
            makeRow(user, row);
            row++;

            for (int i = 1; i < number; i++)
            {


                user = new SimulationCase();
                user.CustomerNumber = i + 1; //1
                user.RandomInterArrival = rd.Next(1, 101);//2
                user.InterArrival = getInterArrivaal(user.RandomInterArrival);//3
                user.ArrivalTime = simSys.SimulationTable[i - 1].ArrivalTime + user.InterArrival;//4

                List<int> server = 
                    method == Enums.SelectionMethod.HighestPriority? getServerOfHightPriorty(simSys.Servers, user.ArrivalTime)
                    :method== Enums.SelectionMethod.Random? getServerOfRandom(simSys.Servers,user.ArrivalTime): null;

                int timeToAvailbe = server[0];
                int serverID = server[1];

                user.RandomService = rd.Next(1, 101); //5
                user.StartTime = Math.Max(user.ArrivalTime, timeToAvailbe);//6
                user.ServiceTime = getServiceTime(user.RandomService, simSys.Servers[serverID]); //7
                user.EndTime = user.StartTime + user.ServiceTime;//8
                simSys.Servers[serverID].FinishTime = user.EndTime;

                user.AssignedServer = simSys.Servers[serverID]; //9
                user.TimeInQueue = user.StartTime - user.ArrivalTime;//10
                simSys.SimulationTable.Add(user);

                makeRow(user, row);
                row++;




            }
        }

        public List<int> getServerOfRandom(List<Server> servers, int timeArrival)
        {
            List<int> output = new List<int>();
            int minTime = 30000000;
            int index = -1;
            output.Add(minTime);
            output.Add(index);

            for (int i = 0; i < servers.Count; i++)
            {

                if (output[0] > servers[i].FinishTime)
                {
                    output[0] = servers[i].FinishTime;
                    output[1] = i;

                }
            }


            return output;
        }

        public List<int> getServerOfHightPriorty(List<Server> servers,int timeArrival)
        {
            List<int> output = new List<int>();
            
            int  minTime = 30000000;
            int index=-1;
            output.Add(minTime);
            output.Add(index);


            for(int i=0; i<servers.Count; i++)
            {
                if (timeArrival >= servers[i].FinishTime)
                {
                    output[0] = servers[i].FinishTime;
                    output[1] = i;
                   
                    return output;

                }
              
            }

            for (int i = 0; i < servers.Count; i++)
            {
               
                if (output[0] > servers[i].FinishTime)
                {
                    output[0] = servers[i].FinishTime;
                    output[1] = i;

                }
            }


            return output;
        }

        public void makeRow(SimulationCase user, int row)
        {
            SimTable.Controls.Add(new Label() { Text = user.CustomerNumber.ToString() }, 0, row);
            SimTable.Controls.Add(new Label() { Text = user.RandomInterArrival.ToString() }, 1, row);
            SimTable.Controls.Add(new Label() { Text = user.InterArrival.ToString() }, 2, row);
            SimTable.Controls.Add(new Label() { Text = user.ArrivalTime.ToString() }, 3, row);
            SimTable.Controls.Add(new Label() { Text = user.RandomService.ToString() }, 4, row);
            SimTable.Controls.Add(new Label() { Text = user.StartTime.ToString() }, 5, row);
            SimTable.Controls.Add(new Label() { Text = user.ServiceTime.ToString() }, 6, row);
            SimTable.Controls.Add(new Label() { Text = user.EndTime.ToString() }, 7, row);
            SimTable.Controls.Add(new Label() { Text = user.AssignedServer.ID.ToString() }, 8, row);
            SimTable.Controls.Add(new Label() { Text = user.TimeInQueue.ToString() }, 9, row);
            
        }

        public int getRandomNumber(Random rd)
        {
            return rd.Next(1, 101);
        }

        public int getInterArrivaal(int number)
        {
            TimeDistribution time = new TimeDistribution();
            for(int i = 0; i < simSys.InterarrivalDistribution.Count; i++)
            {
                time = simSys.InterarrivalDistribution[i];
                if (time.MinRange <= number && time.MaxRange >= number)
                    return time.Time;
            }

            return -1;
            
        }
        public int getServiceTime(int number,Server server)
        {
            TimeDistribution time = new TimeDistribution();
            for (int i = 0; i < server.TimeDistribution.Count; i++)
            {
                time = server.TimeDistribution[i];
                if (time.MinRange <= number && time.MaxRange >= number)
                {
                    
                    return time.Time;

                }
                    
            }

            return -1;

        }
  
    }
}
