using System;
using System.Collections.Generic;
using XUtliPoolLib;

namespace XMainClient
{
    public class XApple : XUseItem
    {
        public XApple()
        {
            AddEffects(CommonObjectPool<XEftAddHp>.Get());
        }
    }

    public class XRottenApple : XApple
    {
        public XRottenApple():base()
        {
            XEftBleed bleedEft = CommonObjectPool<XEftBleed>.Get();
            bleedEft.BleedSpeed = 2;
            AddEffects(bleedEft);
        }
    }
}
