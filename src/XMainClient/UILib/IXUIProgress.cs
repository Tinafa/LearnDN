using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public interface IXUIProgress : IXUIObject
    {
        float value { get; set; }
        int width { get; set; }

        GameObject foreground { get; }
        void SetValueWithAnimation(float value);
        void SetTotalSection(uint section);
        void SetDepthOffset(int d);
        void SetForegroundColor(Color c);
        void ForceUpdate();
    }
}
