using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachablePlatform : MonoBehaviour
{
    protected GameObject empty;
    protected List<GameObject> attachedobjects;
    protected List<Vector3> attachedobjectscales;
    protected bool counteractscaling = true;

    void Awake()
    {
        empty = new GameObject("empty");
        empty.transform.SetParent(this.transform);
        attachedobjects = new List<GameObject>();
        attachedobjectscales = new List<Vector3>();
        OnAwake();
    }

    void Start()
    {
        OnStart();
    }

    // -- overridable
    protected virtual void OnAwake()
    {

    }

    protected virtual void OnStart()
    {

    }

    protected virtual void IOnCollisionEnter(Collision collision)
    {

    }

    protected virtual void IOnCollisionExit(Collision collision)
    {

    }

    private void ObjectEnterInternal(GameObject obj)
    {
        if (obj != null && !attachedobjects.Contains(obj))
        {
            // -- remember object scale and set parent
            attachedobjectscales.Add(obj.transform.localScale);
            obj.transform.SetParent(empty.transform);

            // -- get index for this object and reset scale
            int idx = attachedobjects.Count;
            attachedobjects.Add(obj);

            if(counteractscaling)
                obj.transform.localScale = attachedobjectscales[idx];
        }
    }

    private void ObjectExitInternal(GameObject obj)
    {
        if (obj != null && attachedobjects.Contains(obj))
        {
            // -- set parent to nothing & grab index 
            obj.transform.SetParent(null);
            int idx = attachedobjects.IndexOf(obj);

            // -- remove and reset scale
            attachedobjects.Remove(obj);
            if(counteractscaling)
                obj.transform.localScale = attachedobjectscales[idx];
            attachedobjectscales.RemoveAt(idx);
        }
    }

    public void ObjectEnter(GameObject obj)
    {
        ObjectEnterInternal(obj);
    }

    public void ObjectExit(GameObject obj)
    {
        ObjectExitInternal(obj);
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        ObjectEnterInternal(obj);
        IOnCollisionEnter(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        GameObject obj = collision.gameObject;
        ObjectExitInternal(obj);
        IOnCollisionExit(collision);
    }
}
