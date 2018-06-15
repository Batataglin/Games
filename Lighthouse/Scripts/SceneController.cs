using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GlobalSettings.gCurrentScene += 1;
            SceneManager.LoadScene(GlobalSettings.gCurrentScene);
        }
    }
}
