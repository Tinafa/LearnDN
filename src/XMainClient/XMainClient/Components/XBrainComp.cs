using System;
using XUtliPoolLib;


namespace XMainClient
{
    public class XBrainComp : XComponent
    {
        public static new readonly uint uuID = XCommon.singleton.XHash("XBrainComp");
        public override uint ID { get { return uuID; } }
    }
}
