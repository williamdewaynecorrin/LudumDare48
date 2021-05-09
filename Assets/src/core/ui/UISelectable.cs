using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelectable<T> : MonoBehaviour
{
    [SerializeField]
    protected float activatedscale = 1.25f;

    // -- activation
    protected float initialscale;
    protected T exposedvalue;
    protected bool activated = false;

    // -- input
    protected XboxController controller;
    protected OutOfBoundsInput lastframeinput = OutOfBoundsInput.InBounds;
    protected float prevhoriz;
    protected float prevvert;

    // -- unity functions
    void Awake()
    {
        initialscale = transform.localScale.x;
        OnAwake();
    }

    void Start()
    {
        controller = InputManager.instance.GetController(1);
    }

    void Update()
    {
        lastframeinput = OutOfBoundsInput.InBounds;
        float horiz = controller.GetHorizontalAxis();
        float vert = controller.GetVerticalAxis();

        if (!activated)
        {
            prevhoriz = horiz;
            prevvert = vert;
            return;
        }

        if (horiz > InputManager.DEADZONETHRESHOLD && prevhoriz < InputManager.DEADZONETHRESHOLD)
            lastframeinput = OutOfBoundsInput.OutRight;
        else if (horiz < -InputManager.DEADZONETHRESHOLD && prevhoriz > -InputManager.DEADZONETHRESHOLD)
            lastframeinput = OutOfBoundsInput.OutLeft;
        else if (vert > InputManager.DEADZONETHRESHOLD && prevvert < InputManager.DEADZONETHRESHOLD)
            lastframeinput = OutOfBoundsInput.OutUp;
        else if (vert < -InputManager.DEADZONETHRESHOLD && prevvert > -InputManager.DEADZONETHRESHOLD)
            lastframeinput = OutOfBoundsInput.OutDown;

        UICustomUpdate(horiz, vert);

        prevhoriz = horiz;
        prevvert = vert;
    }

    // -- public functions for base class
    public void Activate(bool activate)
    {
        activated = activate;
        OnActivated(activated);
    }

    public bool Activated()
    {
        return activated;
    }

    public OutOfBoundsInput LastInput()
    {
        return lastframeinput;
    }

    // -- overridable functions
    public virtual T GetValue()
    {
        return exposedvalue;
    }

    public virtual int GetIndexValue()
    {
        return 0;
    }

    public virtual void SetIndexValue(int index)
    {

    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void UICustomUpdate(float horizontalinput, float verticalinput)
    {

    }

    protected virtual void OnActivated(bool activated)
    {
        if (activated)
            transform.localScale = Vector3.one * initialscale * activatedscale;
        else
            transform.localScale = Vector3.one * initialscale;
    }
}

public enum OutOfBoundsInput
{
    InBounds,
    OutUp,
    OutRight,
    OutDown,
    OutLeft
}