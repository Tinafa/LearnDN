using System;
using System.Collections.Generic;
using XUtliPoolLib;

namespace XMainClient.UI
{
    class XMainHallDlg : DlgBase<XMainHallDlg, XMainHallBehaviour>
    {

        private XMainHallDocument _mainDoc = null;
        public XMainHallDocument MainDoc { get { return _mainDoc; } }

        public override string fileName
        {
            get { return "MainHall/HallDlg"; }
        }

        public override int layer
        {
            get { return 1; }
        }

        public override bool isMainUI
        {
            get { return true; }
        }

        public override bool autoload
        {
            get { return true; }
        }

        public XMainHallDlg() : base()
        {

        }

        protected override void Init()
        {
            _mainDoc = XDocuments.GetSpecificDocument<XMainHallDocument>(XMainHallDocument.uuID);
            _mainDoc.View = this;
        }

        protected override void OnLoad()
        {

        }

        protected override void OnUnload()
        {
            _mainDoc.View = null;
        }

        protected override void OnShow()
        {
            base.OnShow();


        }

        protected override void OnHide()
        {
            base.OnHide();
        }

        public void ShowSpeak(string str)
        {
            m_uiBehaviour.m_SpeakLabel.SetText(str);
        }

        public void ShowWeather()
        {
            XWeatherData weather = XWeatherSys.singleton.WeatherState;

            if(weather != null)
            {
                XDebug.singleton.AddErrorLog("UI Weather：", weather.WeatherIdx.ToString(),"--" ,weather.Temprature.ToString());
            }
        }
    }
}
