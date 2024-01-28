using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public Button playButton;
    public Button quitButton;

    public void PlayGame()
    {
        StartCoroutine(StartGame());
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(1);
        yield return null;
    }
}
