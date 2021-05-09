using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// -- the sole purpose of this class is to call OnStart and OnAwake for persistent gameobjects
public class StartRunner : MonoBehaviour
{
    void Awake()
    {
        //GameManager.OnAwake();
    }

    void Start()
    {
        //GameManager.OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
