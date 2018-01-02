using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace dc
{
    public class JsonFile
    {
        public static bool Read<T>(string full_path, ref T obj)
        {
            try
            {
                string str_text = File.ReadAllText(full_path);
                obj = JsonConvert.DeserializeObject<T>(str_text);
                return true;
            }
            catch (Exception e)
            {
                Log.Error("json 读取失败 file:" + full_path + ", error:" + e.ToString());
            }
            return false;
        }
    }
}
