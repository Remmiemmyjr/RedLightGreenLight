using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Light flashlight;
    public Transform camPos;
    float followSpeed = 6.5f;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;
        }
        if (flashlight.enabled)
        {
            flashlight.transform.localRotation = Quaternion.Slerp(flashlight.transform.localRotation,
                camPos.transform.localRotation, followSpeed * Time.deltaTime);
        }
    }
}
