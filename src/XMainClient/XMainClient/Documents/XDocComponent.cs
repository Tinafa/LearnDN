using System;
using System.Collections.Generic;

namespace XMainClient
{
    internal abstract class XDocComponent : XComponent
    {
        protected override void EventSubscribe()
        {
            
        }

        public virtual void OnEnterSceneFinally()
        {

        }

        public virtual void OnSceneStarted()
        {

        }

        public virtual void OnGamePause(bool pause)
        {

        }
    }
}
