using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JSONMasterLoader
{
    public const string kJSONRoot = "json/";
    private static List<JSONRegistration<JSONRawValues, JSONValues>> registry = new List<JSONRegistration<JSONRawValues, JSONValues>>();

    public static RawType LoadRaw<RawType>(string fullpath) where RawType: JSONRawValues
    {
        string rawdata = File.ReadAllText(fullpath);
        RawType jsondata = JsonUtility.FromJson<RawType>(rawdata);

        return jsondata;
    }

    public static FullType ConvertRaw<RawType, FullType>(RawType rawdata) where RawType : JSONRawValues where FullType: JSONValues, new()
    {
        FullType converteddata = rawdata.Convert<FullType>(rawdata);
        return converteddata;
    }

    public static void RegisterTyping<RawData, Data>() where RawData : JSONRawValues where Data : JSONValues
    {
        JSONRegistration<RawData, Data> entry = new JSONRegistration<RawData, Data>();

        // -- check for previous matching entries
        foreach(var existingentry in registry)
        {
            if(existingentry.TypesMatch<RawData, Data>())
            {
                DebugXT.LogError("JSON registry already has an entry for types {0}:{1}", typeof(RawData), typeof(Data));
                return;
            }
        }

        // -- register
        registry.Add((entry as JSONRegistration<JSONRawValues, JSONValues>));
    }
}
