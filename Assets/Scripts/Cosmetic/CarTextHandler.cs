using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTextHandler : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0,180,0);

    }
}
