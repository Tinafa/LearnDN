using System;
using System.Collections.Generic;
using UnityEngine;
using XUtliPoolLib;

namespace XMainClient
{
    class XLoginStage : XStage
    {
        private EXStage _eOld = EXStage.Null;

        public XLoginStage() : base(EXStage.Login) { }

        private bool _ready = false;

        public override void OnEnterStage(EXStage eOld)
        {
            base.OnEnterStage(eOld);

            _ready = false;

            _eOld = eOld;
        }

        public override void OnLeaveStage(EXStage eNew)
        {
            base.OnLeaveStage(eNew);
        }

        public override void OnEnterScene(uint sceneid, bool transfer)
        {
            base.OnEnterScene(sceneid, transfer);
        }

        public override void OnLeaveScene(bool transfer)
        {
            base.OnLeaveScene(transfer);
        }

        public override void Play()
        {
            _ready = true;
        }


        public override void Update(float fDeltaT)
        {
            base.Update(fDeltaT);
        }

        public override void PostUpdate(float fDeltaT)
        {
            base.PostUpdate(fDeltaT);

            if (_ready && XScene.singleton.SceneReady)
            {
                _ready = false;
               // XGame.singleton.SwitchTo(EXStage.SelectChar, 3);
            }
        }
    }
}
