using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using int8 = System.Byte;
using int16 = System.Int16;
using int32 = System.Int32;

// =========================================================================================================
// -- Base flags interface for all flag sizes 
// =========================================================================================================
public interface IFlags<I>
{
    // -- sets a given bit to true
    void SetFlag(I idx);

    // -- sets a given bit to false
    void ClearFlag(I idx);

    // -- gets given bit value given index
    bool GetValue(I idx);

    // -- clears all flags to false
    void ClearFlags();

    // -- outputs number in base 2 format
    void OutputBinary();
}

// =========================================================================================================
// -- 8 bit flags
// =========================================================================================================
[System.Serializable]
public class Flags8<EnumType> : IFlags<int8>
{
    public int8 flags;

    // -- custom editor variables
    [HideInInspector]
    public EnumType[] enumvalues = new EnumType[8];
    [HideInInspector]
    public bool foldout = false;

    // -- default constructor, assign all flag values to false
    public Flags8()
    {
        flags = 0;
        Array vals = Enum.GetValues(typeof(EnumType));
        for(int i = 0; i < enumvalues.Length; ++i)
        {
            enumvalues[i] = (EnumType)vals.GetValue(i);
        }
    }

    // -- sets a given bit to true
    public void SetFlag(int8 idx)
    {
        int8 base10 = (int8)MathXT.Pow2(idx);
        flags = (int8)(flags | base10);
    }

    // -- sets a given bit to false
    public void ClearFlag(int8 idx)
    {
        int8 base10 = (int8)MathXT.Pow2(idx);
        int8 complement = (int8)~base10; 
        flags &= complement;
    }

    // -- gets given bit value given index
    public bool GetValue(int8 idx)
    {
        int8 base10 = (int8)MathXT.Pow2(idx);
        return !((int8)(flags & base10) == 0);
    }

    public void ClearFlags()
    {
        flags = 0;
    }

    public void OutputBinary()
    {
        Debug.Log(Convert.ToString(flags, toBase: 2));
    }

    public EnumType EnumValueAt(int8 idx)
    {
        return enumvalues[idx];
    }

    // -- static function to check any number's bits
    public static bool BitOn(int8 number, int8 bitidx)
    {
        int8 base10 = (int8)MathXT.Pow2(bitidx);
        return !((int8)(number & base10) == 0);
    }
}

// =========================================================================================================
// -- 16 bit flags
// =========================================================================================================
[System.Serializable]
public class Flags16<EnumType> : IFlags<int16>
{
    public int16 flags;

    // -- custom editor variables
    [HideInInspector]
    public EnumType[] enumvalues = new EnumType[16];
    [HideInInspector]
    public bool foldout = false;

    // -- default constructor, assign all flag values to false
    public Flags16()
    {
        flags = 0;
        Array vals = Enum.GetValues(typeof(EnumType));
        for (int i = 0; i < enumvalues.Length; ++i)
        {
            enumvalues[i] = (EnumType)vals.GetValue(i);
        }
    }

    // -- sets a given bit to true
    public void SetFlag(int16 idx)
    {
        int16 base10 = (int16)MathXT.Pow2(idx);
        flags = (int16)(flags | base10);
    }

    // -- sets a given bit to false
    public void ClearFlag(int16 idx)
    {
        int16 base10 = (int16)MathXT.Pow2(idx);
        int16 complement = (int16)~base10;
        flags &= complement;
    }

    // -- gets given bit value given index
    public bool GetValue(int16 idx)
    {
        int16 base10 = (int16)MathXT.Pow2(idx);
        return !((int16)(flags & base10) == 0);
    }

    public void ClearFlags()
    {
        flags = 0;
    }

    public void OutputBinary()
    {
        Debug.Log(Convert.ToString(flags, toBase: 2));
    }

    public EnumType EnumValueAt(int16 idx)
    {
        return enumvalues[idx];
    }

    // -- static function to check any number's bits
    public static bool BitOn(int16 number, int16 bitidx)
    {
        int16 base10 = (int16)MathXT.Pow2(bitidx);
        return !((int16)(number & base10) == 0);
    }
}

// =========================================================================================================
// -- 32 bit flags
// =========================================================================================================
[System.Serializable]
public class Flags32<EnumType> : IFlags<int32>
{
    public int32 flags;

    // -- custom editor variables
    [HideInInspector]
    public EnumType[] enumvalues = new EnumType[32];
    [HideInInspector]
    public bool foldout = false;

    // -- default constructor, assign all flag values to false
    public Flags32()
    {
        flags = 0;
        Array vals = Enum.GetValues(typeof(EnumType));
        for (int i = 0; i < enumvalues.Length; ++i)
        {
            enumvalues[i] = (EnumType)vals.GetValue(i);
        }
    }

    // -- sets a given bit to true
    public void SetFlag(int32 idx)
    {
        int32 base10 = (int32)MathXT.Pow2(idx);
        flags = (int32)(flags | base10);
    }

    // -- sets a given bit to false
    public void ClearFlag(int32 idx)
    {
        int32 base10 = (int32)MathXT.Pow2(idx);
        int32 complement = (int32)~base10;
        flags &= complement;
    }

    // -- gets given bit value given index
    public bool GetValue(int32 idx)
    {
        int32 base10 = (int32)MathXT.Pow2(idx);
        return !((int32)(flags & base10) == 0);
    }

    public void ClearFlags()
    {
        flags = 0;
    }

    public void OutputBinary()
    {
        Debug.Log(Convert.ToString(flags, toBase: 2));
    }

    public EnumType EnumValueAt(int32 idx)
    {
        return enumvalues[idx];
    }

    // -- static function to check any number's bits
    public static bool BitOn(int32 number, int32 bitidx)
    {
        int32 base10 = (int32)MathXT.Pow2(bitidx);
        return !((int32)(number & base10) == 0);
    }

    public static int32 GetNumPostSet(int32 number, int32 idx)
    {
        int32 base10 = (int32)MathXT.Pow2(idx);
        number = (int32)(number | base10);

        return number;
    }

    public static int32 GetNumPostClear(int32 number, int32 idx)
    {
        int32 base10 = (int32)MathXT.Pow2(idx);
        int32 complement = (int32)~base10;
        number &= complement;

        return number;
    }
}

public enum EFlagsType
{
    Default
}