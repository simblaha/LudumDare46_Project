using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameObject player;
    private GameObject zombie;
    private GameObject menuPanel;

    private void Awake()
    {
        player = GameObject.Find("Player");
        zombie = GameObject.Find("Zombie");
        menuPanel = GameObject.Find("MenuPanel");
        menuPanel.SetActive(false);
    }

    private void Update()
    {
        if (player == null || zombie == null)
        {
            if (!menuPanel.activeSelf)
            {
                StartCoroutine(GameOver());
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

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0;
        menuPanel.SetActive(true);
    }
}
