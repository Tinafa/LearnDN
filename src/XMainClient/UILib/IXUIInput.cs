using System;
using System.Collections.Generic;
using System.Text;

namespace UILib
{
    public delegate void InputKeyTriggeredEventHandler(IXUIInput input, UnityEngine.KeyCode key);
    public delegate void InputSubmitEventHandler(IXUIInput input);
    public delegate void InputChangeEventHandler(IXUIInput input);

    public interface IXUIInput : IXUIObject
    {
        string GetText();
        void SetText(string strText);

        void SetCharacterLimit(int num);
        void SetDefault(string strText);
        string GetDefault();
        void RegisterKeyTriggeredEventHandler(InputKeyTriggeredEventHandler eventHandler);
        void RegisterSubmitEventHandler(InputSubmitEventHandler eventHandler);
        void RegisterChangeEventHandler(InputChangeEventHandler eventHandler);
        void selected(bool value);
    }
}
