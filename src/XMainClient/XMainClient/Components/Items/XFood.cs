using System;
using System.Collections.Generic;
using XUtliPoolLib;

namespace XMainClient
{
    public class XApple : XUseItem
    {
        public XApple()
        {
            XEftAddHp eft = CommonObjectPool<XEftAddHp>.Get();
            eft.HpCount = 10;
            AddEffects(eft);
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
