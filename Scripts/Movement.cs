using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    float step;

    // Update is called once per frame
    void Update()
    {
        step = speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * step);

        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * step);

        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.forward * step);

        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.back * step);
    }
}
