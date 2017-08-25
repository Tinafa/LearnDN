using System;
using System.Collections.Generic;
using XMainClient;
using UILib;

namespace XMainClient.UI
{
    class XLoginDlg : DlgBase<XLoginDlg, XLoginBehaviour>
    {
        public override string fileName
        {
            get { return "Login/LoginDlg"; }
        }

        public override int layer
        {
            get { return 1; }
        }

        public override bool isMainUI
        {
            get { return true; }
        }

        public override bool autoload
        {
            get { return true; }
        }

        public XLoginDlg() : base()
        {

        }

        protected override void Init()
        {
            XLoginDocument.singleton.View = this;
        }

        protected override void OnLoad()
        {
            uiBehaviour.m_Login.RegisterLabelClickEventHandler(OnClickLogin);
        }

        protected override void OnUnload()
        {
            XLoginDocument.singleton.View = null;
        }

        protected override void OnShow()
        {
            base.OnShow();


        }

        protected override void OnHide()
        {
            base.OnHide();
        }

        private void OnClickLogin(IXUILabel label)
        {
            XStorageSys.singleton.LoadSaveDoc();

            XGame.singleton.SwitchTo(EXStage.Hall, 3);
        }
    }
}
