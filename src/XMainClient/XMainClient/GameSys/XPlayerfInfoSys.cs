using System;
using System.Collections;
using System.Collections.Generic;
using XUtliPoolLib;
using System.Text;
using System.Xml;
using UnityEngine;
using System.IO;
using System.ComponentModel;

#if DEBUG
using System.Xml.Serialization;
#endif

namespace XMainClient
{
    /*
    [Serializable]
    public class XSaveData
    {
        public string Name = "River";
        public string Profession = "程序员";
        public int ProfessionIdx = 0;
        public long Time = 0;
        public int HP = 100;
        public int Satiety = 30;
        public int WeatherIdx = 0;
    }*/

    public class XPlayerfInfoSys : XSingleton<XPlayerfInfoSys> , IXSys
    {
        public bool Deprecated { get; set; }

        public XPlayerData PlayerData;

        public bool IsLoaded { get; set; }

        string path = "";

        public override bool Init()
        {
            XInterfaceMgr.singleton.AttachInterface<XPlayerfInfoSys>(XCommon.singleton.XHash("XPlayerfInfoSys"), this);
            PlayerData = new XPlayerData();

            path = Application.dataPath + "/Datas/PlayerData.xml";

            IsLoaded = true;

            return true;
        }

        public override void Uninit()
        {
 
        }

        public void UpdateActorPos(int poxX, int posY, bool isRight)
        {
            if (PlayerData == null) return;
            PlayerData.PosX = poxX;
            PlayerData.PosY = posY;
            PlayerData.IsRight = isRight;
        }


