using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller = null;

    public Transform playerCam = null;

    public float sensitivity = 4f;

    [Range(0f, 0.5f)] float moveSmoothTime = 0.15f;
    [Range(0f, 0.5f)] float mouseSmoothTime = 0.05f;
    float xRotation;
    float walkspeed = 6f;
    float gravity = -25f;
    float velY = 0f;

    Vector2 currDir = Vector2.zero;
    Vector2 currVel = Vector2.zero;
    Vector2 currMouseDel = Vector2.zero;
    Vector2 currMouseVel = Vector2.zero;

    bool lockCursor = true;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLook();
        UpdateMovement();
    }

    void UpdateLook()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currMouseDel = Vector2.SmoothDamp(currMouseDel, mouseDelta, ref currMouseVel, mouseSmoothTime);

        xRotation -= currMouseDel.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCam.localEulerAngles = Vector3.right * xRotation;
        transform.Rotate(Vector3.up * currMouseDel.x * sensitivity);
    }

    void UpdateMovement()
    {
        Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        dir.Normalize();

        currDir = Vector2.SmoothDamp(currDir, dir, ref currVel, moveSmoothTime);

        if(controller.isGrounded)
        {
            velY = 0;
        }

        velY += gravity * Time.deltaTime;

        Vector3 vel = (transform.forward * currDir.y + transform.right * currDir.x) * walkspeed + (Vector3.up * velY);
        controller.Move(vel * Time.deltaTime);
    }
}
