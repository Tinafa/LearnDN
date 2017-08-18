using System;
using System.Collections.Generic;
using System.Text;

namespace UILib
{
    public delegate bool CheckBoxOnCheckEventHandler(IXUICheckBox iXUICheckBox);

    public interface IXUICheckBox : IXUIObject
    {
        // Methods
        void RegisterOnCheckEventHandler(CheckBoxOnCheckEventHandler eventHandler);
        CheckBoxOnCheckEventHandler GetCheckEventHandler();
        void SetEnable(bool bEnable);

        // Properties
        bool bChecked { get; set; }
        void ForceSetFlag(bool bCheckd);
        void SetAlpha(float f);

        void SetAudioClip(string name);
        bool bInstantTween { get; set; }

        int spriteHeight { get; set; }
        int spriteWidth { get; set; }

        void SetGroup(int group);
    }
}
