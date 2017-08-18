using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public interface IXUIBehaviour : IXUIObject
    {
        IXUIDlg uiDlgInterface{ get; }
        IXUIObject[] uiChilds { get; }
    }
}
