using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // -- singleton properties
    public static Dictionary<ObjectPoolerType, ObjectPooler> instances = new Dictionary<ObjectPoolerType, ObjectPooler>();

    public static ObjectPooler GetInstance(ObjectPoolerType type)
    {
        return instances[type];
    }

    public static ObjectPooler DefaultInstance()
    {
        ObjectPoolerType firstkey = 0;

        return instances[firstkey];
    }

    // -- instance properties
    [Header("Active Entities")]
    public List<GameObject> pooledObjects;
    public List<GameObject> activeObjects;

    [Header("Pooler Settings")]
    public ObjectPoolerType poolertype;
    public GameObject objectToPool;
    public float deactivateTime = 5.0f;
    public int numberOfObjects;

    private void Awake()
    {
        Assert.Assert_(!instances.ContainsKey(poolertype), "Multiple instances of ObjectPooler with type {0} detected. " +
                                                           "Please only use 1 per type.", poolertype.ToString());

        instances[poolertype] = this;
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();

        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject obj = (GameObject)Instantiate(objectToPool);
            obj.SetActive(false);
            obj.AddComponent<RePoolObject>();
            obj.GetComponent<RePoolObject>().SetPoolerType(poolertype);
            obj.GetComponent<RePoolObject>().SetState(true, deactivateTime);
            SetParentObject(obj, gameObject);
            pooledObjects.Add(obj);
        }
    }

    public void SetParentObject(GameObject child, GameObject parent)
    {
        if (parent != null)
            child.transform.SetParent(parent.transform);
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)//$$$TODO this is causing errors with muzzle flash when mult. clients connected
            {
                activeObjects.Add(pooledObjects[i]);
                return pooledObjects[i];
            }
        }

        if (activeObjects.Count > 0)
        {
            GameObject returnObj = activeObjects[0];
            activeObjects[0].GetComponent<RePoolObject>().SetPoolerType(poolertype);
            activeObjects[0].GetComponent<RePoolObject>().Repool();
            activeObjects.Add(returnObj);
            return returnObj;
        }
        else
        {
            return null;
        }
    }

    public void DeactivateObject(GameObject obj)
    {
        if (activeObjects.Count > 0)
        {
            for (int i = 0; i < activeObjects.Count; i++)
            {
                if (obj == activeObjects[i])
                {
                    activeObjects.RemoveAt(i);
                    break;
                }
            }
        }
    }
}

public enum ObjectPoolerType
{
    Unassigned = -1,
    BulletHoles = 0,
    Footprints = 1,
    BulletTrails = 2,
    MuzzleFlash = 3,
    PlayerRagdoll = 4,
}