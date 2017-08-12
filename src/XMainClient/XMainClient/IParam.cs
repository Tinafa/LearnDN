using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMainClient
{
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
        Fun _exitFun;

        public void StateFunc(Fun enterFun, Funs updateFun, Fun exitFun)
        {
            _enterFun = enterFun;
            _updateFun = updateFun;
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

        public void Exit()
        {
            if (null != _exitFun)
                _exitFun();
        }
    }
}
