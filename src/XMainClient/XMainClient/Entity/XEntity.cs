using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMainClient
{
    public abstract class XEntity : XObject
    {
        protected enum EntityType
        {
            Entity_None = 1 << 0,
            Entity_Role = 1 << 1,
            Entity_Player = 1 << 2,


        }

        protected EntityType _eEntity_Type = EntityType.Entity_None;

        protected XGameObject _xobject = null;

        protected XStateMachine _machine = null;

        public XGameObject EngineObject { get { return _xobject; } }
        public XStateMachine Machine { get { return _machine; } }

        public bool IsPlayer { get { return (_eEntity_Type & EntityType.Entity_Player) != 0; } }

        public XEntity()
        {

        }
        public bool Initilize(XGameObject o, XAttributes attr)
        {
            return true;
        }

        public override void OnCreated()
        {
            //>CreateComponents

            base.OnCreated();
        }


    }
}
