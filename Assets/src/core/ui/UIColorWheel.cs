using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorWheel : UISelectable<Color>
{
    [SerializeField]
    private int width, height;
    [SerializeField]
    private float imagestandardscale, imagehoverscale, imageselectscale;

    // -- images
    private Image[] images;
    private int imagehoverindex;
    private int imageselectedindex;

    private int activecolorindex = 0;

    // -- overridden functions from base class
    protected override void OnAwake()
    {
        images = new Image[width * height];
        for (int i = 0; i < transform.childCount; ++i)
            images[i] = transform.GetChild(i).GetComponent<Image>();

        SetImageScale(imageselectedindex, imageselectscale);
        SetActiveColor(imageselectedindex);
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
            --imagehoverindex;
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
                //imagehoverindex = width + imagehoverindex;
                imagehoverindex = prevhover;
                lastframeinput = OutOfBoundsInput.OutUp;
            }
        }
        else if (verticalinput < -InputManager.DEADZONETHRESHOLD && prevvert > -InputManager.DEADZONETHRESHOLD)
        {
            imagehoverindex += width;
            if (imagehoverindex > images.Length - 1)
            {
                //imagehoverindex = imagehoverindex - width;
                imagehoverindex = prevhover;
                lastframeinput = OutOfBoundsInput.OutDown;
            }
        }

        if (prevhover != imagehoverindex)
        {
            SetImageScale(imagehoverindex, imagehoverscale);

            if (prevhover != imageselectedindex)
                SetImageScale(prevhover, imagestandardscale);
            else
                SetImageScale(prevhover, imageselectscale);
        }

        // - select hovered index
        if (controller.GetAButton(ButtonQuery.Down))
        {
            imageselectedindex = imagehoverindex;
            SetImageScale(imageselectedindex, imageselectscale);

            if (imageselectedindex != prevselected)
            {
                SetImageScale(prevselected, imagestandardscale);
                SetActiveColor(imageselectedindex);
            }
        }
    }

    public override Color GetValue()
    {
        return images[activecolorindex].color;
    }

    public override int GetIndexValue()
    {
        return activecolorindex;
    }

    public override void SetIndexValue(int index)
    {
        activecolorindex = index;
        imagehoverindex = index;
        imageselectedindex = index;
    }

    protected override void OnActivated(bool activated)
    {
        float scale = activated ? imagehoverscale : imagestandardscale;
        if (imagehoverindex == imageselectedindex)
            scale = imageselectscale;

        SetImageScale(imagehoverindex, scale);
    }

    // -- unity functions
    void LateUpdate()
    {

    }

    // -- color wheel specific functions
    void SetImageScale(int index, float scale)
    {
        images[index].transform.localScale = Vector3.one * scale;
    }

    void SetActiveColor(int index)
    {
        activecolorindex = index;
    }

    public bool ColorChanged()
    {
        return false;
    }
}