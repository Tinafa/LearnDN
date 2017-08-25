using System;
using System.Collections.Generic;

namespace XMainClient
{
    public static class XMsgCenter
    {
        //> 同步角色位置方向
        public static bool SendMsgUpdateActorPos(Object obj, int poxX, int posY, bool isRight)
        {
            if(XPlayerfInfoSys.singleton.IsLoaded)
            {
                XPlayerfInfoSys.singleton.UpdateActorPos(poxX, posY,isRight);
            }
            return true;
        }
        
    }
}
