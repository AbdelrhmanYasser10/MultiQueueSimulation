using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MultiQueueModels;

namespace MultiQueueSimulation
{
    public partial class HomePage : Form
    {
        
        private static SimulationSystem simSys;

        public HomePage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (simSys != null)
                resetTableValues(simSys.InterarrivalDistribution.Count);
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                simSys = new SimulationSystem();
                string OpenedFilePath = openFileDialog1.FileName;
             
                if (!OpenedFilePath.EndsWith(".txt"))
                {
                    MessageBox.Show("Make sure to select text file");
                }
                else {
                    /*
                        * This indecies will be needed for the interarrival time
                        * Servers probabilties
                     */
                    List<int> indecies = new List<int>();
                    //Lines of the file
                    string[] lines = File.ReadAllLines(OpenedFilePath);
                    int serverNo = 1;
                    for (int i = 0; i < lines.Length; i++) {
                        switch (lines[i]) {
                            case "NumberOfServers":
                                simSys.NumberOfServers = int.Parse(lines[i + 1]);
                                textBox2.Text = simSys.NumberOfServers.ToString();
                                break;
                            case "StoppingNumber":
                                simSys.StoppingNumber = int.Parse(lines[i + 1]);
                                break;
                            case "StoppingCriteria":
                                simSys.StoppingCriteria = GetStoppingCriteria(int.Parse(lines[i + 1]));
                                break;
                            case "SelectionMethod":
                                simSys.SelectionMethod = GetSelectionMethod(int.Parse(lines[i + 1]));
                                break;
                            case "InterarrivalDistribution":
                                indecies.Add(i + 1);
                                break;
                            default:
                                break;
                        }
                        if (lines[i] == "ServiceDistribution_Server" + (serverNo)) {
                            indecies.Add(i + 1); 
                            serverNo++;
                        }
                    }
                    int interArrIndex = indecies[0];
                    decimal cummulativeProb = 0;

                    //edit the condition
                    while (lines[interArrIndex] != ("\n") && lines[interArrIndex] != ("\r") && lines[interArrIndex] != ("\r\n") && lines[interArrIndex] != ("")) {
                        TimeDistribution timeDistribution = new TimeDistribution();
                        string[] timeAndProb = lines[interArrIndex].Split(',');           
                        timeDistribution.Time = int.Parse(timeAndProb[0]);
                        timeDistribution.MinRange = (cummulativeProb == 0) ? 0 : (int)(cummulativeProb * 100) + 1;
                        cummulativeProb += decimal.Parse(timeAndProb[1]);
                        timeDistribution.Probability = decimal.Parse(timeAndProb[1]);
                        timeDistribution.CummProbability = cummulativeProb;
                        timeDistribution.MaxRange = (int)(timeDistribution.CummProbability * 100);
                        simSys.InterarrivalDistribution.Add(timeDistribution);


                        //*/***Doen't make any sense
                        tableLayoutPanel1.RowCount++;

                        interArrIndex++;
                    }
                    int row = 1;
                    for (int i = 0; i < simSys.InterarrivalDistribution.Count; i++) {

                        TimeDistribution t = simSys.InterarrivalDistribution[i];
                        tableLayoutPanel1.Controls.Add(new Label() { Text = t.Time.ToString()}, 0, row);
                        tableLayoutPanel1.Controls.Add(new Label() { Text = t.Probability.ToString() }, 1, row);
                        tableLayoutPanel1.Controls.Add(new Label() { Text = t.CummProbability.ToString() }, 2, row);
                        tableLayoutPanel1.Controls.Add(new Label() { Text = ""+t.MinRange + " - "+ t.MaxRange }, 3, row);
                        
                        row++;
                    }
                    serverNo = 1;
                    for (int i = 1; i < indecies.Count; i++) {
                        int index = indecies[i];
                        decimal cummulativeProbabilityForServer = 0;
                        Server server = new Server();
                        server.ID = serverNo;
                        while (index < lines.Length && lines[index] != "") {
                            TimeDistribution t = new TimeDistribution();
                            string[] timeAndProb = lines[index].Split(',');
                            t.Time = int.Parse(timeAndProb[0]);
                            t.MinRange = (cummulativeProbabilityForServer == 0) ? 0 : (int)(cummulativeProbabilityForServer * 100) + 1;
                            cummulativeProbabilityForServer += decimal.Parse(timeAndProb[1]);
                            t.Probability = decimal.Parse(timeAndProb[1]);
                            t.Probability = decimal.Parse(timeAndProb[1]);
                            t.CummProbability = cummulativeProbabilityForServer;
                            t.MaxRange = (int)(t.CummProbability * 100);
                            server.TimeDistribution.Add(t);
                            index++;
                        }
                        simSys.Servers.Add(server);
                        serverNo++;
                    }

                }
            }
        }
      
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private Enums.StoppingCriteria GetStoppingCriteria(int number)
        {
            return number == 1 ? Enums.StoppingCriteria.NumberOfCustomers : Enums.StoppingCriteria.SimulationEndTime;
        }

        private Enums.SelectionMethod GetSelectionMethod(int number)
        {
            return number == 1 ? Enums.SelectionMethod.HighestPriority : number  == 2 ? Enums.SelectionMethod.Random : Enums.SelectionMethod.LeastUtilization;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (simSys == null)
            {
                MessageBox.Show("Make sure to read test case first");
            }
            else {
               

                ServerInfo s = new ServerInfo(simSys.Servers[0]);
                s.Show();
               
            }
        }

        public static SimulationSystem getSimSys() {
            return simSys;
        }
        private void resetTableValues(int count)
        {

            for (int i = 0; i < count; i++)
            {
                TableLayoutHelper.RemoveArbitraryRow(tableLayoutPanel1, 1);

            }

        }

        private void HomePage_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SimulatinTableForm simForm = new SimulatinTableForm(simSys);
            simForm.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
