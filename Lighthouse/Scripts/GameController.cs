using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Transform pauseScreen;

    void Awake()
    {
        GlobalSettings.gGameState = (int)GlobalSettings.GAME_STATE.INGAME;
        //GlobalSettings.gCurrentScene = 0;
    }

    void Update()
    {
        TimeControl();

        if (Input.GetKeyDown(KeyCode.Escape) )
        {
            if (GlobalSettings.gGameState != (int)GlobalSettings.GAME_STATE.PAUSE)
            {
                Debug.Log("pause");
                PauseGame();
            }
            else if(GlobalSettings.gGameState == (int)GlobalSettings.GAME_STATE.PAUSE)
            {
                Debug.Log("Unpause");
                UnpauseGame();
            }
        }
    }

    void TimeControl()
    {
        if (GlobalSettings.gGameState == (int)GlobalSettings.GAME_STATE.PAUSE)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    void PauseGame()
    {
        pauseScreen.gameObject.SetActive(true);
        GlobalSettings.gPlayer.GetComponent<PlayerController>().enabled = false;
        GlobalSettings.gGameState = (int)GlobalSettings.GAME_STATE.PAUSE;
    }

    void UnpauseGame()
    {
        pauseScreen.gameObject.SetActive(false);
        GlobalSettings.gPlayer.GetComponent<PlayerController>().enabled = true;
        GlobalSettings.gGameState = (int)GlobalSettings.GAME_STATE.INGAME;
    }

    public void ContinueBTN_Click()
    {
        Debug.Log("continue");
        UnpauseGame();
    }

    public void QuitGameBTN_Click()
    {
        Application.Quit();
    }

    public void StartBTN_Click()
    {
        GlobalSettings.gCurrentScene = 1;
        SceneManager.LoadScene(GlobalSettings.gCurrentScene);
    }

}
