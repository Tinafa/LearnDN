using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using XMainClient;
using XUtliPoolLib;

namespace Assets.Scripts.My
{
    class Test : MonoBehaviour
    {
        ArrayList infoall;

        private void Start()
        {
            string name = "test.txt";
            infoall = LoadFile(Application.dataPath, name);
            XDebug.singleton.AddGreenLog(Application.dataPath);
            Print(infoall);
        }

        private ArrayList LoadFile(string path, string name)
        {
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(path + "//Resources//" + name);
            }catch(Exception e)
            {
                XDebug.singleton.AddErrorLog("Read failed");
                return null;
            }
            string line;

            ArrayList arrList = new ArrayList();
            while((line= sr.ReadLine())!= null)
            {
                arrList.Add(line);
            }
            sr.Close();
            sr.Dispose();
            return arrList;
        }

        void Print(ArrayList arrList)
        {
            if(arrList==null)
            {
                XDebug.singleton.AddErrorLog("arrList == null");
                return;
            }
            XDebug.singleton.AddGreenLog("---------------------------------");
            for(int i=0;i<arrList.Count;i++)
            {
                XDebug.singleton.AddLog(arrList[i].ToString());
            }
            XDebug.singleton.AddGreenLog("---------------------------------");
        }

        private void OnGUI()
        {
            foreach (string str in infoall)
            {
                GUILayout.Label(str);
            }
        }
    }
}
