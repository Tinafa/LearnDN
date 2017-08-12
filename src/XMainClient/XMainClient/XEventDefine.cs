using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMainClient
{
    public enum XEventDefine
    {
        XEvent_Invalid = -1,
        XEvent_Idle = 0,
        XEvent_Move = 1,
        XEvent_Jump = 2,
        XEvent_Fall = 3,
    }

    public abstract class XEventArgs
    {
        protected long _token = 0;
        protected XObject _firer = null;

        public XEventArgs()
        {
            _token = XCommon.Singleton.UniqueToken;
        }

        protected XEventDefine _eDefine = XEventDefine.XEvent_Invalid;

        public XObject Firer
        {
            get { return _firer; }
            set { _firer = value; }
        }

        public XEventDefine ArgsDefine
        {
            get { return _eDefine; }
        }

        public long Token
        {
            get { return _token; }
            set { _token = value; }
        }
    }
}
