using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecology_balance.Interfaces;
using System.Drawing;
using NLog;

namespace Ecology_balance.Classes
{
    public class Organism : IOrganism
    {
        public Logger log = LogManager.GetCurrentClassLogger();

        public Organism()
        {

        }

        public int Index = 0;
        public int OrganismIndex = 0;

        public decimal CountRate = 0.1m;
        public int Mass = 100;
        public int MassDelta = 5;
        public int MassDeltaPlus = 0;
        public int MassToDevide = 60;
        public int MaxMass = 150;
        public bool AllowMove = true;

        public decimal FeedRate = 0.5m;

        public int PoisonLevel = 0;
        public int PoisonDelta = 0;
        public int PoisonResistence = 5;

        public int SleepCicles = 0;

        public int ConsumentOrder = 0;

        public decimal MutationRate = 0.2m;

        public Point Position = new Point(0, 0);

        public OrganismState State = OrganismState.Alive;

        public int MaxSteps = 10;

        public ICore Core;
        public IEnv Enviroment;

        public delegate OrganismMehodResult ActionMethod(int index);
        public int ActionIndex = 0;

        #region Get Methods

        public virtual decimal GetCountRate()
        {
            return CountRate;
        }
        public virtual OrganismState GetState()
        {
            return State;
        }
        public virtual int GetPoisonLevel()
        {
            return PoisonLevel;
        }
        public int GetMass()
        {
            return Mass;
        }
        public virtual int GetConsumentOrder()
        {
            return ConsumentOrder;
        }
        public virtual ICore GetCore()
        {
            return Core;
        }
        public virtual IEnv GetEnviroment()
        {
            return Enviroment;
        }
        public Point GetPosition()
        {
            return Position;
        }

        public virtual int GetOrganismIndex()
        {
            return OrganismIndex;
        }

        public virtual decimal GetMutationRate()
        {
            return MutationRate;
        }

        #endregion

        #region Set Methods

        public void SetParams(int index, int countRate, int mass, int maxMass, int massDelta,
            int massDeltaPlus, int massToDevide, bool allowMove, decimal feedRate,
            int poison, int poisonDelta, int poisonResitrance, int sleep,
            int consumentOrder, Point position, ICore core, IEnv enviroment)
        {
            OrganismIndex = index;

            CountRate = countRate;

            Mass = mass;
            MaxMass = maxMass;
            MassDelta = massDelta;
            MassDeltaPlus = massDeltaPlus;
            MassToDevide = massToDevide;

            AllowMove = allowMove;

            FeedRate = feedRate;

            PoisonLevel = poison;
            PoisonDelta = poisonDelta;
            PoisonResistence = poisonResitrance;

            SleepCicles = sleep;

            ConsumentOrder = consumentOrder;

            Position = position;

            Enviroment = enviroment;
            Core = core.Create();
        }

        #endregion

        #region Init Methods
        public virtual void Init(IEnv enviroment, ICore core, Point position, int index)
        {
            Enviroment = enviroment;
            Core = core.Create(core);
            Position = position;
            OrganismIndex = index;

            Config();
        }

        public virtual void Config()
        {
            log.Info($"Created new organism. Index: {OrganismIndex}");
        }

        public virtual IOrganism Create(IEnv enviroment, ICore core, Point position, int index)
        {
            IOrganism org = new Organism();
            org.Init(enviroment, core, position, index);
            return org;
        }

        public virtual IOrganism Create(IEnv enviroment, IOrganism mother, Point position)
        {
            var org = Create(enviroment, mother.GetCore(), position, mother.GetOrganismIndex());

            return org;
        }
        #endregion


        #region Step Methods
        public virtual OrganismState Step()
        {
            Mass -= MassDelta;
            if (Mass <= 0)
                return OrganismState.Dead;

            var actions = Core.GetTriplets(0);
            var skippers = Core.GetTriplets(1);

            for (int i = 0; i < MaxSteps; i++)
            {
                log.Info($"- Step {OrganismIndex}: {i + 1}. Position: {Position.X} : {Position.Y}");

                if (Index < 0 || Index > actions.Count)
                    break;

                var trip = actions.ElementAt(Index);

                if (trip == null)
                    break;

                var action = SelectAction(trip);
                var result = action(ActionIndex);

                if (result == OrganismMehodResult.Success)
                    break;

                var skipTriplet = SelectSkipTriplet(result, ActionIndex);
                Index += Skip(skipTriplet);

                if (Index < 0)
                    break;
            }

            if (Mass > MaxMass)
                Mass = MaxMass;

            return OrganismState.Alive;
        }


        public virtual void Die()
        {
            State = OrganismState.Dead;
        }

