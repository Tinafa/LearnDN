using System;
using System.Collections.Generic;
using XUtliPoolLib;

namespace XMainClient
{
    public enum EXStage : byte
    {
        Null = 0,
        Login,
        SelectChar,
        World,
        Hall
    }

    public abstract class XStage
    {
        protected EXStage _eStage = EXStage.Null;

        private bool _entered = false;

        public XStage(EXStage stage)
        {
            _eStage = stage;
        }

        public bool IsEntered { get { return _entered; } }

        public virtual void Play() { }

        public virtual void OnEnterStage(EXStage eOld)
        {
            _entered = true;
        }

        public virtual void OnLeaveStage(EXStage eNew)
        {
            _entered = false;

            UIManager.singleton.CloseAllUI();

            XTimerMgr.singleton.KillTimerAll();
        }

        public EXStage Stage
        {
            get { return _eStage; }
        }

        public virtual bool Initialize() { return true; }

        public virtual void PreUpdate(float fDeltaT)
        {
            XScene.singleton.PreUpdate(fDeltaT);
        }

        public virtual void Update(float fDeltaT)
        {
            XScene.singleton.Update(fDeltaT);
        }

        public void FixedUpdate()
        {
            XScene.singleton.FixedUpdate();
        }

        public virtual void PostUpdate(float fDeltaT)
        {
            XScene.singleton.PostUpdate(fDeltaT);
        }

        static public T CreateSpecificStage<T>() where T : new()
        {
            return new T();
        }

        public virtual void OnEnterScene(uint sceneid, bool transfer)
        {
            /*
             * LoadScene
             * XSceneLoader.loadscene
             * XGame.OnEnterScene
             *    |
             *    |
             *    this.OnEnterScene();
             */

            UIManager.singleton.OnEnterScene();
        }

        public virtual void OnLeaveScene(bool transfer)
        {
            XEntityMgr.singleton.OnLeaveScene();
            UIManager.singleton.OnLeaveScene(false);
        }
    }
}
