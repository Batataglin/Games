using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MovementState { Idle, Sneaking, Walking, Running, Jumping };

public class PlayerController : MonoBehaviour
{
    private const float idle = 0f, sneak = 5f, walk = 5f, run = 10f, trappedTimer = 2f;
    private const int maxStamina = 100, maxBomb = 3, maxTrap = 6;

    [SerializeField] float aux;

    [SerializeField]
    private float step, jumpStrength, rotSpeed, movSpeed, speedfactor, currStamina, jumpTimer, timer;
    [SerializeField]
    private int currMovement, recoverStaminaFactor, currQuantBomb, currQuantTraps;

    [SerializeField]
    List<string> inventory = new List<string>();

    [SerializeField]
    private bool canInteract, triggerSync;
    private string itemToBePicked;

    bool isTrapped, isWalking, isRunning, isSneking, moveLeft, moveRight, jump;

    Rigidbody rb;

    Animator anim;

    [SerializeField] private GameObject bomb, trap;
    [SerializeField] private Transform spawnBomb, spawnTrap;

    public float CurrStamina { get { return currStamina; } }
    public List<string> Inventory { get { return inventory; } }
    public float TrappedTimer {  get { return trappedTimer; } }
    public bool IsTrapped { set { isTrapped = value; Trapped(); } }


    void Awake()
    {
        GlobalSettings.gPlayer = this;

        rotSpeed = 250.0f;
        jumpStrength = 250.0f;
        recoverStaminaFactor = 6;
        jumpTimer = 0;

        currQuantBomb = 0;
        currQuantTraps = 0;

        triggerSync = false;
        isTrapped = false;

        currStamina = maxStamina;
        movSpeed = walk;

        rb = rb == null ? GetComponent<Rigidbody>() : null;
        anim = anim == null ? GetComponentInChildren<Animator>() : null;
        spawnBomb = spawnBomb == null ? GameObject.Find("BombSpawn").transform : null;
        spawnTrap = spawnTrap == null ? GameObject.Find("TrapSpawn").transform : null;

        Verifications();
    }

    void Start()
    {
        UpdateGUI();
    }