        /*
        public void CreateData()
        {
            if (!File.Exists(path))
            {
                //创建最上一层的节点。
                XmlDocument xml = new XmlDocument();
                //创建最上一层的节点。
                XmlElement root = xml.CreateElement("objects");
                //创建子节点
                XmlElement element = xml.CreateElement("PlayerData");
       
                XmlElement elementChild1 = xml.CreateElement("Name");
                elementChild1.InnerText = PlayerData.Name;
                element.AppendChild(elementChild1);

                XmlElement elementChild2 = xml.CreateElement("Profession");
                elementChild2.InnerText = PlayerData.Profession;
                element.AppendChild(elementChild2);

                XmlElement elementChild3 = xml.CreateElement("ProfessionIdx");
                elementChild3.InnerText = PlayerData.ProfessionIdx.ToString();
                element.AppendChild(elementChild3);

                XmlElement elementChild4 = xml.CreateElement("Time");
                elementChild4.InnerText = "0";
                element.AppendChild(elementChild4);

                XmlElement elementChild5 = xml.CreateElement("HP");
                elementChild5.InnerText = PlayerData.HP.ToString();
                element.AppendChild(elementChild5);

                XmlElement elementChild6 = xml.CreateElement("Satiety");
                elementChild6.InnerText = PlayerData.Satiety.ToString();
                element.AppendChild(elementChild6);


                XmlElement elementChild7 = xml.CreateElement("WeatherIdx");
                elementChild7.InnerText = PlayerData.WeatherIdx.ToString();
                element.AppendChild(elementChild7);

                root.AppendChild(element);

                xml.AppendChild(root);
                //最后保存文件
                xml.Save(path);
            }
        }

        public void UpdateData()
        {
            if (File.Exists(path))
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;//忽略文档里面的注释
                XmlReader reader = XmlReader.Create(path, settings);
                xmlDoc.Load(reader);
   
                XmlElement player = (XmlElement)xmlDoc.SelectSingleNode("/objects/PlayerData");
                player.GetElementsByTagName("Name").Item(0).InnerText = PlayerData.Name;
                player.GetElementsByTagName("Profession").Item(0).InnerText = PlayerData.Profession;
                player.GetElementsByTagName("ProfessionIdx").Item(0).InnerText = PlayerData.ProfessionIdx.ToString();
                player.GetElementsByTagName("Time").Item(0).InnerText = "1";
                player.GetElementsByTagName("HP").Item(0).InnerText = PlayerData.HP.ToString();
                player.GetElementsByTagName("Satiety").Item(0).InnerText = PlayerData.Satiety.ToString();
                player.GetElementsByTagName("WeatherIdx").Item(0).InnerText = PlayerData.WeatherIdx.ToString();

                reader.Close();
                xmlDoc.Save(path);
            }
        }

        void LoadData()
        {
            if (File.Exists(path))
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;//忽略文档里面的注释
                XmlReader reader = XmlReader.Create(path, settings);
                xmlDoc.Load(reader);

                XmlElement player = (XmlElement)xmlDoc.SelectSingleNode("/objects/PlayerData");
                PlayerData.Name = player.GetElementsByTagName("Name").Item(0).InnerText;
                PlayerData.Profession = player.GetElementsByTagName("Profession").Item(0).InnerText;
                PlayerData.ProfessionIdx = int.Parse(player.GetElementsByTagName("ProfessionIdx").Item(0).InnerText);
                //PlayerData.Time = long.Parse( player.GetElementsByTagName("Time").Item(0).InnerText);
                PlayerData.HP = int.Parse(player.GetElementsByTagName("HP").Item(0).InnerText);
                PlayerData.Satiety = int.Parse(player.GetElementsByTagName("Satiety").Item(0).InnerText);
                PlayerData.WeatherIdx = int.Parse(player.GetElementsByTagName("WeatherIdx").Item(0).InnerText);

                reader.Close();
                xmlDoc.Save(path);
            }
        }
       
        public static Dictionary<string, SItem> tableScriptMap;
        public void TestXMLRead()
        {

            tableScriptMap = new Dictionary<string, SItem>();
            for(int i=0;i< 5;i++)
            {
                SItem item = new SItem();
                item.name = "caozi" + i;
                item.desc = "desc" + i * i * i;
                item.count = 3 * i + 4;
                tableScriptMap.Add("AADD" + (i + 5), item);
            }
            

            SBagItem tableMap = null;
            string xml = Application.dataPath + "/Datas/TestItem.xml";
            try
            {
                System.Xml.Serialization.XmlSerializer formatter = new System.Xml.Serialization.XmlSerializer(typeof(SBagItem));
                using (FileStream reader = new FileStream(xml, FileMode.Open))
                {
                    tableMap = formatter.Deserialize(reader) as SBagItem;
                }
                if (tableMap != null)
                {
                    tableMap.sItem.Clear();
                    tableMap.bagSize = tableScriptMap.Count;
                    foreach (KeyValuePair<string, SItem> kvp in tableScriptMap)
                    {
                        SItem tsm = new SItem();
                        tsm.name = kvp.Value.name;
                        tsm.desc = kvp.Value.desc;
                        tsm.count = kvp.Value.count;

                        tableMap.designations.Add(kvp.Key);
                        tableMap.sItem.Add(tsm);
                    }
                    using (FileStream writer = new FileStream(xml, FileMode.Create))
                    {
                        //using Encoding
                        StreamWriter sw = new StreamWriter(writer, System.Text.Encoding.UTF8);
                        System.Xml.Serialization.XmlSerializerNamespaces xsn = new System.Xml.Serialization.XmlSerializerNamespaces();
                        //empty name spaces
                        xsn.Add(string.Empty, string.Empty);
                        formatter.Serialize(sw, tableMap, xsn);
                    }
                }
            }
            catch (Exception e)
            {
                XDebug.singleton.AddErrorLog(e.Message);
            }
        }

        [Serializable]
        public class SItem
        {
            [SerializeField]
            public string name = "";
            [SerializeField]
            public string desc = "";
            [SerializeField]
            public int count = 0;
        }
        [Serializable]
        public class SBagItem
        {
            [SerializeField, DefaultValueAttribute(100)]
            public int bagSize = 0;
            [SerializeField]
            public List<string> designations = new List<string>();
            [SerializeField]
            public List<SItem> sItem = new List<SItem>();
        }

        public void LoadXML()
        {
            //创建xml文档
            XmlDocument xml = new XmlDocument();
            XmlReaderSettings set = new XmlReaderSettings();
            set.IgnoreComments = true;//这个设置是忽略xml注释文档的影响。有时候注释会影响到xml的读取
            xml.Load(XmlReader.Create((Application.dataPath + "/data2.xml"), set));
            //得到objects节点下的所有子节点
            XmlNodeList xmlNodeList = xml.SelectSingleNode("objects").ChildNodes;

            //遍历所有子节点
            foreach (XmlElement xl1 in xmlNodeList)
            {
                if(xl1.GetAttribute("id")=="1")
                {
                    foreach(XmlElement xl2 in xl1.ChildNodes)
                    {
                        XDebug.singleton.AddLog(xl2.GetAttribute("name"), " = ",xl2.InnerText);
                    }
                }
            }
        }

        public void UpdateXML()
        {
            string path = Application.dataPath + "/data.xml";
            if (File.Exists(path))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                XmlNodeList xmlNodeList = xml.SelectSingleNode("objects").ChildNodes;
                foreach (XmlElement xl1 in xmlNodeList)
                {
                    if (xl1.GetAttribute("id") == "1")
                    {
                        //把messages里id为1的属性改为5
                        xl1.SetAttribute("id", "5");
                    }
                }
                xml.Save(path);
            }
        }

        public void CreateXML()
        {
            string path = Application.dataPath + "/data2.xml";
            if (!File.Exists(path))
            {
                //创建最上一层的节点。
                XmlDocument xml = new XmlDocument();
                //创建最上一层的节点。
                XmlElement root = xml.CreateElement("objects");
                //创建子节点
                XmlElement element = xml.CreateElement("messages");
                //设置节点的属性
                element.SetAttribute("id", "1");
                XmlElement elementChild1 = xml.CreateElement("contents");

                elementChild1.SetAttribute("name", "a");
                //设置节点内面的内容
                elementChild1.InnerText = "这就是你，你就是天狼";
                XmlElement elementChild2 = xml.CreateElement("mission");
                elementChild2.SetAttribute("map", "abc");
                elementChild2.InnerText = "去吧，少年，去实现你的梦想";
                //把节点一层一层的添加至xml中，注意他们之间的先后顺序，这是生成XML文件的顺序
                element.AppendChild(elementChild1);
                element.AppendChild(elementChild2);

                root.AppendChild(element);

                xml.AppendChild(root);
                //最后保存文件
                xml.Save(path);
            }

        }
        */
    }
}
