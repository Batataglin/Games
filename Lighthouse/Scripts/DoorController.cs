using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private string keyName;
    [SerializeField] private bool needKey;
    [SerializeField] private float doorSpeed;

    bool playerNear, isOpen;

    Quaternion target;
    string displayMessage;

    void Awake()
    {
        isOpen = false;
    }

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (playerNear && !isOpen)
            {
                // Check if the door need a key
                if (needKey)
                {
                    // Check if player has the needed key
                    if (GlobalSettings.gPlayer.Inventory.Contains(keyName))
                    {
                        Open();
                        GlobalSettings.gPlayer.DropItem(keyName);
                    }
                    else
                    {
                        this.displayMessage = "Press 'F' to open\nYou need a key to open";
                        GlobalSettings.gGUI.DisplayText = displayMessage;
                    }
                }
                // Ordinary door
                else
                {
                    Open();
                }
            }
            // Close event
            else if (playerNear && isOpen)
            {
                Close();
            }
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, doorSpeed * Time.deltaTime);
    }


    // Open the door 
    void Open()
    {
        // Special rotation for this specific gate
        if (keyName == "Key_VillageGate")
        {
            target = Quaternion.Euler(transform.rotation.x + 90, 0, 0);
        }
        else
        {
            target = Quaternion.Euler(0, transform.rotation.x + 90, 0);
        }
        isOpen = true;
    }

    // Close the door if open
    void Close()
    {
        if (keyName != "VillageGate")
        {
            target = Quaternion.Euler(0, transform.rotation.x - 0, 0);
        }
        isOpen = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerNear = true;
            this.displayMessage = "Press 'F' to open";
            GlobalSettings.gGUI.DisplayText = displayMessage;
            GlobalSettings.gGUI.IsActionKeyActive = true;

        }
    }

    //void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        GlobalSettings.gGUI.DisplayText = displayMessage;
    //    }
    //}

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }
}
