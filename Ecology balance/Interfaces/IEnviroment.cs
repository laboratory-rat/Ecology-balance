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
    public interface IEnv
    {
        #region Get Methods
        IEnumerable<IOrganism> List();
        int GetSize();
        IOrganism OnCell(Point position);
        decimal EnergyLevel(Point point);

        #endregion

        #region Init Methods

        void Init(int size, IOrganism[] organisms, ICore baseCore);
        void Calculate();
        #endregion

        #region Action Methods

        void Move(IOrganism obj, Point oldPoint, Point newPoint);
        void Kill(Point position);
        void Add(IOrganism obj, Point point);

        #endregion
    }
}
