using Ecology_balance.Interfaces;
using Ecology_balance.Utilites;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ecology_balance.Classes
{
    public class Env : IEnv, IDisposable
    {
        public Env()
        {
        }

        public Env(IProgress<ProgressData.ProgressData> progress)
        {
            _progress = progress;
        }

        public List<IOrganism> StartOrganisms = new List<IOrganism>();
        public List<IOrganism> Organisms = new List<IOrganism>();
        public List<IOrganism> DeadOrganisms = new List<IOrganism>();
        public List<IOrganism> LiveOrganisms = new List<IOrganism>();
        public IOrganism[,] Matrix;
        public int Size = 24;

        public List<List<IOrganism>> Isolator = new List<List<IOrganism>>();

        public IProgress<ProgressData.ProgressData> _progress = null;
        Timer _reportTimer = null;

        public decimal[,] EnergyMatrix;

        public int StepsCount = 0;
        public int CyclesCount = 0;
        public int MaxStepsCount = 100000;
        public int MaxCyclesCount = 1000;

        public decimal MutationRate = 0m;
        public decimal TryMutation = 1;
        public decimal SuccessMutation = 1;

        public Random Rand = new Random();

        public ICore BaseCore;

        public void Init(int size, IOrganism[] organisms, ICore baseCore)
        {
            Size = size;
            StartOrganisms = new List<IOrganism>(organisms.ToList());
            BaseCore = baseCore;

            Matrix = new IOrganism[size, size];

            NoiseGenerator noise = new NoiseGenerator(size);
            noise.Generate();
            EnergyMatrix = noise.Matrix;

            ClearMatrix();

            if(_progress != null)
            {
                _reportTimer = new Timer(Report);
                _reportTimer.Change(1000, 10);
            }


            for(int i = 0; i < StartOrganisms.Count; i++)
            {
                var t = StartOrganisms[i];

                int left = (int)Math.Truncate(t.GetCountRate() * size * size);

                do
                {
                    Point p = new Point();

                    p.X = Rand.Next(0, size);
                    p.Y = Rand.Next(0, size);

                    if (Matrix[p.X, p.Y] != null)
                        continue;

                    var obj = t.Create(this, BaseCore, p, i);
                    Add(obj, p);

                    left--;

                } while (left > 0);
            }
        }

        public void Calculate()
        {
            while(CyclesCount < MaxCyclesCount)
            {
                if(CyclesCount == 0)
                {
                    foreach(var o in StartOrganisms)
                    {
                        int count = (int)Math.Truncate(o.GetCountRate() * Size * Size);
                        PlaceOrganisms(o, count, o.GetOrganismIndex());
                    }
                }
                else
                {
                    LiveOrganisms.Clear();
                    ClearMatrix();

                    foreach(var orgs in Isolator)
                    {
                        PlaceOrganisms(orgs);
                    }
                }

                Cicle();
                Restart();

                CyclesCount++;
            }


            for(;StepsCount < MaxStepsCount; StepsCount++)
            {
                for (int i = 0; i < LiveOrganisms.Count; i++) 
                {
                    if (i >= LiveOrganisms.Count)
                        break;

                    var o = LiveOrganisms[i];

                    var result = o.Step();

                    if (result == OrganismState.Dead)
                    {
                        Kill(o.GetPosition());
                    }
                }
            }
        }

        public virtual void PlaceOrganisms(List<IOrganism> container)
        {
            if (container == null || container.Count == 0)
                return;

            var o = container.First();

            int count = (int)Math.Truncate(o.GetCountRate() * Size * Size / container.Count);

            foreach (var organism in container)
            {
                TryMutation++;

                if (organism.TryMutate())
                    SuccessMutation++;

                PlaceOrganisms(organism, count, organism.GetOrganismIndex());
            }
        }


        public virtual void PlaceOrganisms(IOrganism organism, int count, int index)
        {
            
            do
            {
                Point p = new Point();

                p.X = Rand.Next(0, Size);
                p.Y = Rand.Next(0, Size);

                if (Matrix[p.X, p.Y] != null)
                    continue;

                var obj = organism.Create(this, BaseCore, p, index);
                Add(obj, p);

                count--;

            } while (count > 0);
        }

        public void Cicle()
        {
            StepsCount = 0;

            for(; StepsCount < MaxStepsCount; StepsCount++)
            {
                bool restart = false;

                for(int i = 0; i < StartOrganisms.Count; i++)
                {
                    var count = LiveOrganisms.Where(x => x.GetOrganismIndex() == i).Count();

                    if (count < 10)
                    {
                        restart = true;
                        break;
                    }
                }

                if (restart)
                    break;
            }
        }

        public virtual void ClearMatrix()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Matrix[i, j] = null;
                }
            }
        }

        public virtual void Restart()
        {
            Isolator.Clear();

            for (int i = 0; i < StartOrganisms.Count; i++)
            {
                List<IOrganism> specie = new List<IOrganism>(LiveOrganisms.Where(x => x.GetOrganismIndex() == i).Take(10));
                Isolator.Add(specie);
            }
        }

        public virtual void Report(Object state)
        {
            if(_progress != null)
            {
                _progress.Report(new ProgressData.ProgressData()
                {
                    OrganismsAlive = LiveOrganisms.Count,
                    OrganismsAllTime = Organisms.Count,
                    OrganismsDead = DeadOrganisms.Count,
                    Generation = CyclesCount,
                    MutationRate = SuccessMutation / TryMutation,
                });
            }
        }

        #region Get Methods
        public IEnumerable<IOrganism> List()
        {
            return Organisms;
        }
        public int GetSize()
        {
            return Size;
        }
        public IOrganism OnCell(Point point)
        {
            return Matrix[point.X, point.Y];
        }
        public decimal EnergyLevel(Point point)
        {
            return EnergyMatrix[point.X, point.Y];
        }

        #endregion

        #region Action Methods

        public void Move(IOrganism obj, Point oldPoint, Point newPoint)
        {
            var o = Matrix[oldPoint.X, oldPoint.Y];

            Matrix[oldPoint.X, oldPoint.Y] = null;
            Matrix[newPoint.X, newPoint.Y] = o;
        }

        public void Kill(Point point)
        {
            var o = Matrix[point.X, point.Y];

            if(o != null)
            {
                o.Die();
                DeadOrganisms.Add(o);
                LiveOrganisms.Remove(o);
            }
        }

        public void Add(IOrganism obj, Point point)
        {
            var o = Matrix[point.X, point.Y];

            if(o == null)
            {
                Organisms.Add(obj);
                LiveOrganisms.Add(obj);

                Matrix[point.X, point.Y] = obj;
            }
        }

        public void Dispose()
        {
            if (_reportTimer != null)
                _reportTimer.Dispose();
        }

        #endregion
    }


}
