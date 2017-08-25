using System;
using XUtliPoolLib;
using UnityEngine;
using System.Text;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace XMainClient
{
    [Serializable]
    public class XPlayerData : ICloneable
    {
        [SerializeField]
        public string Name = "River";

        [SerializeField]
        public string Profession = "程序员";

        [SerializeField]
        public int ProfessionIdx = 0;

        [SerializeField]
        public int HP = 100;

        [SerializeField]
        public int Satiety = 30;

        [SerializeField]
        public int PosX = 0;

        [SerializeField]
        public int PosY = 0;

        [SerializeField]
        public bool IsRight = true;

        public object Clone()
        {
            return (object)this.MemberwiseClone();
        }
    }

    [Serializable]
    public class XTimeData : ICloneable
    {
        [SerializeField]
        public long Time = 0;

        [SerializeField]
        public bool IsDay = true;

        public object Clone()
        {
            return (object)this.MemberwiseClone();
        }
    }

    [Serializable]
    public class XWeatherData : ICloneable
    {
        
        [SerializeField]
        public int SeasonIdx=0;

        [SerializeField]
        public int WeatherIdx=0;               

        [SerializeField]
        public float Temprature=0f;

        [SerializeField]
        public float Humidity=0f;

        [SerializeField]
        public float Illumination=0f;

        [SerializeField]
        public float Rainfall=.0f;

        public object Clone()
        {
            return (object)this.MemberwiseClone();
        }
    }

    [Serializable]
    public class XItemData : ICloneable
    {
        [SerializeField]
        public int Id=0;

        [SerializeField]
        public string Name="";

        [SerializeField]
        public bool CanEat=true;

        [SerializeField]
        public int Food=0;

        [SerializeField]
        public int Count=0;

        public object Clone()
        {
            return (object)this.MemberwiseClone();
        }
    }

    [Serializable]
    public class XBagData : ICloneable
    {
        public XBagData()
        {
            Capacity = 0;
            Items = new List<XItemData>();
        }

        [SerializeField]
        public int Capacity;

        [SerializeField]
        public List<XItemData> Items;

        public object Clone()
        {
            XBagData bag = new XBagData();
            bag.Capacity = this.Capacity;
            bag.Items = new List<XItemData>();
            for (int i = 0; i < this.Items.Count; ++i)
            {
                bag.Items.Add((XItemData)this.Items[i].Clone());
            }
            return bag;
        }
    }

    [Serializable]
    public class XRepertory : ICloneable
    {
        public XRepertory()
        {
            Capacity = 0;
            Items = new List<XItemData>();
        }

        [SerializeField]
        public int Capacity;

        [SerializeField]
        public List<XItemData> Items;

        public object Clone()
        {
            XRepertory repertory = new XRepertory();
            repertory.Capacity = this.Capacity;
            repertory.Items = new List<XItemData>();
            for(int i=0;i<this.Items.Count;++i)
            {
                repertory.Items.Add((XItemData)this.Items[i].Clone());
            }
            return repertory;
        }
    }

    [Serializable]
    public class XSaveDoc : ICloneable
    {

        public XSaveDoc()
        {
            PlayerData = new XPlayerData();
            TimeData = new XTimeData();
            WeatherData = new XWeatherData();
            BagData = new XBagData();
            RepertotyData = new XRepertory();
        }

        private XSaveDoc(XPlayerData player,XTimeData time,XWeatherData weather,XBagData bag,XRepertory repertory)
        {
            PlayerData = (XPlayerData)player.Clone();
            TimeData = (XTimeData)time.Clone();
            WeatherData = (XWeatherData)weather.Clone();
            BagData = (XBagData)bag.Clone();
            RepertotyData = (XRepertory)repertory.Clone();
        }


        [SerializeField]
        public XPlayerData PlayerData;

        [SerializeField]
        public XTimeData TimeData;

        [SerializeField]
        public XWeatherData WeatherData;

        [SerializeField]
        public XBagData BagData;

        [SerializeField]
        public XRepertory RepertotyData;

        public object Clone()
        {
            XSaveDoc Data = new XSaveDoc(this.PlayerData,this.TimeData,this.WeatherData,this.BagData,this.RepertotyData);
            return Data;
        }
    }

     public class XStorageSys : XSingleton<XStorageSys>, IXSys
    {
        public bool Deprecated { get; set; }

        string pathPrefix = "";

        private bool isLoadSave = false;
        public bool IsLoadSave { get { return isLoadSave; } private set { isLoadSave = value; } }

        XSaveDoc saveDoc;

        public XSaveDoc CurSaveDoc { get { return saveDoc; } }

        public override bool Init()
        {
            XInterfaceMgr.singleton.AttachInterface<XStorageSys>(XCommon.singleton.XHash("XStorageSys"), this);
            pathPrefix = Application.dataPath + "/Datas/Save/";

            isLoadSave = false;

            return true;
        }

        public override void Uninit()
        {
            saveDoc = null;
        }

        public void SaveDoc(int slot=0)
        {
            CreateSaveDoc(saveDoc, slot);
        }

        public void CreateSaveDoc(XSaveDoc doc,int slot=0)
        {
            string path = pathPrefix + (slot == 0 ? "AutoSave.xml" : string.Format("Save{0}.xml", slot));

            if (doc == null) doc = new XSaveDoc();
            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(XSaveDoc));
                using (FileStream writer = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    //using Encoding
                    StreamWriter sw = new StreamWriter(writer, System.Text.Encoding.UTF8);
                    System.Xml.Serialization.XmlSerializerNamespaces xsn = new System.Xml.Serialization.XmlSerializerNamespaces();
                    //empty name spaces
                    xsn.Add(string.Empty, string.Empty);
                    formatter.Serialize(sw, doc, xsn);
                }
            }
            catch
            {
                XDebug.singleton.AddErrorLog("CreateSaveDoc Error!");
            }
            
        }

        public void LoadSaveDoc(int slot=0)
        {
            string path = pathPrefix + (slot == 0 ? "AutoSave.xml" : string.Format("Save{0}.xml", slot));
            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(XSaveDoc));
                using (FileStream reader = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    saveDoc = formatter.Deserialize(reader) as XSaveDoc;
                    isLoadSave = true;
                }
            }
            catch
            {
                XDebug.singleton.AddErrorLog("LoadSaveDoc Error!");
            }
        }

        public void SaveCurrent(int slot = 0)
        {
            XSaveDoc Doc = saveDoc == null ? new XSaveDoc() : (XSaveDoc)saveDoc.Clone();

            Doc.PlayerData = XPlayerfInfoSys.singleton.PlayerData;

            if (XGameManager.instance.GameStarted)
            {
                Doc.TimeData = XTimeSys.singleton.TimeData;
                Doc.WeatherData = XWeatherSys.singleton.WeatherState;
            }
            CreateSaveDoc(Doc,slot);
        }
        
    }
}
