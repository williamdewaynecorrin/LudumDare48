using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnTrigger : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onenter;
    [SerializeField]
    private bool oneshot = true;

    private bool invoked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (invoked && oneshot)
            return;

        //SpawnablePlayerBase p = other.gameObject.GetComponent<SpawnablePlayerBase>();
        //if(p != null)
        //{
        //    onenter.Invoke();
        //    invoked = true;
        //}
    }
}
