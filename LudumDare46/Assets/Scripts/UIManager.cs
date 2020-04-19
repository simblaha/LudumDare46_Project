using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameObject player;
    private GameObject menuPanel;

    private void Awake()
    {
        player = GameObject.Find("Player");
        menuPanel = GameObject.Find("MenuPanel");
        menuPanel.SetActive(false);
    }

    private void Update()
    {
        if (player == null)
        {
            if (!menuPanel.activeSelf)
            {
                Time.timeScale = 0;
                menuPanel.SetActive(true);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = Time.timeScale == 0 ? 1 : 0;
                menuPanel.SetActive(!menuPanel.activeSelf);
            }
        }
    }
}
