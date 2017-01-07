using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecology_balance.Utilites
{
    class DiamondAndSquare
    {
        public int size;

        public decimal[,] Matrix;

        public Random Rand = new Random();

        public decimal Scale = 0.1m;

        public DiamondAndSquare(int s)
        {
            size = s;
            Matrix = new decimal[s, s];


        }

        public void Generate()
        {
            int x, y;
            x = y = (size + 1) / 2;

            diamond(size, size, x, y, Rand.Next(), Rand.Next(), Rand.Next(), Rand.Next());

        }

        public void diamond(int width, int height, int x, int y, decimal r1, decimal r2, decimal r3, decimal r4)
        {
            Matrix[x, y] = (r1 + r2 + r3 + r4) / 4 * Scale;

            if(width > 1 && height > 1)
            {
                int newX = (x + 1) / 2;
                int newY = (y + 1) / 2;


            }
        }

        public void Square()
        {

        }


    }
}
