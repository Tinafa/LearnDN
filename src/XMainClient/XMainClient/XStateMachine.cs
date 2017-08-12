using System;
using System.Collections.Generic;

namespace XMainClient
{
    public interface IAnimStateMachine
    {
        void OnAnimationOverrided();
    }

    internal sealed class XStateMachine : XComponent,IAnimStateMachine
    {
        public static new readonly uint uuID = XCommon.Singleton.XHash("StateMachine");
        public override uint ID { get { return uuID; } }

        public void OnAnimationOverrided()
        {

        }
    }
}
