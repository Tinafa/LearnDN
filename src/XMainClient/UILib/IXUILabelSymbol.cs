using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public delegate void HyperLinkClickEventHandler(string param);

    public delegate void LabelSymbolClickEventHandler(IXUILabelSymbol symbol);
    public interface IXUILabelSymbol : IXUIObject
    {
        string InputText { set; }
        IXUISprite IBoardSprite { get; }
        void UpdateDepth(int depth);
        void RegisterTeamEventHandler(HyperLinkClickEventHandler eventHandler);
        void RegisterGuildEventHandler(HyperLinkClickEventHandler eventHandler);
        void RegisterItemEventHandler(HyperLinkClickEventHandler eventHandler);
        void RegisterNameEventHandler(HyperLinkClickEventHandler eventHandler);
        void RegisterPkEventHandler(HyperLinkClickEventHandler eventHandler);
        void RegisterSpectateEventHandler(HyperLinkClickEventHandler eventHandler);
        void RegisterUIEventHandler(HyperLinkClickEventHandler eventHandler);
        void RegisterDefaultEventHandler(HyperLinkClickEventHandler eventHandler);
        void RegisterSymbolClickHandler(LabelSymbolClickEventHandler eventHandler);
    }
}
