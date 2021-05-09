using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextShadow : MonoBehaviour
{
    private Text main;
    [SerializeField]
    private Text shadow;

    private Vector3 shadowoffset;

    void Awake()
    {
        if(main == null)
            main = GetComponent<Text>();
        shadowoffset = shadow.transform.localPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Text MainText()
    {
        return main;
    }

    public Text ShadowText()
    {
        return shadow;
    }

    public void SetText(string text)
    {
        if (main == null)
            main = GetComponent<Text>();

        main.text = text;
        shadow.text = text;
    }

    public void SetColor(Color c)
    {
        if (main == null)
            main = GetComponent<Text>();

        main.color = c;
    }

    public void SetShadowColor(Color c)
    {
        shadow.color = c;
    }

    public void SetPosition(Vector3 position)
    {
        if (main == null)
            main = GetComponent<Text>();

        main.transform.position = position;
        shadow.transform.position = position;
        shadow.transform.localPosition = shadowoffset;
    }
}
