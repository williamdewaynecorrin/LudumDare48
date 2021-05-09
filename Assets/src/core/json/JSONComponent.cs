using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Type = System.Type;
using FieldInfo = System.Reflection.FieldInfo;
using PropertyInfo = System.Reflection.PropertyInfo;

public class JSONComponent<RawData, Data> where RawData: JSONRawValues where Data: JSONValues, new()
{
    private RawData rawdata;
    private Data data;
    private string filepath;

    public JSONComponent(string relativepath)
    {
        filepath = string.Format("{0}{1}", JSONMasterLoader.kJSONRoot, relativepath);
        rawdata = JSONMasterLoader.LoadRaw<RawData>(filepath);
        data = JSONMasterLoader.ConvertRaw<RawData, Data>(rawdata);
    }

    public RawData GetRawData()
    {
        return rawdata;
    }

    public Data GetData()
    {
        return data;
    }

    public void PrintRawFields()
    {
        DebugXT.LogMessage("Printing raw JSON data for JSON component {0}", this);
        rawdata.PrintFields();
    }

    public void PrintFields()
    {
        DebugXT.LogMessage("Printing JSON data for JSON component {0}", this);
        data.PrintFields();
    }
}

public class JSONRawValues
{
    public virtual ConvertedType Convert<ConvertedType>(JSONRawValues rawdata) where ConvertedType : JSONValues, new()
    {
        ConvertedType converteddata = new ConvertedType();
        object convertedobj = converteddata as object;
        EquivilencyConvert(ref convertedobj, rawdata);

        return convertedobj as ConvertedType;
    }

    private void EquivilencyConvert(ref object destobject, object sourceobject)
    {
        Type sourcetype = sourceobject.GetType();
        Type desttype = destobject.GetType();

        foreach (FieldInfo sourcefield in sourcetype.GetFields())
        {
            FieldInfo destfield = desttype.GetField(sourcefield.Name);
            if (destfield == null)
                continue;
            destfield.SetValue(destobject, sourcefield.GetValue(sourceobject));
        }

        foreach (PropertyInfo sourceprop in sourcetype.GetProperties())
        {
            PropertyInfo destprop = desttype.GetProperty(sourceprop.Name);
            if (destprop == null)
                continue;

            destprop.SetValue(destobject, sourceprop.GetValue(sourceobject, null), null);
        }
    }

    public void PrintFields()
    {
        Type sourcetype = this.GetType();
        foreach (FieldInfo sourcefield in sourcetype.GetFields())
        {
            FieldInfo destfield = sourcetype.GetField(sourcefield.Name);
            string name = destfield.Name;
            object value = destfield.GetValue(this);

            DebugXT.LogMessage("{0}: {1}", name, value);
        }
    }

    public void PrintProperties()
    {
        Type sourcetype = this.GetType();
        foreach (PropertyInfo sourceprop in sourcetype.GetProperties())
        {
            PropertyInfo destprop = sourcetype.GetProperty(sourceprop.Name);
            string name = destprop.Name;
            object value = destprop.GetValue(this);

            DebugXT.LogMessage("{0}: {1}", name, value);
        }
    }
}

public class JSONValues
{
    public void PrintFields()
    {
        Type sourcetype = this.GetType();
        foreach (FieldInfo sourcefield in sourcetype.GetFields())
        {
            FieldInfo destfield = sourcetype.GetField(sourcefield.Name);
            string name = destfield.Name;
            object value = destfield.GetValue(this);

            DebugXT.LogMessage("{0}: {1}", name, value);
        }
    }

    public void PrintProperties()
    {
        Type sourcetype = this.GetType();
        foreach (PropertyInfo sourceprop in sourcetype.GetProperties())
        {
            PropertyInfo destprop = sourcetype.GetProperty(sourceprop.Name);
            string name = destprop.Name;
            object value = destprop.GetValue(this);

            DebugXT.LogMessage("{0}: {1}", name, value);
        }
    }
}

public class JSONRegistration<RawData, Data> where RawData: JSONRawValues where Data: JSONValues
{
    public System.Type RawType()
    {
        return typeof(RawData);
    }

    public System.Type ConvertedType()
    {
        return typeof(Data);
    }

    public bool TypesMatch<OtherRaw, Other>() where OtherRaw : JSONRawValues where Other: JSONValues
    {
        return RawType() == typeof(OtherRaw) && ConvertedType() == typeof(Other);
    }
}