        #endregion

        #region Action Methods

        public virtual OrganismMehodResult MoveAction(int index)
        {
            log.Info("Move action");

            if (index < 1 || index > 8)
                return OrganismMehodResult.Fail;

            Point dot = GetCoordinates(index, Position);

            if (dot == Point.Empty)
                return OrganismMehodResult.Fail;

            var obj = Enviroment.OnCell(dot);

            if(obj == null)
            {
                Enviroment.Move(this, Position, dot);
                Position = dot;

                return OrganismMehodResult.Success;
            }

            return OrganismMehodResult.Fail;
        }


        public virtual OrganismMehodResult ActivateAction(int index)
        {
            log.Info("Activate action");

            return OrganismMehodResult.Null;
        }

        public virtual OrganismMehodResult FightAction(int index)
        {
            log.Info("Fight action");
            return OrganismMehodResult.Null;
        }

        public virtual OrganismMehodResult WatchAction(int index)
        {
            log.Info("Watch action");
            Point dot = GetCoordinates(index, Position);

            if (dot == Point.Empty)
                return OrganismMehodResult.Wall;

            var obj = Enviroment.OnCell(dot);

            if (obj == null)
                return OrganismMehodResult.Empty;

            int i = obj.GetConsumentOrder();

            if (i - 1 == ConsumentOrder)
                return OrganismMehodResult.Food;
            else if (i + 1 == ConsumentOrder)
                return OrganismMehodResult.Enemy;

            return OrganismMehodResult.Wall;
        }

        public virtual OrganismMehodResult SleepAction(int index)
        {
            log.Info("Sleep action");
            return OrganismMehodResult.Success;
        }

        public virtual OrganismMehodResult Duplicate(int index)
        {
            log.Info("Duplicate action");
            return OrganismMehodResult.Null;
        }

        #endregion

        #region Utilites Methods

        public virtual bool TryMutate()
        {
            Random rand = new Random();
            var rate = (decimal)rand.Next(0, 1000) / 1000m;

            if(rate < MutationRate)
            {
                Core.MutateAll();
                return true;
            }

            return false;
        }

        protected virtual Point GetCoordinates(int index, Point p)
        {
            int size = Enviroment.GetSize();
            Point result = Point.Empty;

            if (index == 1)
                result = new Point(p.X - 1, p.Y - 1);
            else if (index == 2)
                result = new Point(p.X, p.Y - 1);
            else if (index == 3)
                result = new Point(p.X + 1, p.Y - 1);
            else if (index == 4)
                result = new Point(p.X + 1, p.Y);
            else if (index == 5)
                result = new Point(p.X + 1, p.Y + 1);
            else if (index == 6)
                result = new Point(p.X, p.Y + 1);
            else if (index == 7)
                result = new Point(p.X - 1, p.Y + 1);
            else
                result = new Point(p.X - 1, p.Y);

            if(result != Point.Empty)
            {
                if (result.X < 0 || result.Y < 0 || result.X >= size || result.Y >= size)
                    result = Point.Empty;
            }

            return result;
        }

        public virtual Nuc[] SelectSkipTriplet(OrganismMehodResult result, int index)
        {
            var i = 0;

            if (result == OrganismMehodResult.Empty)
                i = 1;
            else if (result == OrganismMehodResult.Enemy)
                i = 2;
            else if (result == OrganismMehodResult.Wall)
                i = 3;
            else if (result == OrganismMehodResult.Fail)
                i = 4;
            else if (result == OrganismMehodResult.Food)
                i = 5;
            else if (result == OrganismMehodResult.Poison)
                i = 6;
            else if (result == OrganismMehodResult.Success)
                i = 7;

            var dna = Core.GetTriplets(1);
            var number = i * 8 + index;

            var triplet = dna.ElementAt(number);
            return triplet;
        }

        public virtual int Skip(Nuc[] tr)
        {
            if (tr.Length != 3)
                return 0;

            Nuc f = tr[0];
            Nuc s = tr[1];
            Nuc t = tr[2];

            if (f == Nuc.A)
            {

                if (s == Nuc.A)
                    return 1;
                else if (s == Nuc.T)
                    return 2;
                else if (s == Nuc.C)
                    return 3;
                else if (s == Nuc.G)
                    return 4;
                else
                    return 0;
            }
            else if (f == Nuc.T)
            {
                if (s == Nuc.A)
                    return 5;
                else if (s == Nuc.T)
                    return 6;
                else if (s == Nuc.C)
                    return 7;
                else if (s == Nuc.G)
                    return 8;
                else
                    return 0;
            }
            else if (f == Nuc.C)
            {
                if (s == Nuc.A)
                    return -1;
                else if (s == Nuc.T)
                    return -2;
                else if (s == Nuc.C)
                    return -3;
                else if (s == Nuc.G)
                    return -4;
                else
                    return 0;
            }
            else if (f == Nuc.G)
            {
                if (s == Nuc.A)
                    return -5;
                else if (s == Nuc.T)
                    return -6;
                else if (s == Nuc.C)
                    return -7;
                else if (s == Nuc.G)
                    return -8;
                else
                    return 0;
            }

            return 0;
        }

