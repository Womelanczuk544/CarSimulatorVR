using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private bool isUIActive = true;
    public GameObject UI;
    public GameObject quitButton;
    public GameObject soundButton;
    bool isMuted = false;


    // Update is called once per frame
    void Update()
    {
        switchOnUI();
    }

    private void switchOnUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isUIActive == true)
            {
                UI.SetActive(false);
                quitButton.SetActive(false);
                soundButton.SetActive(false);
                isUIActive = false;
                Debug.Log("jo³");
            }
            else
            {
                UI.SetActive(true);
                quitButton.SetActive(true);
                soundButton.SetActive(true);
                isUIActive = true;
                Debug.Log("jo³ v21");
            }
        }
    }

    public void switchOff()
    {
        Application.Quit();
        Debug.Log("dzia³a");
    }

    public void soundsEffects()
    {
        SoundManager.Instance.ToggleMute(!isMuted);
        isMuted = !isMuted;
    }
}
