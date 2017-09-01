using System;
using XUtliPoolLib;

namespace XMainClient
{
    public class XPresentationComp : XComponent
    {
        public static new readonly uint uuID = XCommon.singleton.XHash("XPresentationComp");
        public override uint ID { get { return uuID; } }
    }
}
