using System;
using System.Collections.Generic;
using UnityEngine;

namespace XUtliPoolLib
{
    public interface IXGameUI : IXInterface
    {
        void OnGenericClick();

        Transform UIRoot { get; set; }

        GameObject[] buttonTpl { get; }
        GameObject[] spriteTpl { get; }

        GameObject DlgControllerTpl { get; }

        void SetOverlayAlpha(float alpha);

        int Base_UI_Width { get; set; }
        int Base_UI_Height { get; set; }

        UnityEngine.Camera UICamera { get; set; }
    }
}
