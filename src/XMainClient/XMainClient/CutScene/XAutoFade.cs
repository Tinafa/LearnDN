using System;
using System.Collections;
using System.Collections.Generic;
using XUtliPoolLib;
using UnityEngine;

namespace XMainClient
{
    public sealed class XAutoFade
    {
        private enum FadeType
        {
            ToBlack,
            ToClear
        }
        private static float _in = 0;
        private static bool _force_from_black = false;

        private static IEnumerator _fadeToBlack = null;
        private static IEnumerator _fadeToClear = null;

        public static void PostUpdate()
        {
            if (_fadeToBlack != null)
            {
                if (!_fadeToBlack.MoveNext())
                {
                    _fadeToBlack = null;
                }
            }
            else if (_fadeToClear != null)
            {
                if (!_fadeToClear.MoveNext())
                {
                    _fadeToClear = null;
                }
            }
        }

        public static void MakeBlack(bool stopall = false)
        {
            if (stopall) StopAll();
            XGameUI.singleton.SetOverlayAlpha(1);
        }

        public static void FadeOut2In(float In, float Out)
        {
            _in = In;
            FadeOut(Out);
        }

        public static void FadeIn(float duration, bool fromBlack = false)
        {
            StopAll();
            _force_from_black = fromBlack;

            Start(FadeType.ToClear, duration);
        }

        public static void FadeOut(float duration)
        {
            StopAll();
            Start(FadeType.ToBlack, duration);
        }

        public static void FastFadeIn()
        {
            StopAll();
            XGameUI.singleton.SetOverlayAlpha(0);
        }



        private static IEnumerator FadeToBlack(float duration)
        {
            float alpha = XGameUI.singleton.GetOverlayAlpha();

            float rate = 1 / duration;
            float progress = alpha;

            while (progress < 1.0f && XGameUI.singleton.GetOverlayAlpha() < 1.0f)
            {
                XGameUI.singleton.SetOverlayAlpha(Mathf.Lerp(0, 1, progress));
                progress += rate * (Time.timeScale != 0f ? Time.deltaTime / Time.timeScale : Time.unscaledDeltaTime);

                yield return null;
            }

            XGameUI.singleton.SetOverlayAlpha(1);

            if (_in > 0)
            {
                FadeIn(_in);
                _in = 0;
            }
        }

        private static IEnumerator FadeToClear(float duration)
        {
            float alpha = XGameUI.singleton.GetOverlayAlpha();

            if (_force_from_black)
            {
                alpha = 1;
                XGameUI.singleton.SetOverlayAlpha(1);
            }

            if (duration == 0)
            {
                alpha = 0;
            }
            else
            {
                float rate = 1 / duration;
                float progress = 1 - alpha;

                while (progress < 1.0f && XGameUI.singleton.GetOverlayAlpha() > 0)
                {
                    XGameUI.singleton.SetOverlayAlpha(Mathf.Lerp(1, 0, progress));
                    progress += rate * (Time.timeScale != 0f ? Time.deltaTime / Time.timeScale : Time.unscaledDeltaTime);

                    yield return null;
                }
            }

            XGameUI.singleton.SetOverlayAlpha(0);
        }

        private static void Start(FadeType type, float duration)
        {
            switch (type)
            {
                case FadeType.ToBlack:
                    {
                        if (_fadeToBlack == null)
                        {
                            _fadeToBlack = FadeToBlack(duration);
                        }
                    }
                    break;
                case FadeType.ToClear:
                    {
                        if (_fadeToClear == null)
                        {
                            _fadeToClear = FadeToClear(duration);
                        }
                    }
                    break;
            }
        }

        private static void StopAll()
        {
            _fadeToBlack = null;
            _fadeToClear = null;
        }
    }
}
