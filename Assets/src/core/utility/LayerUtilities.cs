using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerUtilities
{
    private const string kDefaultLayer = "Default";
    private const string kHideLayer = "HideFromIndex";
    private const string kExclusiveLayer = "OnlyIndex";
    private const string kOverlayLayer = "OverlayCamera";
    private const string kGroundLayer = "Ground";
    private const string kWeaponObjectLayer = "WeaponObject";

    public static int HiddenLayer(int idx)
    {
        if (idx < 1 || idx > 4)
            Debug.LogError("Hidden and exclusive layers only exist between indicies 1 and 4.");

        return LayerMask.NameToLayer(string.Format("{0}{1}", kHideLayer, idx));
    }

    public static int ExclusiveLayer(int idx)
    {
        if (idx < 1 || idx > 4)
            Debug.LogError("Hidden and exclusive layers only exist between indicies 1 and 4.");

        return LayerMask.NameToLayer(string.Format("{0}{1}", kExclusiveLayer, idx));
    }

    public static int OverlayLayer()
    {
        return LayerMask.NameToLayer(kOverlayLayer);
    }

    public static int GroundLayer()
    {
        return LayerMask.NameToLayer(kGroundLayer);
    }

    public static int WeaponObjectLayer()
    {
        return LayerMask.NameToLayer(kWeaponObjectLayer);
    }

    public static int DefaultLayer()
    {
        return LayerMask.NameToLayer(kDefaultLayer);
    }

    public static void SetGameObjectLayer(GameObject obj, int layer)
    {
        obj.layer = layer;
        for(int i = 0; i < obj.transform.childCount; ++i)
        {
            obj.transform.GetChild(i).gameObject.layer = layer;
        }
    }
}
