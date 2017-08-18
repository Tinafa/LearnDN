using System;
using System.Collections.Generic;
using XUtliPoolLib;


namespace XMainClient
{
    class XBasementDocument : XDocComponent
    {
        public static new readonly uint uuID = XCommon.singleton.XHash("BasementDocument");
        public override uint ID { get { return uuID; } }

        public static XTableAsyncLoader AsyncLoader = new XTableAsyncLoader();
        static RepertoryTable _RepertoryTable = new RepertoryTable();
        public RepertoryTable repertoryTable { get { return _RepertoryTable; } }

        public static void Execute(OnLoadedCallback callback = null)
        {
            AsyncLoader.AddTask(@"/Resources/Table/Repertory.txt", _RepertoryTable);

            AsyncLoader.Execute(callback);
        }

        public override void OnAttachToHost(XObject host)
        {
            base.OnAttachToHost(host);
        }

        protected override void EventSubscribe()
        {
            base.EventSubscribe();

        }
    }
}
