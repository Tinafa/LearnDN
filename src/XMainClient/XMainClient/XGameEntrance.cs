using System;
using System.Collections.Generic;
using System.Collections;
using XUtliPoolLib;
using UnityEngine;

namespace XMainClient
{
    class XGameEntrance : IEntrance
    {
        private XGame _game = null;
        private IEnumerator _awake = null;
        private bool _be_awaked = false;

        public bool Deprecated { get; set; }

        public static void Fire()
        {
            XInterfaceMgr.singleton.AttachInterface<IEntrance>(0, new XGameEntrance());
        }

        public bool Awaked { get { return _be_awaked; } }

        public void Awake()
        {
            if (_game == null) _game = XGame.singleton;

            if (_awake == null)
            {
                _awake = _game.Awake();
            }
            else
            {
                if (!_awake.MoveNext())
                {
                    _awake = null;
                    _be_awaked = true;
                }
            }
        }

        public void Start()
        {
            if (!_game.Init())
            {
                Application.Quit();
            }
        }

        public void PreUpdate()
        {
            if (_game != null)
            {
                _game.PreUpdate(Time.deltaTime);
            }
        }

        public void Update()
        {
            if (_game != null)
            {
                _game.Update(Time.deltaTime);
            }
        }

        public void PostUpdate()
        {
            if (_game != null)
            {
                _game.PostUpdate(Time.deltaTime);
            }
        }

        public void FadeUpdate()
        {
            XAutoFade.PostUpdate();
        }

        public void Quit()
        {
            if (_game != null)
            {
                _game.Uninit();
                _game = null;
            }
        }

    }
}
