using System;
using XUtliPoolLib;

namespace XMainClient
{
    public delegate bool EffectUpdateHander(float delta);

    public interface IPool
    {
        IPool Get();

        void Release(IPool gb);
    }

    public interface IXEffect
    {
        void Enter(XAttributes host);

        bool Update(float delta);

        void End();
    }

    public abstract class XEffect : IXEffect
    {
        protected XAttributes host;
        protected bool bNeedUpdate = false;
        public void Enter(XAttributes thost)
        {
            host = thost;
        }

        public bool Update(float delta)
        {
            if (bNeedUpdate)
            {
                return OnUpdate(delta);
            }
            return false;
        }

        public void End()
        {
            OnEnd();
            host = null;
        }

        protected virtual void OnEnter() { }
        protected virtual bool OnUpdate(float delta) { return bNeedUpdate; }
        protected virtual void OnEnd() { }
        public abstract void Release();
    }
}
