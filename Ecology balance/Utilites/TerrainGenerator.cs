using SharpNoise.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ecology_balance.Utilites
{
    class NoiseGenerator
    {

        public decimal[,] Matrix;
        public Random Rand = new Random();

        private int _size;

        public NoiseGenerator()
        {
            Matrix = new decimal[10, 10];
        }

        public NoiseGenerator(int size)
        {
            Matrix = new decimal[size, size];
            _size = size;
        }

        public void Generate()
        {
            Perlin model = new Perlin();

            decimal x, y, z, delta;
            x = (decimal)Rand.Next(0, 1000) / 1000m;
            y = (decimal)Rand.Next(0, 1000) / 1000m;
            z = (decimal)Rand.Next(0, 1000) / 1000m;

            delta = 0.1m;

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Matrix[i, j] = Math.Abs(Convert.ToDecimal(model.GetValue((double)x, (double)y, (double)z)));

                    x += delta * (decimal)Rand.Next(-1000, 1000) / 1000m;

                    if (x > 1)
                        x = 1;
                    else if (x < 0)
                        x = 0;

                    y += delta * (decimal)Rand.Next(-1000, 1000) / 1000m;

                    if (y > 1)
                        y = 1;
                    else if (y < 0)
                        y = 0;

                    z += delta * (decimal)Rand.Next(-1000, 1000) / 1000m;

                    if (z > 1)
                        z = 1;
                    else if (z < 0)
                        z = 0;

                }
            }
        }
    }
}
