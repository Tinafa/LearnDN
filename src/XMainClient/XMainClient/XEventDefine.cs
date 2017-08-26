using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XUtliPoolLib;


namespace XMainClient
{
    public enum XEventDefine
    {
        XEvent_Invalid = -1,
        XEvent_Idle = 0,
        XEvent_Move = 1,
        XEvent_Jump = 2,
        XEvent_Fall = 3,
        XEvent_Chop = 4,
        XEvent_ChopDamage = 5,
        XEvent_MoveTo = 6,
    }

    public abstract class XEventArgs
    {
        protected long _token = 0;
        protected XBaseObject _firer = null;

        public XEventArgs()
        {
            _token = XCommon.singleton.UniqueToken;
            ManualRecycle = false;
        }

        public virtual void Recycle()
        {
            _token = 0;
            _firer = null;
        }

        protected XEventDefine _eDefine = XEventDefine.XEvent_Invalid;

        public bool ManualRecycle
        {
            get;
            set;
        }

        public XBaseObject Firer
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

    public class XEventIdle : XEventArgs
    {
        public XEventIdle()
        {
            _eDefine = XEventDefine.XEvent_Idle;
        }

        public override void Recycle()
        {
            base.Recycle();

            XEventPool<XEventIdle>.Recycle(this);
        }
    }

    public class XEventMove : XEventArgs
    {      

        public XEventMove()
        {
            _eDefine = XEventDefine.XEvent_Move;
        }

        public override void Recycle()
        {
            base.Recycle();
     
            Horizontal = 0;
            Vertical = 0;

            XEventPool<XEventMove>.Recycle(this);
        }

        public int Horizontal { get; set; }
        public int Vertical { get; set; }
    }

    public class XEventChop : XEventArgs
    {
        public XEventChop()
        {
            _eDefine = XEventDefine.XEvent_Chop;
        }

        public override void Recycle()
        {
            base.Recycle();
        }
    }

    public class XEventChopDamage : XEventArgs
    {
        public XEventChopDamage()
        {
            _eDefine = XEventDefine.XEvent_ChopDamage;
        }

        public override void Recycle()
        {
            base.Recycle();

            Attacker = null;
            DamageNum = 0;
        }

        public XActor Attacker { get; set; }
        public int DamageNum { get; set; }
    }

    public class XEventMoveTo : XEventArgs
    {

        public XEventMoveTo()
        {
            _eDefine = XEventDefine.XEvent_MoveTo;
        }

        public override void Recycle()
        {
            base.Recycle();

            PosX = 0;
            PosY = 0;

            XEventPool<XEventMoveTo>.Recycle(this);
        }

        public int PosX { get; set; }
        public int PosY { get; set; }
    }
}
