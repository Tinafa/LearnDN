using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMainClient
{
    public enum EState
    {
        Min = -1,
        Idle = 0,
        Move = 1,
        Climb = 2,
        Chop = 3,
        Operation = 4,
        MoveTo = 5,
        Max = 6,
    }

    abstract class IParam
    {
        int iInt;
        float fFloat;
        bool bBool;
    }

    public class StateFun
    {
        public delegate void Fun();
        public delegate void Funs(float delta);

        Fun _enterFun;
        Funs _updateFun;
        Fun _fixedFun;
        Fun _exitFun;

        public StateFun(Fun enterFun, Funs updateFun, Fun exitFun)
        {
            _enterFun = enterFun;
            _updateFun = updateFun;
            _exitFun = exitFun;
        }

        public StateFun(Fun enterFun, Funs updateFun,Fun fixedFun, Fun exitFun)
        {
            _enterFun = enterFun;
            _updateFun = updateFun;
            _fixedFun = fixedFun;
            _exitFun = exitFun;
        }

        public void Enter()
        {
            if (null != _enterFun)
                _enterFun();
        }

        public void Update(float delta)
        {
            if (null != _updateFun)
                _updateFun(delta);
        }

        public void FixedUpdate()
        {
            if (null != _fixedFun)
                _fixedFun();
        }

        public void Exit()
        {
            if (null != _exitFun)
                _exitFun();
        }
    }
}
