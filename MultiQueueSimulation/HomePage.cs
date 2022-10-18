using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MultiQueueModels;


namespace MultiQueueSimulation
{
    public partial class HomePage : Form
    {

        SimulationSystem simSys;
     

        public HomePage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                simSys = new SimulationSystem();
                string OpenedFilePath = openFileDialog1.FileName;
                if (!OpenedFilePath.Contains(".txt"))
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
                        if (lines[i].ToLower() == ("ServiceDistribution_Server" + (serverNo)).ToLower()) {
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
                    MessageBox.Show(tableLayoutPanel1.RowCount.ToString());


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

        private void resetTableValues() {
            

        }

        
    }
}
