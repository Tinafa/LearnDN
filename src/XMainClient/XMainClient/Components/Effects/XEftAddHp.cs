
using XUtliPoolLib;

namespace XMainClient
{
    public class XEftAddHp : XEffect
    {
        public int HpCount = 5;

        protected override void OnEnter()
        {
            if(host != null)
            {
                host.AddHP(HpCount);
            }
        }

        public override void Release()
        {
            CommonObjectPool<XEftAddHp>.Release(this);
        }
    }
}
