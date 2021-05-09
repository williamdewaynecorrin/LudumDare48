using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimation : MonoBehaviour
{
    [SerializeField]
    private bool startactivated = true;
    [SerializeField]
    private BasicAnimationEntry[] entries;

    private bool activated = false;

    void Awake()
    {
        if (startactivated)
            activated = true;

        foreach (BasicAnimationEntry entry in entries)
        {
            entry.initialscale = transform.localScale;
            entry.storedscale = transform.localScale;
            entry.initialpos = transform.position;
            entry.initialrot = transform.eulerAngles;

            entry.randomphase = Random.Range(-10f, 10f);
            entry.phase += entry.randomphase;

            if(!activated)
            {
                if (entry.type == BasicAnimationTarget.Scale)
                {
                    transform.localScale = Vector3.zero;
                    entry.storedscale = Vector3.zero;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (BasicAnimationEntry entry in entries)
        {
            if (activated)
                entry.phase += Time.deltaTime;

            switch (entry.type)
            {
                case BasicAnimationTarget.Scale:
                    ScaleLogic(entry);
                    break;
                case BasicAnimationTarget.PositionX:
                    PositionLogic(entry, Vector3.right);
                    break;
                case BasicAnimationTarget.PositionY:
                    PositionLogic(entry, Vector3.up);
                    break;
                case BasicAnimationTarget.PositionZ:
                    PositionLogic(entry, Vector3.forward);
                    break;
                case BasicAnimationTarget.RotationX:
                    RotationLogic(entry, Vector3.right);
                    break;
                case BasicAnimationTarget.RotationY:
                    RotationLogic(entry, Vector3.up);
                    break;
                case BasicAnimationTarget.RotationZ:
                    RotationLogic(entry, Vector3.forward);
                    break;
            }
        }
    }

    void ScaleLogic(BasicAnimationEntry entry)
    {
        if (activated)
        {
            entry.storedscale = Vector3.Lerp(entry.storedscale, entry.initialscale, Time.deltaTime * entry.lerpspeed);
        }
        else
        {
            entry.storedscale = Vector3.Lerp(entry.storedscale, Vector3.zero, Time.deltaTime * entry.lerpspeed);
            transform.localScale = entry.storedscale;
            return;
        }

        float sinval = Mathf.Sin(entry.phase * entry.period) * entry.scaleamt;
        transform.localScale = entry.storedscale + entry.initialscale * sinval;
    }

    void PositionLogic(BasicAnimationEntry entry, Vector3 dir)
    {
        if (!activated)
        {
            transform.position = Vector3.Lerp(transform.position, entry.initialpos, Time.deltaTime * entry.lerpspeed);
            return;
        }

        float sinval = Mathf.Sin(entry.phase * entry.period) * entry.moveamt;
        transform.position = entry.initialpos + dir * sinval;
    }

    void RotationLogic(BasicAnimationEntry entry, Vector3 axis)
    {
        if (!activated)
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, entry.initialrot, Time.deltaTime * entry.lerpspeed);
            return;
        }

        float sinval = Mathf.Sin(entry.phase * entry.period) * entry.rotateamt;
        transform.eulerAngles = entry.initialrot + axis * sinval;
    }

    private void ResetToInitial(BasicAnimationEntry entry)
    {
        transform.localScale = entry.storedscale;
        transform.position = entry.initialpos;
        transform.eulerAngles = entry.initialrot;
        entry.phase = entry.randomphase;
    }

    public void Activate(bool activate)
    {
        Activate(activate, true);
    }

    public void Activate(bool activate, bool reset)
    {
        activated = activate;
        foreach (BasicAnimationEntry entry in entries)
        {
            activated = activate;

            if (activated && reset)
                ResetToInitial(entry);
        }
    }

    public bool Activated()
    {
        return activated;
    }
}

[System.Serializable]
public class BasicAnimationEntry
{
    // -- universal
    [Header("Universal Attributes")]
    public BasicAnimationTarget type = BasicAnimationTarget.Scale;
    public float period = 1f;
    public bool randomizephase = false;
    public float lerpspeed = 10.0f;

    [HideInInspector]
    public float phase = 0f;
    [HideInInspector]
    public float randomphase = 0f;

    // -- specific attributs
    [Header("Type Attributes")]
    // -- scale
    [ConditionalHide("type", (int)(BasicAnimationTarget.Scale))]
    public float scaleamt = 1.1f;

    [HideInInspector]
    public Vector3 initialscale;
    [HideInInspector]
    public Vector3 storedscale;

    // -- position
    [ConditionalHide("type", new int[] {  (int)(BasicAnimationTarget.PositionX),
                                          (int)(BasicAnimationTarget.PositionY),
                                          (int)(BasicAnimationTarget.PositionZ)})]
    public float moveamt = 1f;

    [HideInInspector]
    public Vector3 initialpos;

    // -- rotation
    [ConditionalHide("type", new int[] {  (int)(BasicAnimationTarget.RotationX),
                                          (int)(BasicAnimationTarget.RotationY),
                                          (int)(BasicAnimationTarget.RotationZ)})]
    public float rotateamt = 10f;

    [HideInInspector]
    public Vector3 initialrot;
}

public enum BasicAnimationTarget
{
    Scale,
    PositionX,
    PositionY,
    PositionZ,
    RotationX,
    RotationY,
    RotationZ,
}