        public virtual ActionMethod SelectAction(Nuc[] tr)
        {
            Nuc f = tr[0];
            Nuc s = tr[1];
            Nuc t = tr[2];

            if (f == Nuc.A)
            {
                if (s == Nuc.A)
                {
                    var r = new ActionMethod(MoveAction);

                    if (t == Nuc.A)
                        ActionIndex = 1;
                    else if (t == Nuc.T)
                        ActionIndex = 2;
                    else if (t == Nuc.C)
                        ActionIndex = 3;
                    else
                        ActionIndex = 4;

                    return r;
                }
                else if (s == Nuc.T)
                {
                    var r = new ActionMethod(MoveAction);

                    if (t == Nuc.A)
                        ActionIndex = 5;
                    else if (t == Nuc.T)
                        ActionIndex = 6;
                    else if (t == Nuc.C)
                        ActionIndex = 7;
                    else
                        ActionIndex = 8;

                    return r;
                }
                else if (s == Nuc.C)
                {
                    var r = new ActionMethod(FightAction);

                    if (t == Nuc.A)
                        ActionIndex = 1;
                    else if (t == Nuc.T)
                        ActionIndex = 2;
                    else if (t == Nuc.C)
                        ActionIndex = 3;
                    else
                        ActionIndex = 4;

                    return r;
                }
                else
                {
                    var r = new ActionMethod(FightAction);

                    if (t == Nuc.A)
                        ActionIndex = 5;
                    else if (t == Nuc.T)
                        ActionIndex = 6;
                    else if (t == Nuc.C)
                        ActionIndex = 7;
                    else
                        ActionIndex = 8;

                    return r;
                }
            }
            else if (f == Nuc.T)
            {
                if (s == Nuc.A)
                {
                    var r = new ActionMethod(WatchAction);

                    if (t == Nuc.A)
                        ActionIndex = 1;
                    else if (t == Nuc.T)
                        ActionIndex = 2;
                    else if (t == Nuc.C)
                        ActionIndex = 3;
                    else
                        ActionIndex = 4;

                    return r;
                }
                else if (s == Nuc.T)
                {
                    var r = new ActionMethod(WatchAction);

                    if (t == Nuc.A)
                        ActionIndex = 5;
                    else if (t == Nuc.T)
                        ActionIndex = 6;
                    else if (t == Nuc.C)
                        ActionIndex = 7;
                    else
                        ActionIndex = 8;

                    return r;
                }
                if (s == Nuc.C)
                {
                    var r = new ActionMethod(ActivateAction);

                    if (t == Nuc.A)
                        ActionIndex = 1;
                    else if (t == Nuc.T)
                        ActionIndex = 2;
                    else if (t == Nuc.C)
                        ActionIndex = 3;
                    else
                        ActionIndex = 4;

                    return r;
                }
                else if (s == Nuc.G)
                {
                    var r = new ActionMethod(ActivateAction);

                    if (t == Nuc.A)
                        ActionIndex = 5;
                    else if (t == Nuc.T)
                        ActionIndex = 6;
                    else if (t == Nuc.C)
                        ActionIndex = 7;
                    else
                        ActionIndex = 8;

                    return r;
                }
            }
            else if (f == Nuc.C)
            {
                if (s == Nuc.A)
                {
                    var r = new ActionMethod(Duplicate);

                    if (t == Nuc.A)
                        ActionIndex = 1;
                    else if (t == Nuc.T)
                        ActionIndex = 2;
                    else if (t == Nuc.C)
                        ActionIndex = 3;
                    else
                        ActionIndex = 4;

                    return r;
                }
                else if (s == Nuc.T)
                {
                    var r = new ActionMethod(Duplicate);

                    if (t == Nuc.A)
                        ActionIndex = 5;
                    else if (t == Nuc.T)
                        ActionIndex = 6;
                    else if (t == Nuc.C)
                        ActionIndex = 7;
                    else
                        ActionIndex = 8;

                    return r;
                }
            }

            ActionIndex = 0;
            return new ActionMethod(SleepAction);
        }

        #endregion

    }
}
