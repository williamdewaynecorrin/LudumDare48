using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelText : MonoBehaviour
{
    [SerializeField]
    private string levelname = "THE DEEP WEB";
    [SerializeField]
    private int levelnum = 1;
    [SerializeField]
    private Transform levelnumtarget, levelnametarget;
    [SerializeField]
    private Text levelnumtext, levelnumtextshadow;
    [SerializeField]
    private Text levelnametext, levelnametextshadow;

    void Start()
    {
        levelnumtext.text = "LEVEL " + levelnum.ToString();
        levelnumtextshadow.text = levelnumtext.text;

        levelnametext.text = levelname;
        levelnametextshadow.text = levelname;

        StartCoroutine(TextAppear());
    }

    IEnumerator TextAppear()
    {
        // -- have levelnum text appear
        float framesnum = 60;
        Vector3 divnum = (levelnumtarget.localPosition - levelnumtext.transform.localPosition) / framesnum;
        for(int i = 0; i < framesnum; ++i)
        {
            levelnumtext.transform.localPosition += divnum;
            yield return new WaitForFixedUpdate();
        }
        levelnumtext.transform.localPosition = levelnumtarget.localPosition;

        // -- have levelname text appear
        float framesname = 45;
        Vector3 divname = (levelnametarget.localPosition - levelnametext.transform.localPosition) / framesname;
        for (int i = 0; i < framesname; ++i)
        {
            levelnametext.transform.localPosition += divname;
            yield return new WaitForFixedUpdate();
        }
        levelnametext.transform.localPosition = levelnametarget.localPosition;

        yield return null;
    }

    void FixedUpdate()
    {
        
    }
}
