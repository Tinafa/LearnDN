using System;
using System.Collections.Generic;

namespace XMainClient
{
    class XFSM
    {
        StateFun _previous = null;
        StateFun _current = null;
        StateFun _global = null;

        int _preState = -1;
        int _curState = -1;
        int _globalState = -1;

        private IDictionary<int, StateFun> s_StateMap = new Dictionary<int, StateFun>();

        public void RegisterState(int stateID, StateFun state)
        {
            s_StateMap.Add(stateID, state);
        }

        public void SetGlobalState(int stateID, StateFun state)
        {
            _global = state;
            _globalState = stateID;
            RegisterState(stateID, state);
        }

        public void UnRegisterState(int stateID)
        {
            if (s_StateMap.ContainsKey(stateID))
            {
                s_StateMap.Remove(stateID);
            }
        }

        public void CancelGlobalState(int stateID)
        {
            UnRegisterState(stateID);
            _global = null;
            _globalState = -1;
        }

        public void ChangeState(int stateID)
        {
            if (_current != null)
            {
                _current.Exit();
                _previous = _current;

                _preState = _curState;
            }

            if (s_StateMap.ContainsKey(stateID))
            {
                _current = s_StateMap[stateID];
                _current.Enter();

                _curState = stateID;
            }
        }

        public virtual void Update(float delta)
        {
            if (_global != null) _global.Update(delta);
            if (_current != null) _current.Update(delta);
        }

        public virtual void FixedUpdate()
        {
            if (_global != null) _global.FixedUpdate();
            if (_current != null) _current.FixedUpdate();
        }

        public void Clear()
        {
            s_StateMap.Clear();
            _previous = null;
            _current = null;
        }

        public void RevertToPreviousState()
        {
            ChangeState(_preState);
        }
    }
}
