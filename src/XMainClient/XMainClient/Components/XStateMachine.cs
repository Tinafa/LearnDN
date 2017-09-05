using System;
using System.Collections.Generic;
using XUtliPoolLib;

namespace XMainClient
{
    public interface IAnimStateMachine
    {
        void OnAnimationOverrided();
    }

    public sealed class XStateMachine : XComponent,IAnimStateMachine
    {
        public static new readonly uint uuID = XCommon.singleton.XHash("XStateMachine");
        public override uint ID { get { return uuID; } }

        public void OnAnimationOverrided()
        {

        }
    }
}
