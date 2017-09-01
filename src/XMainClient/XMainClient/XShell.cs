using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XUtliPoolLib;
using System.Threading;

namespace XMainClient
{
    public class XShell : XSingleton<XShell>
    {
        public static readonly int TargetFrame = 30;

        private float _time_scale = 1;

        private bool _bPause = false;
        private bool _bPauseTrigger = false;

        private IEntrance _entrance = null;

        private int _main_threadId = 0;
        public int ManagedThreadId { get { return _main_threadId; } }

        public bool Pause
        {
            get { return _bPause; }
            set
            {
                _bPauseTrigger = value;
            }
        }

        private bool isInitDone = false;
        public bool InitDone
        {
            get
            {
                return isInitDone;
            }
        }
        IEnumerator _launcher = null;

        public void Awake()
        {
            _main_threadId = Thread.CurrentThread.ManagedThreadId;

            //> Init
            XReader.Init();

            /*Assembly Main = null;
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer: Main = Assembly.Load("XMainClient"); break;
                case RuntimePlatform.Android:
                default: Main = (_script == null) ? Assembly.Load("XMainClient") : Assembly.Load(_script); break;
            }*/

        }

        IEnumerator LaunchGame()
        {
            MakeEntrance();
            PreLaunch();
            while (!Launched())
            {
                //Thread.Sleep(1);
                yield return null;
                Launch();
            }
            StartGame();
        }

        void MakeEntrance()
        {
            XGameEntrance.Fire();
            _entrance = XInterfaceMgr.singleton.GetInterface<IEntrance>(0);
        }

        public void PreLaunch()
        {
            if (_entrance !=null)
                _entrance.Awake();
        }

        public void Launch()
        {
            _entrance.Awake();
        }

        public bool Launched()
        {
            return _entrance.Awaked;
        }

        public void StartGame()
        {
            _entrance.Start();
        }

        public void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = -1;
        }

        public void PreUpdate()
        {
            if (Pause) return;

            _entrance.PreUpdate();
        }

        public void Update()
        {
            if(InitDone)
            {
                PreUpdate();
                if (Pause) return;

                if (XTimerMgr.singleton.update)
                    XTimerMgr.singleton.Update(Time.deltaTime);

                _entrance.Update();
            }
            else
            {
                if (_launcher == null)
                {
                    _launcher = LaunchGame();
                }
                else
                {
                    if (!_launcher.MoveNext())
                    {
                        _launcher = null;
                        isInitDone = true;
                    }
                }
                XTimerMgr.singleton.Update(Time.deltaTime);
            }
            
        }

        public void PostUpdate()
        {
            if (InitDone)
            {
                PauseChecker();
                _entrance.FadeUpdate();
                if (Pause) return;
                if (_entrance != null) _entrance.PostUpdate();
                XTimerMgr.singleton.PostUpdate();
            }
            
        }

        public void FixedUpdate()
        {
            if(InitDone)
            {
                _entrance.FixedUpdate();
            }
        }

        private void PauseChecker()
        {
            if (_bPause == _bPauseTrigger) return;

            _bPause = _bPauseTrigger;
            Time.timeScale = _bPause ? 0 : _time_scale;
        }

        public void Quit()
        {
            if (InitDone)
                _entrance.Quit();
        }

        /*public void MakeEntrance(Assembly main)
        {
            Type xEntrance = main.GetType("XMainClient.XGameEntrance");

            MethodInfo firer = xEntrance.GetMethod("Fire");
            firer.Invoke(null, null);

            _entrance = XInterfaceMgr.Singleton.GetInterface<IEntrance>(0);
        }*/

        public float TimeMagic(float value)
        {
            if (Time.timeScale == 1 && !_bPause)
            {
                Time.timeScale = value;
                _time_scale = value;
            }

            return Time.timeScale;
        }

        public void TimeMagicBack()
        {
            Time.timeScale = 1;
            _time_scale = 1;
        }

        public float CurrentTimeMagic { get { return Time.timeScale; } }
    }
}
