using System;
using System.Collections.Generic;

namespace XMainClient
{
    class XFSM
    {
        StateFun _previous = null;
        StateFun _current = null;

        private IDictionary<int, StateFun> s_StateMap = new Dictionary<int, StateFun>();



        public void RegisterState(int stateID, StateFun state)
        {
            s_StateMap.Add(stateID, state);
        }

        public void UnRegisterState(int stateID, StateFun state)
        {
            if (s_StateMap.ContainsKey(stateID))
            {
                s_StateMap.Remove(stateID);
            }
        }

        public void ChangeState(int stateID)
        {
            if (_current != null)
            {
                _current.Exit();
                _previous = _current;
            }

            if (s_StateMap.ContainsKey(stateID))
            {
                _current = s_StateMap[stateID];
                _current.Enter();
            }
        }

        public virtual void Update(float delta)
        {
            if (_current != null) _current.Update(delta);
        }

        public virtual void FixedUpdate()
        {
            if (_current != null) _current.FixedUpdate();
        }

        public void Clear()
        {
            s_StateMap.Clear();
            _previous = null;
            _current = null;
        }
    }
}
