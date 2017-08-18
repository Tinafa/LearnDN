using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace XUtliPoolLib
{
    public enum XDebugColor
    {
        XDebug_None = 0,
        XDebug_Green = 1,
        XDebug_Yellow,
        XDebug_Red
    }

    public class XDebug : XSingleton<XDebug>
    {

        private bool _showLog = true;

        private StringBuilder _buffer = new StringBuilder();

        public void AddLog(string log1, string log2 = null, string log3 = null, string log4 = null, string log5 = null, string log6 = null, XDebugColor color = XDebugColor.XDebug_None)
        {
            if (_showLog)
            {
                _buffer.Length = 0;
                _buffer.Append(log1).
                    Append(log2).
                    Append(log3).
                    Append(log4).
                    Append(log5).
                    Append(log6);

                if (color == XDebugColor.XDebug_Green)
                {
                    _buffer.Insert(0, "<color=green>");
                    _buffer.Append("</color>");
                }

                if (color == XDebugColor.XDebug_Red)
                    Debug.LogError(_buffer);
                else if (color == XDebugColor.XDebug_Yellow)
                    Debug.LogWarning(_buffer);
                else
                    Debug.Log(_buffer);
            }
        }

        public void AddGreenLog(string log1, string log2 = null, string log3 = null, string log4 = null, string log5 = null, string log6 = null)
        {
            AddLog(log1, log2, log3, log4, log5, log6, XDebugColor.XDebug_Green);
        }

        public void AddErrorLog(string log1, string log2 = null, string log3 = null, string log4 = null, string log5 = null, string log6 = null)
        {
            AddLog(log1, log2, log3, log4, log5, log6, XDebugColor.XDebug_Red);
        }
    }
}
