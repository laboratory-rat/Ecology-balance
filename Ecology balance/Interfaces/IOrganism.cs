using Ecology_balance.Classes;
using Ecology_balance.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecology_balance.Interfaces
{
    public enum OrganismState {Dead = 0, Alive};
    public enum OrganismMehodResult { Null = 0, Enemy, Wall, Food, Poison, Success, Fail, Empty };
    public interface IOrganism
    {
        #region Get Methods

        decimal GetCountRate();
        OrganismState GetState();
        int GetPoisonLevel();
        int GetMass();
        int GetConsumentOrder();
        ICore GetCore();
        IEnv GetEnviroment();
        Point GetPosition();
        int GetOrganismIndex();

        decimal GetMutationRate();
        #endregion

        #region Set Methods

        void SetParams(int index, int countRate, int mass, int maxMass, int massDelta,
            int massDeltaPlus, int massToDevide, bool allowMove, decimal feedRate,
            int poison, int poisonDelta, int poisonResitrance, int sleep,
            int consumentOrder, Point position, ICore core, IEnv enviroment);

        #endregion

        #region Init Methods

        void Init(IEnv enviroment, ICore core, Point position, int index);
        void Config();
        IOrganism Create(IEnv enviroment, ICore core, Point position, int index);
        IOrganism Create(IEnv enviroment, IOrganism mother, Point position);


        #endregion


        #region Step Methods

        OrganismState Step();

        #endregion

        #region Action Methods

        OrganismMehodResult MoveAction(int index);
        OrganismMehodResult ActivateAction(int index);
        OrganismMehodResult FightAction(int index);
        OrganismMehodResult WatchAction(int index);
        OrganismMehodResult SleepAction(int index);
        OrganismMehodResult Duplicate(int index);

        #endregion

        void Die();

        bool TryMutate();
    }
}
