using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecology_balance.Interfaces
{
    public enum Nuc { Null = 0, A, T, G, C };

    public interface ICore
    {
        void Init();

        void Config();

        ICore Create();
        ICore Create(ICore core);

        Nuc[] Collection(int index = 0);
        Nuc OnIndex(int chromosome, int index);
        int Count();

        void Mutate(int chromosomes, int index);
        void Mutate(int chromosome, int index, Nuc newNuc);
        void MutateAll();

        Nuc GetRandomNuc();
        Nuc[] RandomDNA(int length);

        int[] GetChromosomesLength();
        decimal GetMutationRate();

        List<Nuc[]> GetTriplets(int chromosome);
    }
}
