using System;
using System.Collections.Generic;
using UnityEngine;

namespace UILib
{
    public interface IXUIPlayTweenGroup
    {
        void PlayTween(bool bForward);
        void ResetTween(bool bForward);
        void StopTween();
    }
}
