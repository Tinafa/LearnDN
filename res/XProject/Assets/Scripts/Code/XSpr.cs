using System;
using System.Collections.Generic;
using UnityEngine;
using XMainClient;

namespace Assets.Scripts
{
    class XSpr : MonoBehaviour
    {
         public void OnChop()
        {
            XRole player =  XGameManager.instance.player;
            if (player != null)
            {
                //player.OnChopDamage();
            }
        }
    }
}
