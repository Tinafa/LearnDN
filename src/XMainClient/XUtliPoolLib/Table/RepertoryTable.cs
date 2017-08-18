using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XUtliPoolLib
{
    public class RepertoryTable : TableReader
    {
        public class RowData
        {
            public uint id;
            public string name;
            public Seq2ListRef<int> attr;
            public int count;
        }

        public List<RowData> Datas = new List<RowData>();

        protected override void ReadLine(XReader reader)
        {
            if (4 != reader.LineLength)
            {
                XDebug.singleton.AddErrorLog(string.Format("RepertoryTable Read Fialed: {0}", reader.LineLength));
                return;
            }
            RowData row = new RowData();

            columnno = 0; Read<uint>(reader, ref row.id, uintParse);
            columnno = 1; Read<string>(reader, ref row.name, stringParse);
            columnno = 2; ReadSeq<int>(reader, ref row.attr, intParse);
            columnno = 3; Read<int>(reader, ref row.count, intParse);

            columnno = -1;
            Print(row);
            Datas.Add(row);
        }

        void Print(RowData data)
        {
            XDebug.singleton.AddGreenLog(data.id.ToString(), data.name, data.attr[0].ToString());
        }
    }
}
