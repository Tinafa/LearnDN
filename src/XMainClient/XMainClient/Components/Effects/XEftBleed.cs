using XUtliPoolLib;

namespace XMainClient
{
    public class XEftBleed : XEffect
    {
        public int BleedSpeed = 1;
        public float BleedTime = 10f;
        private float timer = 0f;

        public float BleedFreq = 0.2f;
        public float counter = 0f;

        protected override void OnEnter()
        {
            bNeedUpdate = true;
            timer = 0f;
        }
        protected override bool OnUpdate(float delta)
        {
            if (host == null)
                return false;
            if (!bNeedUpdate)
                return false;

            timer += delta;
            if(timer >= BleedTime)
            {
                bNeedUpdate = true;
                return false;
            }
            counter += delta;
            if (counter >= BleedFreq)
            {
                host.MinusHP(BleedSpeed);
            }
            return true;
        }

        public override void Release()
        {
            CommonObjectPool<XEftBleed>.Release(this);
        }
    }
}
