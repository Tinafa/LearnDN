using System;
using System.Collections.Generic;
using System.Threading;
using XUtliPoolLib;

namespace XMainClient
{
    public class XDocuments : XObject
    {
        delegate void AsyncLoadExecute(OnLoadedCallback notcallback = null);

        private struct AsyncLoadData
        {
            public AsyncLoadData(AsyncLoadExecute exe, OnLoadedCallback loadedcallback, XTableAsyncLoader loader)
            {
                Execute = exe;
                Loaded = loadedcallback;
                TableLoader = loader;
            }

            public AsyncLoadExecute Execute;
            public OnLoadedCallback Loaded;
            public XTableAsyncLoader TableLoader;
        }

        public XDocuments():base()
        {
            _async_data.Clear();
            _async_data.Add(new AsyncLoadData(XMainHallDocument.Execute, null, XMainHallDocument.AsyncLoader));
            _async_data.Add(new AsyncLoadData(XBasementDocument.Execute, null, XBasementDocument.AsyncLoader));
        }

        public void CtorLoad()
        {
            ThreadPool.SetMaxThreads(XTableAsyncLoader.AsyncPerTime, XTableAsyncLoader.AsyncPerTime);
            XTableAsyncLoader.currentThreadCount = XTableAsyncLoader.AsyncPerTime;
            AsyncLoadData[] loader = new AsyncLoadData[XTableAsyncLoader.AsyncPerTime];
            for (int i = 0; i < XTableAsyncLoader.AsyncPerTime; ++i)
            {
                loader[i] = _async_data[i];
                loader[i].Execute(loader[i].Loaded);
            }
            int processedCount = 0;
            int current = XTableAsyncLoader.AsyncPerTime;
            while (processedCount < _async_data.Count)
            {
                for(int i=0;i<XTableAsyncLoader.AsyncPerTime;++i)
                {
                    AsyncLoadData ald = loader[i];
                    if(ald.TableLoader != null && ald.TableLoader.IsDone)
                    {
                        processedCount++;
                        if(current < _async_data.Count)
                        {
                            ald = _async_data[current++];
                            ald.Execute(ald.Loaded);
                        }
                        else
                        {
                            ald.TableLoader = null;
                        }
                        loader[i] = ald;
                    }
                }
                Thread.Sleep(1);
            }
        }

        public override bool Initilize()
        {
            XComponentMgr.singleton.CreateComponent(this, XMainHallDocument.uuID);
            XComponentMgr.singleton.CreateComponent(this, XBasementDocument.uuID);
            return true;
        }

        public static T GetSpecificDocument<T>(uint uuID) where T : XComponent
        {
            return XGame.singleton.Doc.GetXComponent(uuID) as T;
        }

        private List<AsyncLoadData> _async_data = new List<AsyncLoadData>(64);
    }
}
