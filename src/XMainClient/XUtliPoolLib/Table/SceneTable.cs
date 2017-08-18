using System;
using System.Collections.Generic;

namespace XUtliPoolLib
{
    public sealed class SceneTable : TableReader
    {
        public class RowData
        {
            public int id;
            public string name;
            public string scenefile;
        }

        public List<RowData> Table = new List<RowData>();

        public RowData GetBySceneID(int key)
        {
            if (Table == null || Table.Count == 0)
                return null;
            return BinarySearchSceneID(key);
        }

        private RowData BinarySearchSceneID(int key)
        {
            int min = 0;
            int max = Table.Count - 1;
            do
            {
                RowData minRow = Table[min];
                if (minRow.id == key)
                {
                    return minRow;
                }
                RowData maxRow = Table[max];
                if (maxRow.id == key)
                {
                    return maxRow;
                }
                if (max - min <= 1)
                {
                    return null;
                }
                int mid = min + (max - min) / 2;
                RowData midRow = Table[mid];
                if (midRow.id.CompareTo(key) > 0)
                {
                    max = mid;
                }
                else if (midRow.id.CompareTo(key) < 0)
                {
                    min = mid;
                }
                else
                {
                    return midRow;
                }
            } while (min < max);
            return null;
        }

        protected override void ReadLine(XReader reader)
        {
            if (3 != reader.LineLength)
            {
                XDebug.singleton.AddErrorLog(string.Format("SceneTable Read Fialed: {0}", reader.LineLength));
                return;
            }
            RowData row = new RowData();

            columnno = 0; Read<int>(reader, ref row.id, intParse);
            columnno = 1; Read<string>(reader, ref row.name, stringParse);
            columnno = 2; Read<string>(reader, ref row.scenefile, stringParse);

            columnno = -1;
            Print(row);

            Table.Add(row);
        }
        void Print(RowData data)
        {
            XDebug.singleton.AddGreenLog(data.id.ToString(), data.name, data.scenefile);
        }
    }
}
