using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggle : UISelectable<bool>
{
    [SerializeField]
    private Sprite on, off;
    [SerializeField]
    private Color oncolor, offcolor;

    private Image image;

    protected override void OnAwake()
    {
        image = GetComponent<Image>();
    }

    protected override void UICustomUpdate(float horizontalinput, float verticalinput)
    {
        // - select hovered index
        if (controller.GetAButton(ButtonQuery.Down))
        {
            Toggle(!exposedvalue);
        }
    }

    public void Toggle(bool toggle)
    {
        exposedvalue = toggle;

        if (exposedvalue)
        {
            image.sprite = on;
            image.color = oncolor;
        }
        else
        {
            image.sprite = off;
            image.color = offcolor;
        }
    }
}
