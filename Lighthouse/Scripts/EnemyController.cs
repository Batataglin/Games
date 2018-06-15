using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private const float fearTime = 15.0f, trappedTimer = 5.0f, updatePositionTime = 3.0f, initialSpeed = 600.0f;
    [SerializeField] private float speed, timer, timerUpdatePosition;
    [SerializeField] private bool inFear, isTrapped, isRunning;

    [SerializeField] private GameObject player, home;

    private GameObject target;
    private NavMeshAgent _navAgent;
    private Animator anim;

    public float TappedTimer { get { return trappedTimer; } }

    public bool InFear { set { inFear = value; } }
    public bool IsTrapped { set { isTrapped = value; Trapped(); } }

    void Awake()
    {
        GlobalSettings.gEnemy = this;

        _navAgent = _navAgent == null ? GetComponent<NavMeshAgent>() : null;
        home = home == null ? GameObject.FindGameObjectWithTag("DogHouse") : null;
        player = player == null ? GameObject.FindGameObjectWithTag("Player") : null;
        anim = anim == null ? GetComponentInChildren<Animator>() : null;

        target = player; // Set the player as a primary target

        speed = initialSpeed;

        isRunning = inFear = isTrapped = false;

        timerUpdatePosition = updatePositionTime;
    }

    private void Verification()
    {
        if(_navAgent == null)
        {
            Debug.Log("ENEMY - NavMeshAgent not found!");
        }
        if (home == null)
        {
            Debug.Log("ENEMY - Dog house not found!");
        }
        if (player == null)
        {
            Debug.Log("ENEMY - Player not found!");
        }
    }

    void Update()
    {
        // Get a new position of target every "updatePositionTime" time
        timerUpdatePosition -= Time.deltaTime;
        if (timerUpdatePosition < updatePositionTime)
        {
            SetTarget(target.transform);
            timerUpdatePosition = updatePositionTime;
        }

        // Compare the distace to make an attack
        if(Vector3.Distance(transform.position, player.transform.position) < 2.0f)
        {
            //if (!inFear)
            {
                Attack();
            }
        }

        // Move around the map
        _navAgent.speed = speed * Time.deltaTime;

        anim.SetBool("Run", isRunning);

        if(_navAgent.speed <= 0f)
        { isRunning = false; }
        else { isRunning = true; }

        if (inFear)
        {
            timer -= Time.deltaTime;

            if (timer < 0.0f)
            {
                OutFear();
            }
        }
    }
    
    // Get a target position
    void SetTarget(Transform _target)
    {
        _navAgent.SetDestination(_target.position);
    }
    
    // Check the bool value to set speed
    private void Trapped()
    {
        if (isTrapped)
        {
            speed = 0.0f;
            // play sound - pain
        }
    }

    private void OutTrapped()
    {
        // play sound - bark
        speed = initialSpeed;
    }

    // Makes the dog move to his house
    public void ActiveFear()
    {
        // Play sound - "Cain Cain Cain"
        inFear = true;
        target = home;
        SetTarget(target.transform);
        timer = fearTime;
    }

    // Get out of fear and start chase again
    private void OutFear()
    {
        inFear = false;
        target = player;
        SetTarget(target.transform);
        speed = initialSpeed;
    }

    // Attack
    private void Attack()
    {
        // play sound - attack bark
        Debug.Log("Enemy attacked");
        GlobalSettings.gPlayer.Death();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DogHouse"))
        {
            if (inFear)
            {
                speed = 0.0f;
            }
        }
    }
}