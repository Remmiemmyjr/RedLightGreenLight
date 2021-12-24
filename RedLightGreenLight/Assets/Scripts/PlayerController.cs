using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller = null;

    public Transform playerCam = null;

    float ogCamPosY;

    bool lockCursor = true;

    void Start()
    {
        ogCamPosY = playerCam.localPosition.y;

        controller = GetComponent<CharacterController>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }


    void Update()
    {
        UpdateLook();

        UpdateInput();

        UpdateMovement();

        UpdateBobbing();
    }


    // MOUSE LOOK VARIABLES ===========================
    [Range(0f, 0.5f)] float moveSmoothTime = 0.15f;
    public float mouseSensitivity = 4f;
    float xRotation;

    Vector2 currMouseDel = Vector2.zero;
    Vector2 currMouseVel = Vector2.zero;
    // ================================================
    void UpdateLook()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currMouseDel = Vector2.SmoothDamp(currMouseDel, mouseDelta, ref currMouseVel, mouseSmoothTime);

        xRotation -= currMouseDel.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCam.localEulerAngles = Vector3.right * xRotation;
        transform.Rotate(Vector3.up * currMouseDel.x * mouseSensitivity);
    }


    // PLAYER INPUT VARIABLES =========================
    bool isSprinting;
    // ================================================
    void UpdateInput()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift);
    }


    // PLAYER MOVEMENT VARIABLES ======================
    [Range(0f, 0.5f)] float mouseSmoothTime = 0.05f;
    float walkSpeed = 4f;
    float sprintSpeed = 7.5f;
    float gravity = -25f;
    float downwardVel = 0f;

    Vector2 currDir = Vector2.zero;
    Vector2 currVel = Vector2.zero;
    Vector2 currInput = Vector2.zero;

    bool isMoving;
    // ================================================
    void UpdateMovement()
    {
        Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        dir.Normalize();

        currDir = Vector2.SmoothDamp(currDir, dir, ref currVel, moveSmoothTime);

        if(controller.isGrounded)
        {
            downwardVel = 0;
        }

        downwardVel += gravity * Time.deltaTime;

        Vector3 vel = (transform.forward * currDir.y + transform.right * currDir.x) * (isSprinting ? sprintSpeed : walkSpeed) + (Vector3.up * downwardVel);
        controller.Move(vel * Time.deltaTime);

        if (Mathf.Abs(vel.x) > 0.1f || Mathf.Abs(vel.z) > 0.1f)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }


    // PLAYER MOVEMENT VARIABLES ======================
    public float bobWalkSpeed = 10f;
    public float bobSprintSpeed = 16f;
    public float bobWalkAmount = 0.045f;
    public float bobSprintAmount = 0.065f;

    float timer = 0;
    // ================================================
    void UpdateBobbing()
    {
        if(!controller.isGrounded)
        {
            return;
        }

        if (isMoving)
        {
            timer += Time.deltaTime * (isSprinting ? bobSprintSpeed : bobWalkSpeed);
            playerCam.transform.localPosition = new Vector3(playerCam.transform.localPosition.x, ogCamPosY + 
                Mathf.Sin(timer) * (isSprinting ? bobSprintAmount : bobWalkAmount), playerCam.transform.localPosition.z);
        }
        else
        {
            timer = 0;
            //playerCam.transform.position = new Vector3(playerCam.position.x, Mathf.Lerp(playerCam.position.y, ogCamPosY,
                //Time.deltaTime * bobSpeed), playerCam.position.z);
        }
    }
}
