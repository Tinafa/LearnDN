using System;
using System.Collections.Generic;

namespace XUtliPoolLib
{
    public class XInterfaceMgr : XSingleton<XInterfaceMgr>
    {
        private Dictionary<uint, IXInterface> _interfaces = new Dictionary<uint, IXInterface>();

        public T GetInterface<T>(uint key) where T :IXInterface
        {
            IXInterface value = null;
            _interfaces.TryGetValue(key, out value);
            return (T)value;
        }

        public T AttachInterface<T>(uint key, T value) where T : IXInterface
        {
            if (_interfaces.ContainsKey(key))
            {
                _interfaces[key].Deprecated = true;
                XDebug.singleton.AddLog("Duplication key for interface ", _interfaces[key].ToString(), " and ", value.ToString());
                _interfaces[key] = value;
            }
            else
                _interfaces.Add(key, value);

            _interfaces[key].Deprecated = false;

            return value;
        }

        public void DetachInterface(uint key)
        {
            if (_interfaces.ContainsKey(key))
            {
                _interfaces[key].Deprecated = true;
                _interfaces.Remove(key);
            }
        }

        public override void Uninit()
        {
            _interfaces.Clear();
        }
    }
}
