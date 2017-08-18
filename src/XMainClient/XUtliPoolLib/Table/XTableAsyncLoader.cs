using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace XUtliPoolLib
{
    public delegate void OnLoadedCallback();
    internal class XFileReadAsync
    {
        internal bool IsDone = false;
        internal string Location = null;

        internal TableReader Reader = null;
        internal XReader Data = null; 
    }

    public sealed class XTableAsyncLoader
    {
        private bool _executing = false;

        private List<XFileReadAsync> _task_list = new List<XFileReadAsync>();
        private OnLoadedCallback _call_back = null;
        private XFileReadAsync currentXfra = null;

        public static int currentThreadCount = 0;
        public static readonly int AsyncPerTime = 2;

        public bool IsDone
        {
            get
            {
                bool done = false;
                if(currentXfra == null)
                {
                    if (currentThreadCount < AsyncPerTime)
                    {
                        currentThreadCount++;
                    }
                    done = InnerExecute();
                }else if(currentXfra.IsDone)
                {
                    if (currentThreadCount < AsyncPerTime)
                    {
                        currentThreadCount++;
                    }
                    XReader.Return(currentXfra.Data);
                    currentXfra = null;
                    done = InnerExecute();
                }

                if (done)
                {
                    if (_call_back != null)
                    {
                        _call_back();
                        _call_back = null;
                    }

                    _executing = false;
                }
                return done;
            }
        }

        private bool InnerExecute()
        {
            if (_task_list.Count > 0)
            {
                if(currentThreadCount < 0)
                {
                    return false;
                }
                currentThreadCount--;
                currentXfra = _task_list[_task_list.Count - 1];
                _task_list.RemoveAt(_task_list.Count - 1);
                ReadFileAsync(currentXfra);
                return false;
            }
            return true;
        }

        public void AddTask(string location, TableReader reader, bool native = false)
        {
            XFileReadAsync fra = new XFileReadAsync();
            fra.Location = Application.dataPath + location;
            fra.Reader = reader;
            _task_list.Add(fra);
        }

        public bool Execute(OnLoadedCallback callback = null)
        {
            if (_executing) return false;

            _call_back = callback;
            _executing = true;

            InnerExecute();
            return true;
        }

        private void ReadFileAsync(XFileReadAsync xfra)
        {
            xfra.Data = XReader.Get();
            xfra.Data.Init(xfra.Location);

            ThreadPool.QueueUserWorkItem(delegate (object state)
            {
                try
                {
                    bool res = true;
                    res = xfra.Reader.ReadFile(xfra.Data);
                    if (!res)
                        XDebug.singleton.AddErrorLog("ReadFileAsync Failed in :",xfra.Location,xfra.Reader.error);

                }catch(Exception e)
                {
                    XDebug.singleton.AddErrorLog(e.Message," ReadFileAsync Failed in :", xfra.Location, xfra.Reader.error);
                }
                xfra.IsDone = true;
            });
        }
    }
}
