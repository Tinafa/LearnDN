using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMainClient
{
    public interface IXController
    {

    }

    public class XPlayerController : MonoBehaviour, IXController
    {
        public XRole player = null;

        private void Start()
        {
            player = XGameManager.instance.player;
        }

        public void SetHost(XRole host)
        {
            player = host;
        }

        void Update()
        {
            if (player == null) return;

            float horizontal = 0;
            float vertical = 0;

#if true || UNITY_STANDALONE || UNITY_WEBPLAYER
            horizontal = (Input.GetAxisRaw("Horizontal"));
            vertical = (Input.GetAxisRaw("Vertical"));
            if (horizontal != 0)
            {
                vertical = 0;
            }
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            if (Input.touchCount > 0){
                Touch myTouch = Input.touches[0];
                if (myTouch.phase == TouchPhase.Began)
				{
					touchOrigin = myTouch.position;
				}
                else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					Vector2 touchEnd = myTouch.position;
					
					float x = touchEnd.x - touchOrigin.x;
					
					float y = touchEnd.y - touchOrigin.y;
					
					touchOrigin.x = -1;
					
					if (Mathf.Abs(x) > Mathf.Abs(y))
						horizontal = x > 0 ? 1 : -1;
					else
						vertical = y > 0 ? 1 : -1;
				}
            }
#endif
            XEventMove ev = XEventPool<XEventMove>.GetEvent();
            ev.Firer = player;
            ev.Horizontal = horizontal;
            ev.Vertical = vertical;

            XEventMgr.singleton.FireEvent(ev);
        }
    }
}
