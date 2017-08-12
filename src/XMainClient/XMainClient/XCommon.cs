using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XUtliPoolLib;

namespace XMainClient
{
    class XCommon : XSingleton<XCommon>
    {
        public XCommon()
        {
            _idx = 5;
        }
        private int _idx = 0;

        private System.Random _random = new System.Random(DateTime.Now.Millisecond);

        private int _new_id = 0;

        public int New_id
        {
            get { return ++_new_id; }
        }

        public long UniqueToken
        {
            get { return DateTime.Now.Ticks + New_id; }
        }

        public StringBuilder sb = new StringBuilder();

        public uint XHash(string str)
        {
            if (str == null) return 0;

            uint hash = 0;
            for (int i = 0; i < str.Length; i++)
            {
                hash = (hash << _idx) + hash + str[i];
            }

            return hash;
        }

        public uint XHash(StringBuilder str)
        {
            if (str == null) return 0;

            uint hash = 0;
            for (int i = 0; i < str.Length; i++)
            {
                hash = (hash << _idx) + hash + str[i];
            }

            return hash;
        }

        public uint XHashLowerRelpaceDot(uint hash, string str)
        {
            if (str == null) return hash;

            for (int i = 0; i < str.Length; i++)
            {
                char c = char.ToLower(str[i]);
                if (c == '/' || c == '\\')
                    c = '.';
                hash = (hash << _idx) + hash + c;
            }

            return hash;
        }
        public uint XHash(uint hash, string str)
        {
            if (str == null) return hash;

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                hash = (hash << _idx) + hash + c;
            }

            return hash;
        }
    }
}
