using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpeedometer : MonoBehaviour
{
    [SerializeField]
    private Image handsprite;
    [SerializeField]
    private Text speedtext;
    [SerializeField]
    private float minhandangle;
    [SerializeField]
    private float maxhandangle;
    [SerializeField]
    private int maxmph;

    // Start is called before the first frame update
    void Awake()
    {
        SetSpeedRatio(0f);
    }

    public void SetSpeedRatio(float ratio)
    {
        SetHandRatio(ratio);

        float mph = ((float)maxmph) * ratio;
        speedtext.text = ((int)mph).ToString();
    }

    private void SetHandRatio(float ratio)
    {
        float dif = maxhandangle - minhandangle;
        float angle = minhandangle + dif * ratio;

        handsprite.transform.localRotation = Quaternion.Euler(-Vector3.forward * angle);
    }
}
