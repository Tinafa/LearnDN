using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public delegate bool SliderValueChangeEventHandler(float val);
    public delegate bool SliderClickEventHandler(GameObject go);

    public interface IXUISlider : IXUIObject
    {
        float Value { get; set; }
        void RegisterValueChangeEventHandler(SliderValueChangeEventHandler eventHandler);
        void RegisterClickEventHandler(SliderClickEventHandler eventHandler);
    }
}
