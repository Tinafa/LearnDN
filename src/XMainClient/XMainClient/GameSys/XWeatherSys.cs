using System;
using System.Collections.Generic;
using XUtliPoolLib;
using UnityEngine;
using System.ComponentModel;
using XMainClient.UI;

namespace XMainClient
{
    class XWeatherSys : XSingleton<XWeatherSys>,IXSys
    {
        public bool Deprecated { get; set; }

        private XWeatherData weatherParam;
        public XWeatherData WeatherState { get { return weatherParam; } }

        private bool isBegin = false;
        public bool IsBegin { get { return isBegin; } }

        public override bool Init()
        {
            XInterfaceMgr.singleton.AttachInterface<XWeatherSys>(XCommon.singleton.XHash("XWeatherSys"), this);
            isBegin = false;
            return true;
        }

        public override void Uninit()
        {
            
        }

        public void BeginWeatherSys()
        {
            XMainHallDlg.singleton.ShowWeather();
            isBegin = true;
        }

        public void SetWeatherState(XWeatherData data)
        {
            weatherParam = (XWeatherData)data.Clone();
        }

        public void GetNewdayWeather()
        {
            weatherParam = new XWeatherData();

            EXSeason eseason = XTimeSys.singleton.Today.Season;

            XSeason season = XSeason.Get(eseason);
            XWeather weather = season.GetRandomWeather();

            weatherParam.SeasonIdx = weather.SeasonIdx;
            weatherParam.WeatherIdx = weather.WeatherIdx;
            weatherParam.Temprature = weather.Temprature;
            weatherParam.Humidity = weather.Humidity;
            weatherParam.Illumination = weather.Illumination;
            weatherParam.Rainfall = weather.Rainfall;
        }
       
    }
}
