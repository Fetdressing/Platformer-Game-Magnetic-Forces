﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public GameObject gameUIPanel;
    public GameObject menuUIPanel;
    public GameObject settingsUIPanel;

    private Transform player;
    public Transform goal;
    public GameObject goalDisplay; //aktiveras när man går i mål
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        goalDisplay.SetActive(false);
        ToggleMenu(false);
	}
	
	// Update is called once per frame
	void LateUpdate () {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }

        if(Vector3.Distance(player.position, goal.position) < 45)
        {
            goalDisplay.SetActive(true);
            Time.timeScale = 0;
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
        if (uiM.gameObject == gameUIPanel.gameObject) { gameUIPanel.SetActive(true); Cursor.visible = false; } else { gameUIPanel.SetActive(false); Cursor.visible = true; }
        if (uiM.gameObject == settingsUIPanel.gameObject) settingsUIPanel.SetActive(true); else settingsUIPanel.SetActive(false);
        if (uiM.gameObject == menuUIPanel.gameObject) menuUIPanel.SetActive(true); else menuUIPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
