using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniCutscene : MonoBehaviour
{
    [SerializeField]
    private string cutscenename;
    [SerializeField]
    private Transform cutscene;
    [SerializeField]
    private float cutscenelength = 4f;
    [SerializeField]
    private float eventactivatetime = 2f;
    [SerializeField]
    private bool oneshot = true;
    [SerializeField]
    private UnityEvent oneventactivate;
    [SerializeField]
    private UnityEvent oneventdeactivate;

    bool cutsceneactivated = false;
    bool eventactivated = false;
    float prevactivetimer = 0f;
    float activetimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cutsceneactivated)
        {
            activetimer += Time.deltaTime;

            if(activetimer >= eventactivatetime && prevactivetimer < eventactivatetime)
            {
                if (eventactivated)
                    oneventactivate.Invoke();
                else
                    oneventdeactivate.Invoke();
            }
            if(activetimer >= cutscenelength && prevactivetimer < cutscenelength)
            {
                cutsceneactivated = false;
                activetimer = 0f;
                //GameManager.RemoveMiniCutscene();
            }

            prevactivetimer = activetimer;
        }
    }

    public void TriggerCutscene()
    {
        if (oneshot)
        {
            if (eventactivated)
                return;

            eventactivated = true;
            activetimer = 0f;
        }
        else
        {
            eventactivated = !eventactivated;
            activetimer = 0f;
        }

        cutsceneactivated = true;
        //GameManager.SetMiniCutscene(cutscene);
    }
}
