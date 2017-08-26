using System;
using UnityEngine;

namespace XMainClient
{
    public interface IXAIController { }

    class XAIController : MonoBehaviour,IXAIController
    {
        XCharcacter target = null;
        XPet host = null;

        public float think = 0.3f;
        public float lookup = 0.2f;

        private float looktimer = 0f;
        private float thinktimer = 0f;

        void Start()
        {
            looktimer = 0f;
            thinktimer = 0f;
        }

        public void SetSmartParam(float thinkT, float lookT)
        {
            think = thinkT;
            lookup = lookT;
        }

        public void SetTarget(XCharcacter character)
        {
            target = character;
        }
        public void SetHost(XPet character)
        {
            host = character;
        }

        void Update()
        {
            looktimer += Time.deltaTime;
            if(looktimer >= lookup)
            {
                looktimer = 0f;

                if(target != null && host != null)
                {
                    Vector3 pos = target.gameObject.transform.localPosition;
                    int posx = Mathf.RoundToInt(pos.x);
                    int posy = Mathf.RoundToInt(pos.y);
                    Vector3 curPos = host.gameObject.transform.localPosition;
                    int curPosX = Mathf.RoundToInt(curPos.x);
                    int curPosY = Mathf.RoundToInt(curPos.y);
                    if(Mathf.Abs(curPosX-posx)>=2 || Mathf.Abs(curPosY-posy)>=2)
                    {
                        thinktimer = 0f;

                        XEventMoveTo evt = XEventPool<XEventMoveTo>.GetEvent();
                        evt.Firer = host;
                        evt.PosX = posx;
                        evt.PosY = posy;
                        XEventMgr.singleton.FireEvent(evt);
                    }
                }
            }

            thinktimer += Time.deltaTime;
            if(thinktimer >= think)
            {
                thinktimer = 0f;

                if(host !=null)
                {
                    host.Think();
                }
            }

        }
    }
}
