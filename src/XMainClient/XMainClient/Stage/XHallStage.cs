using System;
using System.Collections.Generic;
using UnityEngine;
using XMainClient.UI;

namespace XMainClient
{
    class XHallStage : XStage
    {
        private EXStage _eOld = EXStage.Null;

        public XHallStage() : base(EXStage.Hall) { }

        public GameObject gameGB = null;
        private XGameManager gameManager;
        private XSoundManager soundManager;

        public override void OnEnterStage(EXStage eOld)
        {
            base.OnEnterStage(eOld);

            _eOld = eOld;
        }

        public override void OnLeaveStage(EXStage eNew)
        {
            base.OnLeaveStage(eNew);
        }

        public override void OnEnterScene(uint sceneid, bool transfer)
        {
            base.OnEnterScene(sceneid, transfer);

            gameGB = GameObject.Find("GameManager");
            if(gameGB != null)
            {
                gameManager = gameGB.AddComponent<XGameManager>();
                soundManager = gameGB.AddComponent<XSoundManager>();
            }

            XGameUI.singleton.LoadHallUI(_eStage);
        }

        public override void OnLeaveScene(bool transfer)
        {
            base.OnLeaveScene(transfer);

            XGameUI.singleton.UnLoadHallUI(_eStage);
        }


    }
}
