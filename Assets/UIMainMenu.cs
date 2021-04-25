using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField]
    private string firstlevelname = "level_01";
    [SerializeField]
    private GameObject mainmenu;
    [SerializeField]
    private GameObject controls;
    [SerializeField]
    private AudioClip sfxhover, sfxclick;

    void Start()
    {
        OnBackClicked();
    }

    void Update()
    {
        
    }

    public void OnStartClicked()
    {
        SceneManager.LoadScene(firstlevelname);
    }

    public void OnControlsClicked()
    {
        mainmenu.SetActive(false);
        controls.SetActive(true);
    }

    public void OnBackClicked()
    {
        mainmenu.SetActive(true);
        controls.SetActive(false);
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void PlayHoverSFX()
    {
        SFXManager.PlayClip2D(sfxhover);
    }

    public void PlayClickSFX()
    {
        SFXManager.PlayClip2D(sfxclick);
    }
}
