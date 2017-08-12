using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMainClient
{
    public abstract class Charcacter : BaseObject
    {
        public static new readonly uint uuID = XCommon.Singleton.XHash("Charcacter");
        public override uint ID { get { return uuID; } }

        StateFun _previous = null;
        StateFun _current = null;

        private IDictionary<int, StateFun> s_StateMap = new Dictionary<int, StateFun>();

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
