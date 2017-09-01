using System;
using XUtliPoolLib;
using UnityEngine;

namespace XMainClient
{
    public class XAudioComp : XComponent
    {
        public static new readonly uint uuID = XCommon.singleton.XHash("XAudioComp");
        public override uint ID { get { return uuID; } }

        public void PlayActionClipAt(Vector3 position)
        {

        }
    }
}
