using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UILib
{
    public interface IXRadarMap
    {
        void Refresh();
        void SetSite(int pos, float value);
    }
}
