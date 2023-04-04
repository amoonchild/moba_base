using KVLib;
using KVLib.KeyValues;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public class BattleKvLibrary
    {
        protected int _version = 0;
        protected Dictionary<int, KeyValue> _keyValues = new Dictionary<int, KeyValue>();

        public int Version
        {
            get
            {
                return _version;
            }
        }
        public int Count
        {
            get
            {
                return _keyValues.Count;
            }
        }


        public bool Add(string text)
        {
            try
            {
                KeyValue rootKv = KVParser.ParseKeyValueText(text);
                if (rootKv != null && rootKv.HasChildren)
                {
                    foreach (var child in rootKv.Children)
                    {
                        if (child.Key == "Version")
                        {
                            _version = child.GetInt();
                            continue;
                        }

                        KeyValue kvSerialId = child["SerialId"];
                        if (kvSerialId == null)
                        {
                            Log.Error("{0} 中存在没有 SerialId 的项 {1}", rootKv.Key, child.Key);
                            continue;
                        }

                        int serialId = kvSerialId.GetInt();
                        if (_keyValues.ContainsKey(serialId))
                        {
                            Log.Error("{0} 中存在相同 SerialId 的项 {1}", rootKv.Key, child.Key);
                            continue;
                        }

                        _keyValues.Add(serialId, child);
                    }
                }

                return true;
            }
            catch (KeyValueParsingException e)
            {
                Log.Error(e.Message);
                return false;
            }
        }
        public bool HasValue(int serialId)
        {
            return _keyValues.ContainsKey(serialId);
        }
        public KeyValue GetValue(int serialId)
        {
            if (!_keyValues.ContainsKey(serialId))
            {
                return null;
            }

            return _keyValues[serialId];
        }
    }
}