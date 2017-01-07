using Ecology_balance.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecology_balance.Classes.ActionObject
{
    /*
    class Plant : RootActionObject, IActionObject
    {
        public Plant()
        {

        }

        public Plant(IDNARepository dna, IEnviroment env, int mass)
        {
            DNA = new RootDNARepository((RootDNARepository)dna);
            Env = env;

            Mass = mass;

            Configuration();
        }

        public override void Configuration()
        {
            ConsumentOrder = 0;
            MaxSteps = 5;

            MassDelta = 5;

            _self = typeof(Plant);
        }

        public override ActionObjectMethodResults ActivateAction(int index)
        {
            Mass += MassDelta * 2;
            return ActionObjectMethodResults.Success;
        }

        public override ActionObjectMethodResults FightAction(int index)
        {
            PoisonLevel += PoisonDelta;

            if (PoisonLevel > MaxPoison)
                PoisonLevel = MaxPoison;

            return ActionObjectMethodResults.Success;
        }

        public override ActionObjectMethodResults Duplicate(int index)
        {
            if (Mass < MassToDevide)
                return ActionObjectMethodResults.Fail;

            int x, y;

            if (!GetCoordinates(out x, out y, index))
                return ActionObjectMethodResults.Fail;

            var obj = Env.OnCell(x, y);

            if(obj == null)
            {
                Plant pl = new Plant(new RootDNARepository((RootDNARepository)DNA), (RootEnviroment)Env, Convert.ToInt32((decimal)Mass / 2));
                Mass = Convert.ToInt32((decimal)Mass / 2);


                Env.AddActionObject(pl, x, y);
                return ActionObjectMethodResults.Success;
            }

            return ActionObjectMethodResults.Fail;
        }

        public override Type ObjectType()
        {
            return typeof(Plant);
        }

    }

    */
}
