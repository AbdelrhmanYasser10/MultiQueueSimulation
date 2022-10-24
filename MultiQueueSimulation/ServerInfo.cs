using MultiQueueModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiQueueSimulation
{
    public partial class ServerInfo : Form
    {

        private Server server;
        public ServerInfo()
        {
            InitializeComponent();
        }

        public ServerInfo(Server server) {
            InitializeComponent();
            this.server = server;
        }

        private void ServerInfo_Load(object sender, EventArgs e)
        {
            int row = 1;
            for (int i = 0; i < server.TimeDistribution.Count; i++)
            {
                TimeDistribution t = server.TimeDistribution[i];
                tableLayoutPanel1.Controls.Add(new Label() { Text = t.Time.ToString() }, 0, row);
                tableLayoutPanel1.Controls.Add(new Label() { Text = t.Probability.ToString() }, 1, row);
                tableLayoutPanel1.Controls.Add(new Label() { Text = t.CummProbability.ToString() }, 2, row);
                tableLayoutPanel1.Controls.Add(new Label() { Text = "" + t.MinRange + " - " + t.MaxRange }, 3, row);
                row++;
            }
            label1.Text = "Server #" + server.ID;
            if (server.ID == HomePage.getSimSys().Servers[HomePage.getSimSys().NumberOfServers - 1].ID) {

                button1.Text = "Close"; 
            
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (server.ID == HomePage.getSimSys().Servers[HomePage.getSimSys().NumberOfServers - 1].ID)
            {

                Dispose();

            }
            else {
               
                Dispose();
                new ServerInfo(HomePage.getSimSys().Servers[server.ID]).Show();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
