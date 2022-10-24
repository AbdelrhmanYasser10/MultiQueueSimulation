using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            this.Servers = new List<Server>();
            this.InterarrivalDistribution = new List<TimeDistribution>();
            this.PerformanceMeasures = new PerformanceMeasures();
            this.SimulationTable = new List<SimulationCase>();
        }

        ///////////// INPUTS ///////////// 
        public int NumberOfServers { get; set; }
        public int StoppingNumber { get; set; }
        public List<Server> Servers { get; set; }
        public List<TimeDistribution> InterarrivalDistribution { get; set; }
        public Enums.StoppingCriteria StoppingCriteria { get; set; }
        public Enums.SelectionMethod SelectionMethod { get; set; }

        public int getNumberOfServers()
        {
            return this.NumberOfServers;
        }
        public int getStoppingNumber()
        {
            return this.StoppingNumber;
        }
        public List<Server> getServers()
        {
            return this.Servers;
        }
        public List<TimeDistribution> getInterarrivalDistribution()
        {
            return this.InterarrivalDistribution;
        }
        public Enums.StoppingCriteria getStoppingCriteria()
        {
            return this.StoppingCriteria;
        }
        public Enums.SelectionMethod getSelectionMethod()
        {
            return this.SelectionMethod;
        }
   


        ///////////// OUTPUTS /////////////
        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }

        public List<SimulationCase> getSimulationTable()
        {
            return this.SimulationTable;
        }
        public PerformanceMeasures getPerformanceMeasures()
        {
            return this.PerformanceMeasures;
        }

    }
}
