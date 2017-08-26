using System;
using UnityEngine;
using System.Collections.Generic;

namespace XMainClient
{
    public class XAttributes : MonoBehaviour
    {
        XPlayerData playerData;

        XPlayer host;

        public void SetHost(XPlayer player)
        {
            playerData = new XPlayerData();
            host = player;
        }

        public void AddEftUpdate(EffectUpdateHander handler)
        {
            if (host == null) return;

            host.AddEffects(handler);
        }

        public void AddHP(int hp)
        {
            int tHp = hp + playerData.HP;
            playerData.HP = tHp >= 100 ? 100 : tHp;
        }

        public void MinusHP(int hp)
        {
            int tHp = playerData.HP - hp;
            playerData.HP = tHp <= 0 ? 0 : tHp;

            //Die!
        }

    }
}
