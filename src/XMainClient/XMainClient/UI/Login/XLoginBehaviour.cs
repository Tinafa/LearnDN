using System;
using UILib;

namespace XMainClient.UI
{
    class XLoginBehaviour : DlgBehaviourBase
    {
        public override void Init()
        {
            base.Init();

            m_Login = transform.Find("Login").GetComponent("XUILabel") as IXUILabel;
        }

        public IXUILabel m_Login;
    }
}
