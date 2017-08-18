using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMainClient
{
    public abstract class XCharcacter : XBaseObject
    {     
        StateFun _previous = null;
        StateFun _current = null;

        private IDictionary<int, StateFun> s_StateMap = new Dictionary<int, StateFun>();

        protected virtual void Start()
        {
            RegisterEvent(XEventDefine.XEvent_Idle, OnEventIdle);
            RegisterEvent(XEventDefine.XEvent_Move, OnEventMove);
            RegisterEvent(XEventDefine.XEvent_Chop, OnEventChop);
        }

        #region Events
        protected abstract bool OnEventIdle(XEventArgs e);

        protected abstract bool OnEventMove(XEventArgs e);

        protected abstract bool OnEventChop(XEventArgs e);
        #endregion

        protected void RegisterState(int stateID, StateFun state)
        {
            s_StateMap.Add(stateID, state);
        }

        protected void UnRegisterState(int stateID, StateFun state)
        {
            if(s_StateMap.ContainsKey(stateID))
            {
                s_StateMap.Remove(stateID);
            }
        }

        public void ChangeState(int stateID)
        {
            if(_current != null)
            {
                _current.Exit();
                _previous = _current;
            }

            if(s_StateMap.ContainsKey(stateID))
            {
                _current = s_StateMap[stateID];
                _current.Enter();
            }
        }

        protected virtual void Update()
        {
            if (_current != null) _current.Update(Time.deltaTime);
        }
    }
}
