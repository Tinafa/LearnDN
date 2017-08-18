using System;
using System.Collections.Generic;
using XUtliPoolLib;
using UnityEngine;

namespace XMainClient
{
    class UIManager : XSingleton<UIManager>
    {
        private Transform m_uiRoot = null;
        public Transform UIRoot
        {
            get { return m_uiRoot; }
            set
            {
                m_uiRoot = value;
            }
        }

        public void CloseAllUI() { }
    }
}
