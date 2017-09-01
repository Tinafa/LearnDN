using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMainClient
{
    public class XRole : XEntity
    {
        public override bool Initilize()
        {
            _eEntity_Type |= EntityType.Entity_Role;

            _layer = LayerMask.NameToLayer("Role");

            _brain = XComponentMgr.singleton.CreateComponent(this, XBrainComp.uuID) as XBrainComp;
            _heart = XComponentMgr.singleton.CreateComponent(this, XHeartComp.uuID) as XHeartComp;
            _energy = XComponentMgr.singleton.CreateComponent(this, XEnergyComp.uuID) as XEnergyComp;
            _machine = XComponentMgr.singleton.CreateComponent(this, XStateMachine.uuID) as XStateMachine;
            _presentation = XComponentMgr.singleton.CreateComponent(this, XPresentationComp.uuID) as XPresentationComp;
            _audio = XComponentMgr.singleton.CreateComponent(this, XAudioComp.uuID) as XAudioComp;
            _action = XComponentMgr.singleton.CreateComponent(this, XActionComp.uuID) as XActionComp;

            return base.Initilize();
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        #region Public
        public bool IsAlive { get { return _heart == null ? false : _heart.IsBeating; } }

        #endregion
    }
}