    // Verificate if all GetComponets are getting some components
    void Verifications()
    {
        if (rb == null)
        {
            Debug.Log("PLAYER - Rigidbody not found.");
        }
        if (anim == null)
        {
            Debug.Log("PLAYER - Animator not found.");
        }
        if (spawnBomb == null)
        {
            Debug.Log("PLAYER - Spawn bomb not found.");
        }
        if (spawnTrap == null)
        {
            Debug.Log("PLAYER - Spawn trap not found.");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        step = movSpeed * Time.deltaTime;

        Move();
        RotatePlayer();
        Jump();
        AnimationsControl();

        StaminaRecover();

        // Pickup itens
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUpItem();
        }

        // Throw a bomb
        if (Input.GetMouseButtonDown(0))
        {
            ThrowBomb();
        }

        // Place a trap
        if (Input.GetMouseButtonDown(1))
        {
            PlaceTrap();
        }

        if (isTrapped)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                OutTrap();
            }
        }

    }

    // Player movement
    private void Move()
    {
        #region Movement
        if (!isTrapped)
        {
            // Straight move
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * step);
                if (currStamina > 0 && currMovement == (int)MovementState.Running)
                {
                    currStamina -= 6 * Time.deltaTime;
                }

                if (!isRunning)
                {
                    isWalking = true;
                }
            }
            else
            {
                isWalking = false;
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate((Vector3.back * step) / 2f);
            }
        }
        #endregion

        #region Speed
        // Set relative speed of move
        if (Input.GetKey(KeyCode.LeftShift) && currStamina > 0)
        {
            movSpeed = run;
            currMovement = (int)MovementState.Running;
            isRunning = true;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            movSpeed = sneak;
            currMovement = (int)MovementState.Sneaking;
            isSneking = true;
        }
        else
        {
            isSneking = isRunning = false;
            movSpeed = walk;
            currMovement = (int)MovementState.Walking;
        }
        #endregion
    }

    // Jump
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrouded())
        {
            rb.AddForce(Vector3.up * jumpStrength);
        }

        //if (Input.GetKeyDown(KeyCode.Space) && IsGrouded())
        //{
        //    jump = true;
        //    triggerSync = true;
        //    jumpTimer = 0.55f;
        //}
        //else
        //{
        //    jump = false;
        //}

            //if (triggerSync)
            //{
            //    jumpTimer -= Time.deltaTime;
            //    if (jumpTimer <= 0.0f)
            //    {
            //        rb.AddForce(Vector3.up * jumpStrength);
            //        triggerSync = false;
            //    }
            //}

    }

    // Rotate player
    private void RotatePlayer()
    {
        // rotate player
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -rotSpeed * Time.deltaTime, 0);
            moveLeft = true;
        }
        else
        {
            moveLeft = false;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, rotSpeed * Time.deltaTime, 0);
            moveRight = true;
        }
        else
        {
            moveRight = false;
        }
    }

    // Animator control
    void AnimationsControl()
    {
        anim.SetBool("IsWalking", isWalking);
        anim.SetBool("IsRunning", isRunning);
        anim.SetBool("IsSneking", isSneking);
        anim.SetBool("TurnRight", moveRight);
        anim.SetBool("TurnLeft", moveLeft);
        anim.SetBool("Jump", jump);
    }

    // Control if player is trapped
    private void Trapped()
    {
        if(isTrapped)
        {
            movSpeed = idle;
            timer = trappedTimer;
            isWalking = false;
        }
    }

    // Set things back
    private void OutTrap()
    {
        isTrapped = false;
        movSpeed = walk;
    }

    // Check if player is grounded
    bool IsGrouded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 3f))
            return false;
        else
            return true;
    }

    // Thowing bombs
    private void ThrowBomb()
    {
        if (currQuantBomb > 0)
        {
            Instantiate(bomb, spawnBomb.transform.position, spawnBomb.transform.rotation);
            currQuantBomb--;
            GlobalSettings.gGUI.CurrQuantBomb = currQuantBomb;
        }
    }

    public void Death()
    {
        GlobalSettings.gGameState = (int)GlobalSettings.GAME_STATE.DEATH;
    }

    // Place a trap in curr position
    private void PlaceTrap()
    {
        if(currQuantTraps > 0)
        {
            Instantiate(trap, spawnTrap.transform.position, transform.rotation);
            currQuantTraps--;
            GlobalSettings.gGUI.CurrQuantTrap = currQuantTraps;
        }
    }

    // Increase stamina till its maximum
    private void StaminaRecover()
    {
        if (currStamina < maxStamina && currMovement != (int)MovementState.Running)
        {
            // recoverStaminaFactor -> need item to boost Stamina
            currStamina += recoverStaminaFactor * Time.deltaTime;
        }
    }

    // Pick up an item and add into inventory
    private void PickUpItem()
    {
        if (canInteract)
            inventory.Add(itemToBePicked);

        if(itemToBePicked == "BombPick")
        {
            if (currQuantBomb < maxBomb)
            {
                currQuantBomb++;
                UpdateGUI();
            }
        }
         if(itemToBePicked == "BearTrap" || itemToBePicked == "BearTrap(Clone)")
        {
            if (currQuantTraps < maxTrap)
            {
                currQuantTraps++;
                UpdateGUI();
            }
        }

        Destroy(GameObject.Find(itemToBePicked));
        itemToBePicked = null;
        canInteract = false;
        GlobalSettings.gGUI.IsActionKeyActive = false;
    }

    // Item used
    public void DropItem(string _item)
    {
        inventory.Remove(_item);
    }

    // Update HUD with current quantity
    private void UpdateGUI()
    {
        GlobalSettings.gGUI.CurrQuantBomb = currQuantBomb;
        GlobalSettings.gGUI.CurrQuantTrap = currQuantTraps;
    }

    #region Triggers
    private void OnTriggerEnter(Collider other)
    {
        // If collide with an pick up item set its name into a variable
        if (other.CompareTag("CanBePicked"))
        {
            itemToBePicked = other.name;
            canInteract = true;
            GlobalSettings.gGUI.DisplayText = "Press 'E' to pick";
            GlobalSettings.gGUI.IsActionKeyActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        itemToBePicked = null; // Trigger out set null 
        canInteract = false;
        GlobalSettings.gGUI.IsActionKeyActive = false;
    }

    #endregion

}

