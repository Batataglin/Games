using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float timer, timeToExplode;
    [SerializeField] Light explosionLight;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        timer = timeToExplode;

        rb.AddForce(GlobalSettings.gPlayer.transform.forward * 100);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0.0f)
        {
            GlobalSettings.gEnemy.ActiveFear();
            explosionLight.enabled = true;
            //play sound

            if (timer < -0.2f)
                Destroy(gameObject);
        }
    }


}
