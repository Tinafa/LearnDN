using System;
using System.Collections.Generic;
using System.Text;
using XUtliPoolLib;
using Random = UnityEngine.Random;

namespace XMainClient
{
    class XTalkCenter : XSingleton<XTalkCenter>
    {
        private XTableAsyncLoader AsyncLoader=null;
        static TalkTable _reader = new TalkTable();

        public override bool Init()
        {
            if(AsyncLoader == null)
            {
                AsyncLoader = new XTableAsyncLoader();
                AsyncLoader.AddTask(@"/Resources/Table/Talk.txt", _reader);

                AsyncLoader.Execute();
            }            

            return AsyncLoader.IsDone;
        }

        public string GetRandomWord()
        {
            if(_reader.Table.Count > 0)
            {
                int index = Random.Range(0, _reader.Table.Count);
                return _reader.Table[index].words;
            }
            return "Hello World!";
        }
    }
}
