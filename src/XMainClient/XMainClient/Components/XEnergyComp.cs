using System;
using XUtliPoolLib;

namespace XMainClient
{
    public class XEnergyComp :XComponent
    {
        public static new readonly uint uuID = XCommon.singleton.XHash("XEnergyComp");
        public override uint ID { get { return uuID; } }
    }
}
