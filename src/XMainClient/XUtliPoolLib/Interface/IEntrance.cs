using System;
using System.Collections.Generic;

namespace XUtliPoolLib
{
    public interface IEntrance : IXInterface
    {
        bool Awaked { get; }

        void Awake();

        void Start();

        void PreUpdate();

        void Update();

        void PostUpdate();

        void FadeUpdate();

        void Quit();
    }
}
