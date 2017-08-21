using System;
using System.Collections.Generic;
using System.Collections;
using XUtliPoolLib;
using UnityEngine;

namespace XMainClient
{
    public class XGame : XSingleton<XGame>
    {
        private XStage _stage = null;
        private XStage _old_stage = null;

        private List<XBaseSingleton> _singletons = null;
        private XDocuments _doc = null;

        public static GameObject XGameRoot = null;

        private int _fps_count = 0;
        private float _fps = XShell.TargetFrame;
        private float _fpsAvg = XShell.TargetFrame;
#if DEBUG
        private float _fps_interval = 0.16667f;
        private bool _calc_fps = true;
#else
        private bool _calc_fps = false;
#endif
        private float _fps_real_time = 0;
        private int _fpsCalcCount = 0;
        private float _fpsAcc = 0.0f;




#if DEBUG
        private XTimerMgr.ElapsedEventHandler _fpsHandler = null;
#endif
        private XTimerMgr.ElapsedEventHandler _innerSwitchCb = null;


        public bool switchScene = false;

        public XDocuments Doc
        {
            get { return _doc; }
        }

        public bool notLoadScene = true;

        public bool StageReady
        {
            get { return _stage != null && _stage.IsEntered; }
        }

        public XGame()
        {
            _singletons = new List<XBaseSingleton>();

            _singletons.Add(XGameUI.singleton);
            _singletons.Add(UIManager.singleton);
            _singletons.Add(XScene.singleton);
            _singletons.Add(XSceneMgr.singleton);
            _singletons.Add(XAttributeMgr.singleton);
            _singletons.Add(XTalkCenter.singleton);

#if DEBUG
            _fpsHandler = new XTimerMgr.ElapsedEventHandler(CalculateFPS);
#endif
            _innerSwitchCb = new XTimerMgr.ElapsedEventHandler(this.InnerSwitch);
            _doc = new XDocuments();
        }

        public IEnumerator Awake()
        {
            TriggerFps();
            XGameRoot = GameObject.Find("XGamePoint");

            //TableReader.Init();

            int i = 0;
            for (i = 0; i < _singletons.Count; i++)
            {
                int tryCount = 0;
                while (!_singletons[i].Init())
                {
                    tryCount++;
                    if (tryCount % 1000 == 0)
                        System.Threading.Thread.Sleep(1);
                }
            }

            _doc.CtorLoad();
            //TableReader.UInit();

            _doc.Initilize();

            yield return null;
        }

        public void PreUpdate(float delta)
        {
            if (StageReady && notLoadScene)
            {
                _stage.PreUpdate(delta);
            }
        }

        public void Update(float delta)
        {
            if (StageReady && notLoadScene)
            {
                _stage.Update(delta);
                _doc.Update(delta);
            }

            if (_calc_fps) _fps_count++;
        }

        public void PostUpdate(float delta)
        {
            if (StageReady && notLoadScene)
            {
                _stage.PostUpdate(delta);
                _doc.PostUpdate(delta);
            }
        }

        public override bool Init()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer)
                Application.targetFrameRate = 60;
            else
                Application.targetFrameRate = XShell.TargetFrame;

            SwitchTo(EXStage.Login, 2);
            return true;
        }

        public override void Uninit()
        {
            SwitchTo(EXStage.Null, 0);

            _doc.Uninitilize();

            for (int i = _singletons.Count - 1; i >= 0; i--)
            {
                _singletons[i].Uninit();
            }

            XInterfaceMgr.singleton.Uninit();
        }

        public void SwitchTo(EXStage eStage, uint sceneID)
        {
            if (EXStage.Null == eStage || sceneID == 0)
            {
                _old_stage = _stage;

                _old_stage.OnLeaveStage(EXStage.Null);
                _old_stage.OnLeaveScene(false);
                XScene.singleton.OnLeaveScene(false);

                _old_stage = null;
            }else
            {
                int n = EnumInt32ToInt.Convert<EXStage>(eStage);
                uint para = (((uint)n << 16) | sceneID);

                if ( false /*(eStage == EXStage.World || eStage == EXStage.Hall) && XScene.singleton.TurnToTransfer(sceneID)*/)
                {
                   // XAutoFade.FadeOut(0.5f);
                    //XTimerMgr.singleton.SetTimer(1.0f, _innerSwitchCb, para | 0x80000000);
                }
                else
                {
                    float delay = 0.3f/*XSceneMgr.singleton.GetSceneDelayTransfer(sceneID)*/;
                    if (delay > 0)
                    {
                        //XAutoFade.FadeOut(delay);
                        XTimerMgr.singleton.SetTimer(delay + 0.03f, _innerSwitchCb, para);
                    }
                    else
                        InnerSwitch(para);
                }
            }
        }

        private void InnerSwitch(object o)
        {
            switchScene = true;

            uint n = (uint)o;

            bool bTransfer = (n >> 31) > 0;
            uint sceneID = (n & 0xFFFF);

            EXStage eStage = (EXStage)((n >> 16) & 0x7FFF);

            //if (bTransfer) XAutoFade.MakeBlack();

            _old_stage = _stage;
            _stage = (_old_stage != null && eStage == _old_stage.Stage) ? _old_stage : CreateSpecifiecStage(eStage);

            if (_old_stage != null)
            {
                _old_stage.OnLeaveStage(_stage.Stage);
                if (sceneID != XScene.singleton.SceneID)
                {
                    _old_stage.OnLeaveScene(bTransfer);
                    if (XScene.singleton.SceneID > 0) XScene.singleton.OnLeaveScene(bTransfer);
                    XScene.singleton.LoadSceneAsync(sceneID, eStage, !bTransfer, bTransfer);
                }
                else
                {
                    OnEnterStage();
                    switchScene = false;
                }
            }
            else
            {
                XScene.singleton.LoadSceneAsync(sceneID, eStage, true, false);
            }
        }

        public void TriggerFps()
        {
#if DEBUG
            if (_calc_fps) XTimerMgr.singleton.SetGlobalTimer(_fps_interval, _fpsHandler, null);
#endif
        }

        private XStage CreateSpecifiecStage(EXStage estage)
        {
            switch (estage)
            {
                case EXStage.Login:return XStage.CreateSpecificStage<XLoginStage>();
                case EXStage.Hall:return XStage.CreateSpecificStage<XHallStage>();
                default: return null;
            }
        }

        private void OnEnterStage()
        {
            _stage.OnEnterStage(_old_stage != null ? _old_stage.Stage : EXStage.Null);
        }

        public void OnEnterScene(uint sceneid, bool bTransfer = false)
        {
            XScene.singleton.OnEnterScene(sceneid, bTransfer);
            _stage.OnEnterScene(sceneid, bTransfer);
            OnEnterStage();
        }

        private void CalculateFPS(object o)
        {
            float time = Time.realtimeSinceStartup;

            _fps = _fps_count / (time - _fps_real_time);
            _fpsCalcCount++;
            _fpsAcc += _fps;
            if (_fpsCalcCount >= 20)
            {
                _fpsAvg = _fpsAcc / _fpsCalcCount;
                _fpsCalcCount = 0;
                _fpsAcc = 0.0f;
            }

            //UI> 
            

            _fps_real_time = time;
            _fps_count = 0;
#if DEBUG
            XTimerMgr.singleton.SetGlobalTimer(_fps_interval, _fpsHandler, null);
#endif
        }
    }
}
