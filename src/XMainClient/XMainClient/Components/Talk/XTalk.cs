using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using XUtliPoolLib;
using XMainClient.UI;

namespace XMainClient
{
    public class XTalk : MonoBehaviour
    {
        private Transform host;
        private float _frequency;
        private bool _say;


        private string allWords;
        private string curWords;

        private float _time;
        private float _speakSpeed = 3f;
        private float _interval = 1 / 3f;

        private void Awake()
        {
            _frequency = .0f;
            _say = false;
        }

        public void SetHost(Transform obj)
        {
            host = obj;
        }

        public void BeginSay(float freq)
        {
            _frequency = freq;
            _time = 0f;
            _speakSpeed = 3f;
            _interval = 1 / _speakSpeed;

            if (_frequency > .0f)
            {
                _say = true;
            }else
                _say = false;
            
        }

        private void Update()
        {
            if(_say)
            {
                _time += Time.deltaTime;
                if (_time > _frequency)
                {
                    _time = 0.0f;
                    StartSpeak();
                }
            }            
        }

        void StartSpeak()
        {
            string words = XTalkCenter.singleton.GetRandomWord();
            StartCoroutine(SpeakWords(words));
        }

        void StartSpeak(uint key)
        {
            //do//
        }

        IEnumerator SpeakWords(string word)
        {
            allWords = word;
            curWords = "";
            yield return null;

            int i = 1;
            while(i<=allWords.Length)
            {
                curWords = allWords.Substring(0, i);
                ShowWord();
                yield return new WaitForSeconds(_interval);
                i++;
            }
        }

        public void ShowWord()
        {
            //XDebug.singleton.AddGreenLog("SomeOne Speak: ", curWords);
            if (XMainHallDlg.singleton.IsVisible())
                XMainHallDlg.singleton.ShowSpeak(curWords);
        }
    }
}
