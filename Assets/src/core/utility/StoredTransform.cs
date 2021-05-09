using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoredTransform
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Vector3 localposition;
    public Quaternion localrotation;

    private bool reoriented = false;

    public StoredTransform(TransformOrientation orient = TransformOrientation.XYZ_Default)
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;
        scale = Vector3.one;
        localposition = Vector3.zero;
        localrotation = Quaternion.identity;
    }

    public StoredTransform(Transform from, TransformOrientation orient = TransformOrientation.XYZ_Default)
    {
        position = from.position;
        rotation = from.rotation;
        scale = from.localScale;
        localposition = from.localPosition;
        localrotation = from.localRotation;

        if(orient != TransformOrientation.XYZ_Default)
            OrientTransform(orient);
    }

    public StoredTransform(StoredTransform from, TransformOrientation orient = TransformOrientation.XYZ_Default)
    {
        position = from.position;
        rotation = from.rotation;
        scale = from.scale;
        localposition = from.localposition;
        localrotation = from.localrotation;

        if(orient != TransformOrientation.XYZ_Default)
            OrientTransform(orient);
    }

    private void OrientTransform(TransformOrientation orient)
    {
        Vector3 prevpos = position;
        Quaternion prevrot = rotation;
        Vector3 prevlocalpos = localposition;
        Quaternion prevlocalrot = localrotation;
        switch (orient)
        {
            case TransformOrientation.XYZ_Default:
                return;
            case TransformOrientation.YXZ:
                position.x = prevpos.y;
                position.y = prevpos.x;
                localposition.x = prevlocalpos.y;
                localposition.y = prevlocalpos.x;

                //$$TODO Scale

                rotation.x = prevrot.y;
                rotation.y = prevrot.x;
                localrotation.x = prevlocalrot.y;
                localrotation.y = prevlocalrot.x;
                return;
            case TransformOrientation.ZXY:
                position.x = prevpos.z;
                position.y = prevpos.x;
                position.z = prevpos.y;
                localposition.x = prevlocalpos.z;
                localposition.y = prevlocalpos.x;
                localposition.z = prevlocalpos.y;

                //$$TODO Scale

                rotation.x = prevrot.z;
                rotation.y = prevrot.x;
                rotation.z = prevrot.y;
                localrotation.x = prevlocalrot.z;
                localrotation.y = prevlocalrot.x;
                localrotation.z = prevlocalrot.y;
                return;
            case TransformOrientation.ZYX:
                position.x = prevpos.z;
                position.z = prevpos.x;
                localposition.x = prevlocalpos.z;
                localposition.z = prevlocalpos.x;

                //$$TODO Scale

                rotation.x = prevrot.z;
                rotation.z = prevrot.x;
                localrotation.x = prevlocalrot.z;
                localrotation.z = prevlocalrot.x;
                return;
        }

        reoriented = true;
    }

    public bool ReOriented()
    {
        return reoriented;
    }

    public void ReOrientTransform(TransformOrientation orient, bool erroronalreadyoriented = true)
    {
        if (erroronalreadyoriented && reoriented)
            DebugXT.LogError("Re-orienting a StoredTransform that previously has been oriented.");

        OrientTransform(orient);
    }
}

public enum TransformOrientation
{
    // -- default
    XYZ_Default = 0,

    // -- one swap
    YXZ = 1,
    ZYX = 2,
    XZY = 3,

    // all swapped
    ZXY = 4,
}