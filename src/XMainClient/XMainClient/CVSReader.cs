using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using XUtliPoolLib;

namespace XMainClient
{
    public class XBinaryReader
    {
        static class StringBuilderCache
        {
            private const int MAX_BUILDER_SIZE = 360;

            [ThreadStatic]
            private static StringBuilder CachedInstance;

            public static StringBuilder Acquire(int capacity = 16)
            {
                if(capacity <= MAX_BUILDER_SIZE)
                {
                    StringBuilder sb = StringBuilderCache.CachedInstance;
                    if(sb != null)
                    {
                        if(capacity <= sb.Capacity)
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
                if(sb.Capacity <= MAX_BUILDER_SIZE)
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

        public XBinaryReader()
        {
            UTF8Encoding encoding = new UTF8Encoding();
            m_decoder = encoding.GetDecoder();
        }

        private byte[] m_srcBuff = null;
        private int m_Length = 0;
        private int m_Position = 0;
        private TextAsset m_ta = null;

        private byte[] m_charBytes = null;
        private char[] m_charBuffer = null;
        private char[] m_singleChar = null;

        private const int MaxCharBytesSize = 128;
        private static int MaxCharsSize = 128;

        private Decoder m_decoder;

        public bool IsEof
        {
            get { return m_Length == m_Position; }
        }

        public static void Init()
        {
            StringBuilder sb = StringBuilderCache.Acquire();
            StringBuilderCache.Release(sb);
        }
        
        public static XBinaryReader Get()
        {
            return CommonObjectPool<XBinaryReader>.Get();
        }

        public static void Return(XBinaryReader reader, bool readShareResource = false)
        {
            if (reader != null)
            {
                reader.Close(readShareResource);
                CommonObjectPool<XBinaryReader>.Release(reader);
            }
        }

        public void Init(TextAsset ta)
        {
            m_ta = ta;
            InitByte(m_ta != null ? m_ta.bytes : null);
        }

        public void InitByte(byte[] buff)
        {
            m_srcBuff = buff;
            m_Position = 0;
            m_Length = m_srcBuff != null ? m_srcBuff.Length : 0;
        }

        public byte[] GetBuffer()
        {
            return m_srcBuff;
        }

        public int Seek(int offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    m_Position = offset > m_Length ? m_Length : offset;
                    break;
                case SeekOrigin.Current:
                    m_Position = (m_Position + offset) > m_Length ? m_Length : (m_Position+offset);
                    break;
                case SeekOrigin.End:
                    m_Position = (m_Length - offset) > m_Length ? m_Length : (m_Length - offset);
                    break;
            }
            return m_Position;
        }

        public void Close(bool readShareResource)
        {
            if(m_ta != null)
            {
                if(!readShareResource)
                    Resources.UnloadAsset(m_ta);
                m_ta = null;
            }
            Init(null);
        }

        public int GetPosition()
        {
            return m_Position;
        }

        public char ReadChar()
        {
            int charsRead = 0;
            int numBytes = 1;
            int posSav = 0;

            posSav = m_Position;

            if(m_charBytes == null)
            {
                m_charBytes = new byte[MaxCharBytesSize];
            }
            if(m_singleChar == null)
            {
                m_singleChar = new char[1];
            }

            while (charsRead == 0)
            {
                // We really want to know what the minimum number of bytes per char
                // is for our encoding.  Otherwise for UnicodeEncoding we'd have to
                // do ~1+log(n) reads to read n characters.
                // Assume 1 byte can be 1 char unless m_2BytesPerChar is true.

                int r = ReadByte();
                m_charBytes[0] = (byte)r;
                if (r == -1)
                    numBytes = 0;

                if (numBytes == 0)
                {
                    return char.MinValue;
                }

                try
                {
                    charsRead = m_decoder.GetChars(m_charBytes, 0, numBytes, m_singleChar, 0);
                }
                catch
                {
                    Seek((posSav - m_Position), SeekOrigin.Current);
                    throw;
                }
            }
            if(charsRead == 0)
                return char.MinValue;
            return m_singleChar[0];
        }

        public byte ReadByte()
        {
            if (m_srcBuff == null || m_Position + 1 > m_Length) return 0;

            int b = m_srcBuff[m_Position++];
            return (byte)b;
        }

        public int ReadInt32()
        {
            if (m_srcBuff == null) return 0;

            int pos = (m_Position += 4); // use temp to avoid ----
            if (pos > m_Length)
            {
                m_Position = m_Length;
                return 0;
            }
            return (int)(m_srcBuff[pos - 4] | m_srcBuff[pos - 3] << 8 | m_srcBuff[pos - 2] << 16 | m_srcBuff[pos - 1] << 24);
        }
        public uint ReadUInt32()
        {
            if (m_srcBuff == null) return 0;

            int pos = (m_Position += 4); // use temp to avoid ----
            if (pos > m_Length)
            {
                m_Position = m_Length;
                return 0;
            }
            return (uint)(m_srcBuff[pos - 4] | m_srcBuff[pos - 3] << 8 | m_srcBuff[pos - 2] << 16 | m_srcBuff[pos - 1] << 24);
        }

        public short ReadInt16()
        {
            if (m_srcBuff == null) return 0;

            int pos = (m_Position += 2); // use temp to avoid ----
            if (pos > m_Length)
            {
                m_Position = m_Length;
                return 0;
            }
            return (short)(m_srcBuff[pos - 2] | m_srcBuff[pos - 1] << 8);
        }
        public float ReadSingle()
        {

            if (m_srcBuff == null) return 0;

            int pos = (m_Position += 4); // use temp to avoid ----
            if (pos > m_Length)
            {
                m_Position = m_Length;
                return 0;
            }
            uint tmpBuffer = (uint)(m_srcBuff[pos - 4] | m_srcBuff[pos - 3] << 8 | m_srcBuff[pos - 2] << 16 | m_srcBuff[pos - 1] << 24);
            unsafe
            {
                return *((float*)&tmpBuffer);
            }
        }
        public double ReadDouble()
        {
            if (m_srcBuff == null) return 0;

            int pos = (m_Position += 8); // use temp to avoid ----
            if (pos > m_Length)
            {
                m_Position = m_Length;
                return 0;
            }
            uint lo = (uint)(m_srcBuff[pos - 8] | m_srcBuff[pos - 7] << 8 |
                m_srcBuff[pos - 6] << 16 | m_srcBuff[pos - 5] << 24);
            uint hi = (uint)(m_srcBuff[pos - 4] | m_srcBuff[pos - 3] << 8 |
                m_srcBuff[pos - 2] << 16 | m_srcBuff[pos - 1] << 24);

            ulong tmpBuffer = ((ulong)hi) << 32 | lo;
            unsafe
            {
                return *((double*)&tmpBuffer);
            }
        }

        public long ReadInt64()
        {
            if (m_srcBuff == null) return 0;

            int pos = (m_Position += 8); // use temp to avoid ----
            if (pos > m_Length)
            {
                m_Position = m_Length;
                return 0;
            }
            uint lo = (uint)(m_srcBuff[pos - 8] | m_srcBuff[pos - 7] << 8 |
                m_srcBuff[pos - 6] << 16 | m_srcBuff[pos - 5] << 24);
            uint hi = (uint)(m_srcBuff[pos - 4] | m_srcBuff[pos - 3] << 8 |
                m_srcBuff[pos - 2] << 16 | m_srcBuff[pos - 1] << 24);
            return (long)((ulong)hi) << 32 | lo;
        }
        public bool ReadBoolean()
        {
            if (m_srcBuff == null || m_Position + 1 > m_Length) return false;
            return (m_srcBuff[m_Position++] != 0);
        }
        public string ReadString()
        {
            if (m_srcBuff == null) return String.Empty;
            int currPos = 0;
            int stringLength;
            int readLength;
            int charsRead;

            // Length of the string in bytes, not chars
            stringLength = Read7BitEncodedInt();
            if (stringLength < 0)
            {
                XDebug.singleton.AddErrorLog("IO.IO_InvalidStringLen_Len");
                return String.Empty;
            }

            if (stringLength == 0)
            {
                return String.Empty;
            }

            if (m_charBytes == null)
            {
                m_charBytes = new byte[MaxCharBytesSize];
            }

            if (m_charBuffer == null)
            {
                m_charBuffer = new char[MaxCharsSize];
            }

            StringBuilder sb = null;
            do
            {
                readLength = ((stringLength - currPos) > MaxCharBytesSize) ? MaxCharBytesSize : (stringLength - currPos);
                int startPos = m_Position;
                int pos = (m_Position += readLength);
                if (pos > m_Length)
                {
                    m_Position = m_Length;
                    readLength = m_Position - startPos;
                }
                Array.Copy(m_srcBuff, startPos, m_charBytes, 0, readLength);
                if (readLength == 0)
                {
                    XDebug.singleton.AddErrorLog("Read String 0 length");
                    return String.Empty;
                }

                charsRead = m_decoder.GetChars(m_charBytes, 0, readLength, m_charBuffer, 0);

                if (currPos == 0 && readLength == stringLength)
                    return new String(m_charBuffer, 0, charsRead);

                if (sb == null)
                    sb = StringBuilderCache.Acquire(stringLength); // Actual string length in chars may be smaller.
                sb.Append(m_charBuffer, 0, charsRead);
                currPos += readLength;

            } while (currPos < stringLength);

            return StringBuilderCache.GetStringAndRelease(sb);
        }

        public void SkipString()
        {
            if (m_srcBuff == null) return;
            int currPos = 0;
            int stringLength;
            int readLength;

            // Length of the string in bytes, not chars
            stringLength = Read7BitEncodedInt();
            if (stringLength < 0)
            {
                XDebug.singleton.AddErrorLog("IO.IO_InvalidStringLen_Len");
                return;
            }

            if (stringLength == 0)
            {
                return;
            }

            do
            {
                readLength = ((stringLength - currPos) > MaxCharBytesSize) ? MaxCharBytesSize : (stringLength - currPos);
                int startPos = m_Position;
                int pos = (m_Position += readLength);
                if (pos > m_Length)
                {
                    m_Position = m_Length;
                    readLength = m_Position - startPos;
                }
                if (readLength == 0)
                {
                    XDebug.singleton.AddErrorLog("Read String 0 length");
                    return;
                }

                if (currPos == 0 && readLength <= stringLength)
                    return;

                currPos += readLength;

            } while (currPos < stringLength);

            return;
        }
        private int Read7BitEncodedInt()
        {
            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
            int count = 0;
            int shift = 0;
            byte b;
            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                // In a future version, add a DataFormatException.
                if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
                {
                    XDebug.singleton.AddErrorLog("Format_Bad7BitInt32");
                    return 0;
                }

                // ReadByte handles end of stream cases for us.
                b = ReadByte();
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return count;
        }
    }

    class CVSReader
    {
    }
}
