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
        protected int _layer = 0;

        protected XGameObject _xobject = null;
        protected XAttrComp _attr = null;
        protected XBrainComp _brain = null;
        protected XHeartComp _heart = null;
        protected XEnergyComp _energy = null;
        protected XStateMachine _machine = null;
        protected XPresentationComp _presentation = null;
        protected XAudioComp _audio = null;
        protected XActionComp _action = null;

        public XGameObject EngineObject { get { return _xobject; } }

        public XAttrComp Attr { get { return _attr; } }
        public XBrainComp Brain { get { return _brain; } }
        public XHeartComp Heart { get { return _heart; } }
        public XEnergyComp Energy { get { return _energy; } }
        public XStateMachine Machine { get { return _machine; } }
        public XPresentationComp Presentation { get { return _presentation; } }
        public XAudioComp Audio { get { return _audio; } }
        public XActionComp Action { get { return _action; } }

        public bool IsPlayer { get { return (_eEntity_Type & EntityType.Entity_Player) != 0; } }


        public virtual ulong ID
        {
            get { return _attr != null ? _attr.EntityID : 0; }
        }


        public XEntity()
        {

        }
        public bool Initilize(XGameObject o, XAttrComp attr)
        {
            //> 
            _xobject = o;
            _layer = o.Layer;

            bool res = Initilize();
            return res;
        }

        public override void OnCreated()
        {
            //> CreateComponents

            base.OnCreated();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void Uninitilize()
        {

            base.Uninitilize();
        }

        public override void Update(float fDeltaT)
        {
            base.Update(fDeltaT);
            
            EngineObject.Update();
        }

        public override bool DispatchEvent(XEventArgs e)
        {
            //> Brain Handler!

            return base.DispatchEvent(e);
        }
    }
}
