using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour
{
    [SerializeField] float timeToActivate, destroyTimer;
    bool isReady, catchSomething; // Is armed and can catch something
    string catchedTag; // Tag of object got in trap

    void Awake()
    {
        timeToActivate = 2.0f;
        isReady = false;
        catchSomething = false;
    }

    void Update()
    {
        if (!isReady)
        {
            timeToActivate -= Time.deltaTime;

            if (timeToActivate <= 0.0f)
            {
                isReady = true;
                gameObject.tag = "Untagged";
                //play sound ready
            }
        }

        if(catchSomething)
        {
            destroyTimer -= Time.deltaTime;
            if(destroyTimer < 0.0f)
            {
                // check who is trapped and free
                if (catchedTag == GlobalSettings.gPlayer.tag)
                { GlobalSettings.gPlayer.IsTrapped = false; }

                if (catchedTag == GlobalSettings.gEnemy.tag)
                { GlobalSettings.gEnemy.IsTrapped = false; }

                GlobalSettings.gGUI.IsActionKeyActive = false; // Deactivate message
                Destroy(gameObject);
            }
        }

    }

    void OnTriggerEnter (Collider other)
    {
        if (isReady)
        {
            if (other.CompareTag(GlobalSettings.gPlayer.tag))
            {
                catchedTag = other.tag;
                //play sound close
                GlobalSettings.gPlayer.IsTrapped = true;
                catchSomething = true;
                destroyTimer = GlobalSettings.gPlayer.TrappedTimer;
            }

            if (other.CompareTag(GlobalSettings.gEnemy.tag))
            {
                catchedTag = other.tag;
                //play sound close
                GlobalSettings.gEnemy.IsTrapped = true;
                catchSomething = true;
                destroyTimer = GlobalSettings.gEnemy.TappedTimer;
            }

        }
    }
}
