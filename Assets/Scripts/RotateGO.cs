using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGO : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.up, -15 * Time.deltaTime); 
    }
}
