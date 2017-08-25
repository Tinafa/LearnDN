using System;
using System.Collections.Generic;
using System.Text;

namespace XUtliPoolLib
{
    public sealed class TalkTable : TableReader
    {
        public class RowData
        {
            public uint id;
            public string words;
        }

        public List<RowData> Table = new List<RowData>();

        protected override void ReadLine(XReader reader)
        {
            if (2 != reader.LineLength)
            {
                XDebug.singleton.AddErrorLog(string.Format("TalkTable Read Fialed: {0}", reader.LineLength));
                return;
            }
            RowData row = new RowData();

            columnno = 0; Read<uint>(reader, ref row.id, uintParse);
            columnno = 1; Read<string>(reader, ref row.words, stringParse);

            columnno = -1;
            Print(row);

            Table.Add(row);
        }

        void Print(RowData data)
        {
            XDebug.singleton.AddGreenLog(data.id.ToString(), data.words);
        }

        public string GetWord(uint key)
        {
            if (key < 0 || key >= Table.Count)
                return string.Empty;

            RowData data = BinarySearch(key);
            return data != null ? data.words : string.Empty;
        }

        private RowData BinarySearch(uint key)
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
    }
}
