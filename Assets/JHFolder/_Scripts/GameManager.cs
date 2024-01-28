using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    bool isPaused;

    public GameObject pausedText;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        pausedText.SetActive(false);
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            if(isPaused)
            {
                Time.timeScale = 0f;
                pausedText.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                pausedText.SetActive(false);
            }
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
