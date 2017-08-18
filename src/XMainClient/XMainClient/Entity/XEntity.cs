using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMainClient
{
    public abstract class XEntity : XObject
    {
        protected enum EnitityType
        {
            Entity_None = 1 << 0,
            Entity_Role = 1 << 1,
            Entity_Player = 1 << 2,


        }

        protected EnitityType _eEntity_Type = EnitityType.Entity_None;

        protected XStateMachine _machine = null;

        public XStateMachine Machine { get { return _machine; } }

        public bool IsPlayer { get { return (_eEntity_Type & EnitityType.Entity_Player) != 0; } }
    }
}
