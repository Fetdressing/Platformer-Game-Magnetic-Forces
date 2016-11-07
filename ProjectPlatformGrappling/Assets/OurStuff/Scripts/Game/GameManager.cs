using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public GameObject gameUIPanel;
    public GameObject menuUIPanel;
    public GameObject settingsUIPanel;
	// Use this for initialization
	void Start () {
        ToggleMenu(false);
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
	}

    void ToggleMenu()
    {
        bool b;
        if(menuUIPanel.activeSelf == true || settingsUIPanel.activeSelf == true)
        {
            b = false;
        }
        else
        {
            b = true;
        }

        if(b)
        {
            Time.timeScale = 0;
            ToggleUI(menuUIPanel);
        }
        else
        {
            Time.timeScale = 1;
            ToggleUI(gameUIPanel);
        }
    }

    void ToggleMenu(bool b)
    {

        if (b)
        {
            Time.timeScale = 0;
            ToggleUI(menuUIPanel);
        }
        else
        {
            Time.timeScale = 1;
            ToggleUI(gameUIPanel);
        }
    }

    public void ToggleUI(GameObject uiM)
    {
        if (uiM.gameObject == gameUIPanel.gameObject) gameUIPanel.SetActive(true); else gameUIPanel.SetActive(false);
        if (uiM.gameObject == settingsUIPanel.gameObject) settingsUIPanel.SetActive(true); else settingsUIPanel.SetActive(false);
        if (uiM.gameObject == menuUIPanel.gameObject) menuUIPanel.SetActive(true); else menuUIPanel.SetActive(false);
    }
}
