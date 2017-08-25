using System;
using System.Collections.Generic;
using XUtliPoolLib;
using XMainClient.UI;

namespace XMainClient
{
    class XMainHallDocument : XDocComponent
    {
        public static new readonly uint uuID = XCommon.singleton.XHash("MainHallDocoment");
        public override uint ID { get { return uuID; } }


        private XMainHallDlg _view = null;
        public XMainHallDlg View { get { return _view; } set { _view = value; } }

        public static XTableAsyncLoader AsyncLoader = new XTableAsyncLoader();
        static ItemTable _ItemTable = new ItemTable();
        static FoodTable _FoodTable = new FoodTable();

        public ItemTable itemTable { get { return _ItemTable; } }
        public FoodTable foodTable { get { return _FoodTable; } }

        public static void Execute(OnLoadedCallback callback = null)
        {
            AsyncLoader.AddTask(@"/Resources/Table/Item.txt", _ItemTable);
            AsyncLoader.AddTask(@"/Resources/Table/Food.txt", _FoodTable);

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
