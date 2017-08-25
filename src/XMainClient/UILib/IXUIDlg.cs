using System;
using System.Collections.Generic;
using System.Text;

namespace UILib
{
    public interface IXUIDlg
    {
        IXUIBehaviour uiBehaviourInterface { get; }
        string fileName { get; }
        int layer { get; }
        int group { get; }
        bool exclusive { get; }
        bool hideMainMenu { get; }
        bool pushstack { get; }
        bool isMainUI { get; }
        
        int sysid { get; }
        bool fullscreenui { get; }
        void OnUpdate();
        void OnPostUpdate();
        void Load();
        void UnLoad(bool bTransfer = false);
        void SetVisiblePure(bool bVisible);
        void SetVisible(bool bVisible, bool bEnableAuto = true);
        bool IsVisible();
        void Reset();
        void SetDepthZ(int nDepthZ);
        void SetAlpha(float a);
        void StackRefresh();
        void LeaveStackTop();
    }

    public interface IXUIInterface
    {
        void ShowUI(string name);
        void HideUI(string name);

        void SetCustomId(string dlgName, string widgetName, uint ID);
    }
}
