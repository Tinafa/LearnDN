using System;
using System.Collections.Generic;
using XUtliPoolLib;

namespace XMainClient
{
    internal sealed class XScene : XSingleton<XScene>
    {
        private XSceneLoader _loader = null;

        private bool _bSceneEntered = false;
        private uint _scene_id = 0;

        public bool SceneReady { get { return _bSceneEntered; } }


        private bool _bStarted = false;
        public bool SceneStarted
        {
            get { return _bStarted; }
            set
            {
                if (_bStarted == value) return;

                _bStarted = value;
                if (_bStarted)
                {
                    OnSceneStarted();
                }
            }
        }

        public uint SceneID
        {
            get
            {
                return _scene_id;
            }
        }

        public override bool Init()
        {
            _loader = XGame.XGameRoot.AddComponent<XSceneLoader>();
            return true;
        }

        public void PreUpdate(float fDeltaT)
        {

        }
        public void Update(float fDeltaT)
        {
            if(_bSceneEntered)
            {
                XEntityMgr.singleton.Update(fDeltaT);
            }
        }

        public void PostUpdate(float fDeltaT)
        {
            if (_bSceneEntered)
            {
                XEntityMgr.singleton.PostUpdate(fDeltaT);
            }
        }

        public void FixedUpdate()
        {
            if (_bSceneEntered)
            {
                XEntityMgr.singleton.FixedUpdate();
            }
        }

        public void OnLeaveScene(bool transfer)
        {
            _bStarted = false;

            XShell.singleton.TimeMagicBack();

            XTimerMgr.singleton.KillTimerAll();
        }

        public void LoadSceneAsync(uint sceneid, EXStage eStage, bool progress, bool transfer)
        {
            string _scene_file = XSceneMgr.singleton.GetUnitySceneFile(sceneid);

            _bSceneEntered = false; 
            XEventMgr.singleton.Clear();

            _loader.LoadScene(_scene_file, eStage, progress, sceneid, _scene_id);
        }

        public void OnSceneBeginLoad(uint sceneid)
        {
            _scene_id = sceneid;

            _bStarted = false;
        }

        // this function will be called after only scene be loaded
        public void OnSceneLoaded(uint sceneid)
        {
        }

        public void OnEnterScene(uint sceneid, bool transfer)
        {

        }

        public void OnSceneStarted()
        {
            for (int i = 0; i < XGame.singleton.Doc.Components.Count; i++)
            {
                (XGame.singleton.Doc.Components[i] as XDocComponent).OnSceneStarted();
            }
        }
    }
}
