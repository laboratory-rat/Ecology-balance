using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecology_balance.Classes.ProgressData
{
    public class ProgressData
    {
        public int OrganismsAllTime { get; set; } = 0;
        public int OrganismsAlive { get; set; } = 0;
        public int OrganismsDead { get; set; } = 0;
        public int Generation { get; set; } = 0;
        public decimal MutationRate { get; set; } = 0m;

        protected string _separator = "----------------------------";

        public string Print()
        {
            StringBuilder s = new StringBuilder();

            s.AppendLine($"{ _separator} {DateTime.Now.ToShortTimeString()} {_separator}");
            s.AppendLine($"All: {OrganismsAllTime}");
            s.AppendLine($"Alive: {OrganismsAlive}");
            s.AppendLine($"Dead: {OrganismsDead}");
            s.AppendLine($"Generation: {Generation}");
            s.AppendLine($"Mutation rate: {MutationRate}");
            s.AppendLine($"{ _separator}{_separator}");

            return s.ToString();
        }
    }

}
