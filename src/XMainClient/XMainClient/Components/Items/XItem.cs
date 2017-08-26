using System.Collections.Generic;
using XUtliPoolLib;

namespace XMainClient
{
    public interface IXItem
    {
        int Count { get; set; }
    }

    public interface IXUse
    {
        bool Use(XAttributes host);
    }

    public abstract class XItem : IXItem
    {
        protected XAttributes host;

        private int count = 1;
        public int Count { get; set; }
    }

    public abstract class XUseItem : XItem, IXUse
    {
        protected List<XEffect> effcts;

        public void AddEffects(XEffect eft)
        {
            if (effcts == null)
            {
                effcts = ListPool<XEffect>.Get();
                effcts.Clear();
            }
            effcts.Add(eft);
        }

        public bool Use(XAttributes host)
        {
            if (host == null)
                return false;

            for(int i=0;i<effcts.Count;++i)
            {
                effcts[i].Enter(host);
                host.AddEftUpdate(effcts[i].Update);
            }

            return true;
        }

        public virtual void Release()
        {
            for(int i = effcts.Count; i>= 0;++i)
            {
                effcts[i].Release();
            }
            ListPool<XEffect>.Release(effcts);
        }
    }
}
