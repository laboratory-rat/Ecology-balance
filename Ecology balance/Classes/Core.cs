using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecology_balance.Interfaces;

namespace Ecology_balance.Classes
{
    public class Core : ICore
    {
        public Core()
        {

        }

        public List<Nuc[]> Chromosomes = new List<Nuc[]>();
        public int[] ChromosomeLength = new int[] { 192, 192 };
        public decimal MutationRate = 0.0125m;

        public Random Rand = new Random();

        public Nuc[] Collection(int index = 0)
        {
            return Chromosomes.Skip(index).FirstOrDefault();
        }

        public int Count()
        {
            return Chromosomes.Count();
        }

        public ICore Create()
        {
            var core = new Core();
            core.Init();

            return (ICore)core;
        }


        public virtual ICore Create(ICore mother)
        {
            Core core = new Core() { ChromosomeLength = mother.GetChromosomesLength(), MutationRate = mother.GetMutationRate() };
            core.Init();

            return core;
        }

        public void Config()
        {
            
        }


        public void Init()
        {
            foreach(var i in ChromosomeLength)
            {
                Nuc[] dna = RandomDNA(i);
                Chromosomes.Add(dna);
            }
        }



        public virtual int[] GetChromosomesLength()
        {
            return ChromosomeLength;
        }

        public virtual decimal GetMutationRate()
        {
            return MutationRate;
        }

        public virtual void Mutate(int chromosome, int index)
        {
            Mutate(chromosome, index, GetRandomNuc());
        }

        public virtual void Mutate(int chromosome, int index, Nuc newNuc)
        {
            if (Count() - 1 > chromosome)
                return;

            var chr = Chromosomes.ElementAt(chromosome);

            if (chr.Length < index)
                return;

            chr[index] = newNuc;
        }

        public virtual void MutateAll()
        {
            foreach (var c in Chromosomes)
            {
                for (int i = 0; i < c.Length; i++)
                {
                    var rate = ((decimal)Rand.Next(0, 100) / (decimal)100);

                    if (rate <= MutationRate)
                        c[i] = GetRandomNuc();
                }
            }
        }

        public virtual Nuc OnIndex(int chromosome, int index)
        {
            if (Count() - 1 < chromosome)
                return Nuc.Null;

            if (Chromosomes.ElementAt(chromosome).Length < index)
                return Nuc.Null;

            return Chromosomes.ElementAt(chromosome)[index];

        }

        public virtual Nuc[] RandomDNA(int length)
        {
            Nuc[] array = new Nuc[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = GetRandomNuc();
            }

            return array;
        }

        public virtual Nuc GetRandomNuc()
        {
            var i = ((double)Rand.Next(0, 100) / (double)100);

            if (i <= 0.25d)
                return Nuc.A;
            else if (i <= 0.5d)
                return Nuc.T;
            else if (i <= 0.75d)
                return Nuc.G;

            return Nuc.C;
        }

        public virtual List<Nuc[]> GetTriplets(int chromosome)
        {
            Nuc[] coll = Collection(chromosome);

            if (coll == null)
                return null;

            List<Nuc[]> result = new List<Nuc[]>();

            var index = coll.Length % 3;

            if (index != 0)
                coll = coll.Skip(-index).ToArray();

            for (int i = 0; i < coll.Length / 3; i++)
                result.Add(coll.Skip(i * 3).Take(3).ToArray());

            return result;

        }
    }
}
