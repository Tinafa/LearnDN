using System;
using XUtliPoolLib;
using XMainClient.UI;

namespace XMainClient
{
    class XLoginDocument : XSingleton<XLoginDocument>
    {
        XLoginDlg _view = null;
        public XLoginDlg View { get { return _view; }set { _view = value; } }
    }
}
