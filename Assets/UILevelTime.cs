using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelTime : MonoBehaviour
{
    [SerializeField]
    private Text levelnumtext, levelnumtextshadow;

    private float totalseconds;

    private int seconds;
    private int minutes;

    void Start()
    {
        levelnumtext.text = "00:00";
        levelnumtextshadow.text = levelnumtext.text;
    }

    void FixedUpdate()
    {
        totalseconds = Time.timeSinceLevelLoad;

        float minutes = Mathf.Floor(totalseconds / 60.0f);
        float seconds = totalseconds - minutes * 60.0f;

        string timetext = string.Format("{0:00}:{1:00}", minutes, seconds);
        levelnumtext.text = timetext;
        levelnumtextshadow.text = timetext;
    }
    
}
