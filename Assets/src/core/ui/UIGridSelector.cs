using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGridSelector<T> : UISelectable<T>
{
    [SerializeField]
    private int width, height;
    [SerializeField]
    private float imagehoverscale, imageselectscale;
    [SerializeField]
    private Color imagehovercolor, imageselectcolor;

    // -- images
    private Image[] images;
    private T[][] objects;
    private int imagehoverindex;
    private int imageselectedindex;
    private float imagestandardscale;
    private Color imagestandardcolor;

    private int activevalueindex = 0;

    // -- overridden functions from base class
    protected override void OnAwake()
    {
        // -- init images array
        images = new Image[width * height];

        // -- init objects array using length of component data from the SIComponentRef 
        SIComponentRef firstchildsicomp = transform.GetChild(0).GetComponent<SIComponentRef>();
        int sicompdatalen = firstchildsicomp.SIRefLength();
        objects = JaggedArray.CreateJaggedArray<T[][]>(images.Length, sicompdatalen);

        // -- fill out data
        for (int i = 0; i < transform.childCount; ++i)
        {
            images[i] = transform.GetChild(i).GetComponent<Image>();

            SIComponentRef sicomp = transform.GetChild(i).GetComponent<SIComponentRef>();
            Object[] siobjects = sicomp.SIGetRefs();
            if (siobjects[0] is T)
                objects[i] = (T[])(object)siobjects;
            else
                Debug.LogError("Mismatch in template type to SIComponentRef type: " + typeof(T).ToString());
        }

        imagestandardscale = images[0].transform.localScale.x;
        imagestandardcolor = images[0].color;

        SetImageScale(imageselectedindex, imageselectscale);
        SetImageColor(imageselectedindex, imageselectcolor);
        SetActiveMesh(imageselectedindex);
    }

    protected override void UICustomUpdate(float horizontalinput, float verticalinput)
    {
        lastframeinput = OutOfBoundsInput.InBounds;
        int prevhover = imagehoverindex;
        int prevselected = imageselectedindex;

        if (horizontalinput > InputManager.DEADZONETHRESHOLD && prevhoriz < InputManager.DEADZONETHRESHOLD)
        {
            ++imagehoverindex;
            if (imagehoverindex % width == 0)
            {
                imagehoverindex = prevhover;
                lastframeinput = OutOfBoundsInput.OutRight;
            }
        }
        else if (horizontalinput < -InputManager.DEADZONETHRESHOLD && prevhoriz > -InputManager.DEADZONETHRESHOLD)
        {
            --imagehoverindex;//0 or 2
            if ((imagehoverindex + 1) % width == 0)
            {
                imagehoverindex = prevhover;
                lastframeinput = OutOfBoundsInput.OutLeft;
            }
        }
        else if (verticalinput > InputManager.DEADZONETHRESHOLD && prevvert < InputManager.DEADZONETHRESHOLD)
        {
            imagehoverindex -= width;
            if (imagehoverindex < 0)
            {
                imagehoverindex = prevhover;
                lastframeinput = OutOfBoundsInput.OutUp;
            }
        }
        else if (verticalinput < -InputManager.DEADZONETHRESHOLD && prevvert > -InputManager.DEADZONETHRESHOLD)
        {
            imagehoverindex += width;
            if (imagehoverindex > images.Length - 1)
            {
                imagehoverindex = prevhover;
                lastframeinput = OutOfBoundsInput.OutDown;
            }
        }

        if (prevhover != imagehoverindex)
        {
            SetImageScale(imagehoverindex, imagehoverscale);
            SetImageColor(imagehoverindex, imagehovercolor);

            if (prevhover != imageselectedindex)
            {
                SetImageScale(prevhover, 1f);
                SetImageColor(prevhover, imagestandardcolor);
            }
            else
            {
                SetImageScale(prevhover, imageselectscale);
                SetImageColor(prevhover, imageselectcolor);
            }
        }

        // - select hovered index
        if (controller.GetAButton(ButtonQuery.Down))
        {
            imageselectedindex = imagehoverindex;
            SetImageScale(imageselectedindex, imageselectscale);
            SetImageColor(imageselectedindex, imageselectcolor);

            if (imageselectedindex != prevselected)
            {
                SetImageScale(prevselected, 1f);
                SetImageColor(prevselected, imagestandardcolor);
                SetActiveMesh(imageselectedindex);
            }
        }
    }

    public override T GetValue()
    {
        return objects[activevalueindex][0];
    }

    public T[] GetValues()
    {
        return objects[activevalueindex];
    }

    public override int GetIndexValue()
    {
        return activevalueindex;
    }

    public override void SetIndexValue(int index)
    {
        activevalueindex = index;
        imagehoverindex = index;
        imageselectedindex = index;
    }

    protected override void OnActivated(bool activated)
    {
        Color color = activated ? imagehovercolor : imagestandardcolor;
        float scale = activated ? imagehoverscale : 1f;
        if(imagehoverindex == imageselectedindex)
        {
            color = imageselectcolor;
            scale = imageselectscale;
        }

        SetImageColor(imagehoverindex, color);
        SetImageScale(imagehoverindex, scale);
    }

    // -- color wheel specific functions
    void SetImageScale(int index, float scale)
    {
        images[index].transform.localScale = Vector3.one * imagestandardscale * scale;
    }

    void SetImageColor(int index, Color color)
    {
        images[index].color = color;
    }

    void SetActiveMesh(int index)
    {
        activevalueindex = index;
    }
}
