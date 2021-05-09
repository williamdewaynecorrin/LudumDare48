using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SIComponentRef : MonoBehaviour
{
    public SIComponentRefType type = SIComponentRefType.Material;

    [ConditionalHide("type", (int)SIComponentRefType.Material)]
    public Material[] material;
    [ConditionalHide("type", (int)SIComponentRefType.Mesh)]
    public Mesh[] mesh;
    [ConditionalHide("type", (int)SIComponentRefType.Shader)]
    public Shader[] shader;
    [ConditionalHide("type", (int)SIComponentRefType.Sprite)]
    public Sprite[] sprite;
    [ConditionalHide("type", (int)SIComponentRefType.Texture2D)]
    public Texture2D[] texture2d;

    public Object SIGetRef()
    {
        switch(type)
        {
            case SIComponentRefType.Material:
                return material[0];
            case SIComponentRefType.Mesh:
                return mesh[0];
            case SIComponentRefType.Shader:
                return shader[0];
            case SIComponentRefType.Sprite:
                return sprite[0];
            case SIComponentRefType.Texture2D:
                return texture2d[0];
        }

        return null;
    }

    public Object[] SIGetRefs()
    {
        switch (type)
        {
            case SIComponentRefType.Material:
                return material;
            case SIComponentRefType.Mesh:
                return mesh;
            case SIComponentRefType.Shader:
                return shader;
            case SIComponentRefType.Sprite:
                return sprite;
            case SIComponentRefType.Texture2D:
                return texture2d;
        }

        return null;
    }

    public int SIRefLength()
    {
        switch (type)
        {
            case SIComponentRefType.Material:
                return material.Length;
            case SIComponentRefType.Mesh:
                return mesh.Length;
            case SIComponentRefType.Shader:
                return shader.Length;
            case SIComponentRefType.Sprite:
                return sprite.Length;
            case SIComponentRefType.Texture2D:
                return texture2d.Length;
        }

        return 0;
    }

    public bool Multiple()
    {
        return SIRefLength() > 1;
    }
}

public enum SIComponentRefType
{
    Material,
    Mesh,
    Shader,
    Sprite,
    Texture2D,
}