using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoolerUtilities
{
    // -- instance functionality
    public static GameObject SpawnMuzzleFlash(Transform parent, Vector3 position, Quaternion rotation, int overlaylayer)
    {
        ObjectPooler muzzleflashpooler = ObjectPooler.GetInstance(ObjectPoolerType.MuzzleFlash);

        GameObject muzzleflash = SpawnPooledObject(muzzleflashpooler, position, rotation);
        muzzleflash.transform.SetParent(parent);
        LayerUtilities.SetGameObjectLayer(muzzleflash, overlaylayer);

        return muzzleflash;
    }

    public static GameObject SpawnPooledObject (ObjectPooler pooler, Vector3 position)
    {
        return SpawnPooledObject(pooler, position, Quaternion.identity);
    }

    public static GameObject SpawnPooledObject(ObjectPooler pooler, Vector3 position, Quaternion rotation)
    {
        return SpawnPooledObject(pooler, null, position, rotation);
    }

    public static GameObject SpawnPooledObject(ObjectPooler pooler, Transform parent, Vector3 position, Quaternion rotation)
    {
        // -- use pooled decals to create projector prefab
        GameObject pooledobject = pooler.GetPooledObject();
        if (pooledobject != null)
        {
            // -- position and orient
            pooledobject.transform.SetParent(parent);
            pooledobject.transform.localPosition = Vector3.zero;
            pooledobject.transform.localRotation = Quaternion.identity;
            pooledobject.transform.position = position;
            pooledobject.transform.rotation = rotation;
            pooledobject.SetActive(true);
        }

        return pooledobject;
    }
}
