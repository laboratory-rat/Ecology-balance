using Ecology_balance.Classes;
using Ecology_balance.Classes.ProgressData;
using Ecology_balance.Utilites;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecology_balance
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Start");

            LogManager.Configuration = LogManager.Configuration.Reload();

            var l = LogManager.GetCurrentClassLogger();
            l.Info("Start");

            int size = 10;

            Organism[] orgs = new Organism[]
            {
                new Organism() {CountRate = 0.1m, MutationRate = 0.25m, },
                new Organism() {CountRate = 0.15m, MutationRate = 0.8m, },
                new Organism() {CountRate = 0.2m, MutationRate = 0.1m, },
            };

            Core core = new Core();
            core.Init();

            Progress<ProgressData> progress = new Progress<ProgressData>(ConsoleWrite);

            Env enviroment = new Env(progress);
            enviroment.Init(size, orgs, core);

            Task task = new Task(() => enviroment.Calculate());
            task.Start();

            task.Wait();

            enviroment.Calculate();

            enviroment.Dispose();

            Console.WriteLine("End of program");
            Console.ReadLine();
        }

        static void ConsoleWrite(ProgressData data)
        {
            Console.Write(data.Print());
        }
    }
}
