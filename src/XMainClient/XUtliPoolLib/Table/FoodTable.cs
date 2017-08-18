using System;
using System.Collections.Generic;

namespace XUtliPoolLib
{
    public class FoodTable : TableReader
    {
        public class RowData
        {
            public uint id;
            public string name;
        }

        public List<RowData> Datas = new List<RowData>();

        protected override void ReadLine(XReader reader)
        {
            if (2 != reader.LineLength)
            {
                XDebug.singleton.AddErrorLog(string.Format("FoodTable Read Fialed: {0}", reader.LineLength));
                return;
            }
            RowData row = new RowData();

            columnno = 0; Read<uint>(reader, ref row.id, uintParse);
            columnno = 1; Read<string>(reader, ref row.name, stringParse);

            columnno = -1;
            Print(row);
            Datas.Add(row);
        }

        void Print(RowData data)
        {
            XDebug.singleton.AddGreenLog(data.id.ToString(), data.name);
        }
    }
}
