using UILib;

namespace XMainClient.UI
{
    class XMainHallBehaviour : DlgBehaviourBase
    {
        public override void Init()
        {
            base.Init();

            m_SpeakLabel = transform.Find("Speak").GetComponent("XUILabel") as IXUILabel;
        }

        public IXUILabel m_SpeakLabel;
    }
}
