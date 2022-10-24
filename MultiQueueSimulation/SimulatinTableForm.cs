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
        private int testCaseValue;
        public SimulatinTableForm()
        {
            InitializeComponent();
        }

        public SimulatinTableForm(SimulationSystem simSys , int testCaseValue)
        {
            InitializeComponent();
            this.simSys = simSys;
            this.testCaseValue = testCaseValue;
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
                methodSelection(number, user, Enums.SelectionMethod.HighestPriority);

            }
            else if (simSys.SelectionMethod == Enums.SelectionMethod.Random)
            {
                methodSelection(number, user, Enums.SelectionMethod.Random);
            }
            else {
                methodSelection(number, user, Enums.SelectionMethod.LeastUtilization);
            }
            string result = "";
            switch (testCaseValue) {
                case 1:
                result = TestingManager.Test(simSys, Constants.FileNames.TestCase1);
                break;
                case 2:
                result = TestingManager.Test(simSys, Constants.FileNames.TestCase2);
                break;
                case 3:
                result = TestingManager.Test(simSys, Constants.FileNames.TestCase3);
                break;
            }
            MessageBox.Show(result);
        }

        public void methodSelection(int number, SimulationCase user, Enums.SelectionMethod method)
        {

            int maxFinish = 0;
            user.CustomerNumber = 1;
            user.RandomInterArrival = 1;
            user.InterArrival = 0;
            user.ArrivalTime = 0;
            user.RandomService = rd.Next(1, 100);
            user.ServiceTime = getServiceTime(user.RandomService, simSys.Servers[0]);
            user.StartTime = 0;
            user.EndTime = user.StartTime + user.ServiceTime;
            simSys.Servers[0].FinishTime = user.EndTime;
            simSys.Servers[0].TotalWorkingTime += user.ServiceTime;
            maxFinish = Math.Max(user.EndTime, maxFinish);
            user.AssignedServer = simSys.Servers[0];
            user.TimeInQueue = 0;
            
            decimal totalServiceTime = user.ServiceTime;
            simSys.SimulationTable.Add(user);

            int row = 1;
            makeRow(user, row);
            row++;
            int numberOfUsersWaited = 0;
            int totalNumberOfCustomers = 1;
            int timeInQueue = 0;
            List<int> customersForEachServer = new List<int>();
            for (int i = 0; i < simSys.Servers.Count; i++)
            {
                customersForEachServer.Add(0);
            }
            customersForEachServer[0]++;
            for (int i = 1; i < number; i++)
            {

                user = new SimulationCase();
                user.CustomerNumber = i + 1; //1
                user.RandomInterArrival = rd.Next(1, 100);//2
                user.InterArrival = getInterArrivaal(user.RandomInterArrival);//3
                user.ArrivalTime = simSys.SimulationTable[i - 1].ArrivalTime + user.InterArrival;//4
                List<int> server =
                    method == Enums.SelectionMethod.HighestPriority ? getServerOfHightPriorty(simSys.Servers, user.ArrivalTime)
                    : method == Enums.SelectionMethod.Random ? getServerOfRandom(simSys.Servers, user.ArrivalTime) : getServerLeastUtilization(simSys.Servers , maxFinish);

                int timeToAvailbe = server[0];
                int serverID = server[1];


                user.RandomService = rd.Next(1, 100); //5
                user.StartTime = Math.Max(user.ArrivalTime, timeToAvailbe);//6
                user.ServiceTime = getServiceTime(user.RandomService, simSys.Servers[serverID]); //7
                user.EndTime = user.StartTime + user.ServiceTime;//8
                simSys.Servers[serverID].FinishTime = user.EndTime;
                user.AssignedServer = simSys.Servers[serverID]; //9

                user.TimeInQueue = user.StartTime - user.ArrivalTime;//10
                simSys.Servers[serverID].TotalWorkingTime += user.ServiceTime;
                maxFinish = Math.Max(user.EndTime, maxFinish);
                simSys.SimulationTable.Add(user);
                if (user.TimeInQueue > 0)
                {
                    timeInQueue += user.TimeInQueue;
                    numberOfUsersWaited++;
                }
                makeRow(user, row);
                row++;
                totalServiceTime += user.ServiceTime;
                customersForEachServer[serverID]++;
                totalNumberOfCustomers++;

            }
            for (int i = 0; i < simSys.Servers.Count; i++)
            {
                simSys.Servers[i].IdleProbability = (maxFinish - simSys.Servers[i].TotalWorkingTime);
                simSys.Servers[i].IdleProbability /= maxFinish;
                simSys.Servers[i].Utilization = simSys.Servers[i].TotalWorkingTime;
                simSys.Servers[i].Utilization /= maxFinish;
                simSys.Servers[i].AverageServiceTime = simSys.Servers[i].TotalWorkingTime;
                simSys.Servers[i].AverageServiceTime /= Math.Max(customersForEachServer[i], 1);
          
            }
            simSys.PerformanceMeasures.AverageWaitingTime = timeInQueue;  
            simSys.PerformanceMeasures.AverageWaitingTime /= totalNumberOfCustomers;
            simSys.PerformanceMeasures.WaitingProbability = numberOfUsersWaited;
            simSys.PerformanceMeasures.WaitingProbability /= totalNumberOfCustomers;

            //if there's no users waiting in queue then we don't need queue or the length will be zero
            //else then get the max Length for the queue :)
            simSys.PerformanceMeasures.MaxQueueLength = timeInQueue == 0 ? 0 : getMaxQueueLength(simSys);
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


        //Bonus
        public List<int> getServerLeastUtilization(List<Server> servers,decimal totalServiceTime) {
            List<int> output = new List<int>();

            decimal leastUtilization = decimal.MaxValue;
            int index = -1;
            output.Add(int.MaxValue);
            output.Add(index);

            for (int i = 0; i < servers.Count; i++) {
                decimal x = 0;
                if (totalServiceTime > 0)
                {
                    x = simSys.Servers[i].TotalWorkingTime;
                    x /= totalServiceTime;
                }
                if (x < leastUtilization) {
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
            return rd.Next(1, 100);
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
        private int getMaxQueueLength(SimulationSystem simSys)
        {
            int counter = 0;
            for (int i = 0; i < simSys.SimulationTable.Count; i++)
            {
                int k = 1;
                for (int j = i + 1; j < simSys.SimulationTable.Count; j++)
                {
                    if (simSys.SimulationTable[i].StartTime > simSys.SimulationTable[j].ArrivalTime)
                        k++;
                }
                counter = Math.Max(k, counter);
            }
            return counter;
        }


    }

}
