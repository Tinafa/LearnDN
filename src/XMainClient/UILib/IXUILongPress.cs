using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public interface IXUILongPress : IXUIObject
    {
        void RegisterSpriteLongPressEventHandler(SpriteClickEventHandler eventHandler);
    }
}
