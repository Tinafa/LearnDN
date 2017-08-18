using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace XUtliPoolLib
{
    public class XReader
    {
        public static readonly char[] SequenceSeparator = new char[] { '=' };
        public static readonly char[] ListSeparator = new char[] { '|' };
        public static readonly char[] AllSeparators = new char[] { '|', '=' };
        public static readonly char[] SpaceSeparator = new char[] { ' ' };
        public static readonly char[] TabSeparator = new char[] { ' ', '\t' };

        static class StringBuilderCache
        {
            private const int MAX_BUILDER_SIZE = 360;

            [ThreadStatic]
            public static StringBuilder CachedInstance;

            public static StringBuilder Acquire(int capacity = 16)
            {
                if (capacity <= MAX_BUILDER_SIZE)
                {
                    StringBuilder sb = StringBuilderCache.CachedInstance;
                    if (sb != null)
                    {
                        if (capacity <= sb.Capacity)
                        {
                            StringBuilderCache.CachedInstance = null;
                            sb.Length = 0;
                            return sb;
                        }
                    }
                }
                return new StringBuilder(capacity);
            }

            public static void Release(StringBuilder sb)
            {
                if (sb.Capacity <= MAX_BUILDER_SIZE)
                {
                    StringBuilderCache.CachedInstance = sb;
                }
            }

            public static string GetStringAndRelease(StringBuilder sb)
            {
                string result = sb.ToString();
                Release(sb);
                return result;
            }
        }

        public XReader()
        {             
            m_encoder = new UTF8Encoding();
        }

        UTF8Encoding m_encoder = null;
        StreamReader m_sr = null;

        public static void Init()
        {
            StringBuilder sb = StringBuilderCache.Acquire();
            StringBuilderCache.Release(sb);
        }

        public static XReader Get()
        {
            return new XReader();
            //return CommonObjectPool<XReader>.Get();
        }

        public static void Return(XReader reader, bool readShareResource = false)
        {
            if (reader != null)
            {
                reader.Close(readShareResource);
                //CommonObjectPool<XReader>.Release(reader);
            }
        }

        public void Close(bool readShareResource)
        {
            m_sr.Close();
            m_sr.Dispose();
            m_sr = null;

            if (lineStrs!=null)
            {
                lineStrs.Clear();
                lineStrs = null;
            }
        }

        public void Init(string path)
        {           
            try
            {      
                m_sr = new StreamReader(path, m_encoder, true);
            }
            catch
            {
                XDebug.singleton.AddErrorLog("Init StreamReader Failed!",path);
            }
            lineStrs = new List<string>();
        }

        public bool EndOfStream
        {
            get
            {
                return m_sr==null? false : m_sr.EndOfStream;
            }
        }

        public float ReadSingle(int index)
        {
            if (Out(index)) return 0f;

            return ParseSingle(lineStrs[index]);
        }

        public double ReadDouble(int index)
        {
            if (Out(index)) return 0;

            return ParseDouble(lineStrs[index]);
        }

        public uint ReadUint(int index)
        {
            if (Out(index)) return 0;

            return ParseUInt(lineStrs[index]);
        }

        public int ReadInt(int index)
        {
            if (Out(index)) return 0;

            return ParseInt(lineStrs[index]);
        }

        public long ReadLong(int index)
        {
            if (Out(index)) return 0;

            return ParseLong(lineStrs[index]);
        }

        public string ReadString(int index)
        {
            if (Out(index)) return string.Empty;
            return lineStrs[index];
        }

        public bool ReadBool(int index)
        {
            if (Out(index)) return false;

            return ParseBool(lineStrs[index]);
        }

        public byte ReadByte(int index)
        {
            if (Out(index)) return (byte)0;

            return ParseByte(lineStrs[index]);
        }

        public short ReadShort(int index)
        {
            if (Out(index)) return (short)0;

            return ParseShort(lineStrs[index]);
        }
        public void ReadStringSeq(ref string[] ret, int index, int dim)
        {
            if (ret == null)
                ret = new string[dim];
            string line = ReadString(index);
            string[] str = line.Split(ListSeparator);
            if (str.Length == dim)
            {
                for (byte i = 0; i < dim; i++)
                {
                    ret[i] = str[i];
                }
            }
            else
            {
                XDebug.singleton.AddErrorLog("ReadStringSeq Failed");
            }
        }

        public void ReadSingleSeq(ref float[] ret, int index, int dim)
        {
            if(ret == null)
                ret = new float[dim];
            string line = ReadString(index);
            string[] str = line.Split(ListSeparator);
            if(str.Length == dim)
            {
                for(byte i=0;i<dim;i++)
                {
                    ret[i] = ParseSingle(str[i]);
                }
            }else
            {
                XDebug.singleton.AddErrorLog("ReadFloatSeq Failed");                
            }
        }

        public void ReadDoubleSeq(ref double[] ret, int index, int dim)
        {
            if (ret == null)
                ret = new double[dim];
            string line = ReadString(index);
            string[] str = line.Split(ListSeparator);
            if (str.Length == dim)
            {
                for (byte i = 0; i < dim; i++)
                {
                    ret[i] = ParseDouble(str[i]);
                }
            }
            else
            {
                XDebug.singleton.AddErrorLog("ReadDoubleSeq Failed");
            }
        }

        public void ReadUIntSeq(ref uint[] ret, int index, int dim)
        {
            if (ret == null)
                ret = new uint[dim];
            string line = ReadString(index);
            string[] str = line.Split(ListSeparator);
            if (str.Length == dim)
            {
                for (byte i = 0; i < dim; i++)
                {
                    ret[i] = ParseUInt(str[i]);
                }
            }
            else
            {
                XDebug.singleton.AddErrorLog("ReadUIntSeq Failed");
            }
        }

        public void ReadIntSeq(ref int[] ret, int index, int dim)
        {
            if (ret == null)
                ret = new int[dim];
            string line = ReadString(index);
            string[] str = line.Split(ListSeparator);
            if (str.Length == dim)
            {
                for (byte i = 0; i < dim; i++)
                {
                    ret[i] = ParseInt(str[i]);
                }
            }
            else
            {
                XDebug.singleton.AddErrorLog("ReadIntSeq Failed");
            }
        }

        public void ReadLongSeq(ref long[] ret, int index, int dim)
        {
            if (ret == null)
                ret = new long[dim];
            string line = ReadString(index);
            string[] str = line.Split(ListSeparator);
            if (str.Length == dim)
            {
                for (byte i = 0; i < dim; i++)
                {
                    ret[i] = ParseLong(str[i]);
                }
            }
            else
            {
                XDebug.singleton.AddErrorLog("ReadLongSeq Failed");
            }
        }

        public void ReadBoolSeq(ref bool[] ret, int index, int dim)
        {
            if (ret == null)
                ret = new bool[dim];
            string line = ReadString(index);
            string[] str = line.Split(ListSeparator);
            if (str.Length == dim)
            {
                for (byte i = 0; i < dim; i++)
                {
                    ret[i] = ParseBool(str[i]);
                }
            }
            else
            {
                XDebug.singleton.AddErrorLog("ReadBoolSeq Failed");
            }
        }

        public void ReadByteSeq(ref byte[] ret, int index, int dim)
        {
            if (ret == null)
                ret = new byte[dim];
            string line = ReadString(index);
            string[] str = line.Split(ListSeparator);
            if (str.Length == dim)
            {
                for (byte i = 0; i < dim; i++)
                {
                    ret[i] = ParseByte(str[i]);
                }
            }
            else
            {
                XDebug.singleton.AddErrorLog("ReadByteSeq Failed");
            }
        }

        public void ReadShortSeq(ref short[] ret, int index, int dim)
        {
            if (ret == null)
                ret = new short[dim];
            string line = ReadString(index);
            string[] str = line.Split(ListSeparator);
            if (str.Length == dim)
            {
                for (byte i = 0; i < dim; i++)
                {
                    ret[i] = ParseShort(str[i]);
                }
            }
            else
            {
                XDebug.singleton.AddErrorLog("ReadShortSeq Failed");
            }
        }

        float ParseSingle(string str)
        {
            float ret;
            if (float.TryParse(str, out ret))
            {
                return ret;
            }
            else
            {
                Error = "ReadFloat";
            }
            return 0f;
        }

        double ParseDouble(string str)
        {
            double ret;
            if (double.TryParse(str, out ret))
            {
                return ret;
            }
            else
            {
                Error = "ReadDouble";
            }
            return 0;
        }
        
        uint ParseUInt(string str)
        {
            uint ret;
            if (uint.TryParse(str, out ret))
            {
                return ret;
            }
            else
            {
                Error = "ReadUInt";
            }
            return 0;
        }

        int ParseInt(string str)
        {
            int ret;
            if (int.TryParse(str, out ret))
            {
                return ret;
            }
            else
            {
                Error = "ReadInt";
            }
            return 0;
        }

        long ParseLong(string str)
        {
            long ret;
            if (long.TryParse(str, out ret))
            {
                return ret;
            }
            else
            {
                Error = "ReadLong";
            }
            return 0;
        }

        bool ParseBool(string str)
        {
            if (str.Equals("1")
                || str.Equals("true")
                || str.Equals("True")
                || str.Equals("TRUE"))
                return true;
            else if (str.Equals("0")
                || str.Equals("false")
                || str.Equals("False")
                || str.Equals("FALSE"))
            {
                return false;
            }
            else
            {
                Error = "ReadBool";
                return false;
            }
        }

        byte ParseByte(string str)
        {
            byte ret;
            if (byte.TryParse(str, out ret))
            {
                return ret;
            }
            else
            {
                Error = "ReadByte";
            }
            return (byte)0;
        }

        short ParseShort(string str)
        {
            short ret;
            if (short.TryParse(str, out ret))
            {
                return ret;
            }
            else
            {
                Error = "ReadSHORT";
            }
            return (short)0;
        }

        string Error
        {
            set
            {
                XDebug.singleton.AddErrorLog("Parse Error!",value);
            }

        }

        bool Out(int index)
        {
            if (index >= lineStrs.Count || index < 0) return true;
            else return false;
        }

        public void ReadLine()
        {
            lineStrs.Clear();
            string line = m_sr.ReadLine();

            string[] str = line.Split(TabSeparator);
            for(int i=0;i< str.Length; i++)
            {
                lineStrs.Add(str[i]);
            }
        }

        public int LineLength
        {
            get
            {
                return lineStrs.Count;
            }
        }
        public List<string> LineData
        {
            get { return lineStrs; }
        }

        List<string> lineStrs;
    }

    public abstract class TableReader
    {
      
        public abstract class ValueParse<T>
        {
            public abstract void Read(XReader stream, ref T t, int index);

            public abstract void ReadSeq(XReader stream, ref T[] t, int index, int dim);

        }

        public sealed class FloatParse : ValueParse<float>
        {
            public override void Read(XReader stream, ref float t, int index)
            {
                t = stream.ReadSingle(index);
            }

            public override void ReadSeq(XReader stream, ref float[] t, int index, int dim)
            {
                stream.ReadSingleSeq(ref t, index, dim);
            }
        }

        public sealed class DoubleParse : ValueParse<double>
        {
            public override void Read(XReader stream, ref double t, int index)
            {
                t = stream.ReadDouble(index);
            }

            public override void ReadSeq(XReader stream, ref double[] t, int index, int dim)
            {
                stream.ReadDoubleSeq(ref t, index, dim);
            }
        }

        public sealed class UIntParse : ValueParse<uint>
        {
            public override void Read(XReader stream, ref uint t, int index)
            {
                t = stream.ReadUint(index);
            }

            public override void ReadSeq(XReader stream, ref uint[] t, int index, int dim)
            {
                stream.ReadUIntSeq(ref t, index, dim);
            }
        }

        public sealed class IntParse : ValueParse<int>
        {
            public override void Read(XReader stream, ref int t, int index)
            {
                t = stream.ReadInt(index);
            }

            public override void ReadSeq(XReader stream, ref int[] t, int index, int dim)
            {
                stream.ReadIntSeq(ref t, index, dim);
            }
        }

        public sealed class LongParse : ValueParse<long>
        {
            public override void Read(XReader stream, ref long t, int index)
            {
                t = stream.ReadLong(index);
            }

            public override void ReadSeq(XReader stream, ref long[] t, int index, int dim)
            {
                stream.ReadLongSeq(ref t, index, dim);
            }
        }

        public sealed class StringParse : ValueParse<string>
        {
            public override void Read(XReader stream, ref string t, int index)
            {
                t = stream.ReadString(index);
            }

            public override void ReadSeq(XReader stream, ref string[] t, int index, int dim)
            {
                stream.ReadStringSeq(ref t, index, dim);
            }
        }

        public sealed class BoolParse : ValueParse<bool>
        {
            public override void Read(XReader stream, ref bool t, int index)
            {
                t = stream.ReadBool(index);
            }

            public override void ReadSeq(XReader stream, ref bool[] t, int index, int dim)
            {
                stream.ReadBoolSeq(ref t, index, dim);
            }
        }

        public sealed class ByteParse : ValueParse<byte>
        {
            public override void Read(XReader stream, ref byte t, int index)
            {
                t = stream.ReadByte(index);
            }

            public override void ReadSeq(XReader stream, ref byte[] t, int index, int dim)
            {
                stream.ReadByteSeq(ref t, index, dim);
            }
        }

        public sealed class ShortParse : ValueParse<short>
        {
            public override void Read(XReader stream, ref short t, int index)
            {
                t = stream.ReadShort(index);
            }

            public override void ReadSeq(XReader stream, ref short[] t, int index, int dim)
            {
                stream.ReadShortSeq(ref t, index, dim);
            }
        }

        public bool ReadFile(XReader reader)
        {
            lineno = 0;
            columnno = -1;

            while(reader != null && !reader.EndOfStream)
            {
                PreReadLine(reader);
                if(!(lineno == 0 && skipFirstLine))
                {
                    ReadLine(reader);
                }
                    
                ++lineno;
            }
            return true;
        }

        void PreReadLine(XReader reader)
        {
            reader.ReadLine();
        }

        protected virtual void ReadLine(XReader reader)
        {
            
        }

        protected bool Read<T>(XReader stream, ref T v, ValueParse<T> parse)
        {
            parse.Read(stream, ref v, columnno);
            return true;
        }

        protected bool ReadSeq<T>(XReader stream, ref Seq2ListRef<T> v, ValueParse<T> parse)
        {
            parse.ReadSeq(stream, ref v.buffRef, columnno, v.Dim);
            return true;
        }

        protected bool ReadSeq<T>(XReader stream, ref Seq3ListRef<T> v, ValueParse<T> parse)
        {
            parse.ReadSeq(stream, ref v.buffRef, columnno, v.Dim);
            return true;
        }

        protected bool ReadSeq<T>(XReader stream, ref Seq4ListRef<T> v, ValueParse<T> parse)
        {
            parse.ReadSeq(stream, ref v.buffRef, columnno, v.Dim);
            return true;
        }

        protected bool ReadSeq<T>(XReader stream, ref Seq5ListRef<T> v, ValueParse<T> parse)
        {
            parse.ReadSeq(stream, ref v.buffRef, columnno, v.Dim);
            return true;
        }

        protected static FloatParse floatParse = new FloatParse();
        protected static DoubleParse doubleParse = new DoubleParse();
        protected static UIntParse uintParse = new UIntParse();
        protected static IntParse intParse = new IntParse();
        protected static LongParse longParse = new LongParse();
        protected static StringParse stringParse = new StringParse();
        protected static BoolParse boolParse = new BoolParse();
        protected static ByteParse byteParse = new ByteParse();
        protected static ShortParse shortParse = new ShortParse();

        public bool skipFirstLine = true;
        public int lineno = -1;
        public int columnno = -1;
        public string error
        {
            get
            {
                return " line: " + lineno.ToString() + " column: " + columnno.ToString();
            }
        }
    }

    public interface ISeqRef<T>
    {
        T this[int key] { get; }
        int Dim { get; }
    }

    public struct Seq2ListRef<T> : ISeqRef<T>
    {
        public T[] buffRef;

        public T this[int index]
        {
            get
            {
                return buffRef[index];
            }
        }

        public int Dim
        {
            get
            {
                return 2;
            }
        }
    }

    public struct Seq3ListRef<T> : ISeqRef<T>
    {
        public T[] buffRef;

        public T this[int index]
        {
            get
            {
                return buffRef[index];
            }
        }

        public int Dim
        {
            get
            {
                return 3;
            }
        }
    }

    public struct Seq4ListRef<T> : ISeqRef<T>
    {
        public T[] buffRef;

        public T this[int index]
        {
            get
            {
                return buffRef[index];
            }
        }

        public int Dim
        {
            get
            {
                return 4;
            }
        }
    }

    public struct Seq5ListRef<T> : ISeqRef<T>
    {
        public T[] buffRef;

        public T this[int index]
        {
            get
            {
                return buffRef[index];
            }
        }

        public int Dim
        {
            get
            {
                return 5;
            }
        }
    }
}
