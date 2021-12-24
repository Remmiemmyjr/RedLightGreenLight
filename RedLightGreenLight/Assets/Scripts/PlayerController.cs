using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform playerCam = null;

    public Light flashlight = null;

    CharacterController controller = null;

    Rigidbody rb;

    float ogCamPosY;

    bool lockCursor = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

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

        flashlight.transform.Rotate(Vector3.up * -currMouseDel.x * mouseSensitivity);
        
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
    float jumpHeight = 3f;
    float jumpForce;
    float gravity = -25f;
    float downwardVel = 0f;

    Vector2 currDir = Vector2.zero;
    Vector2 currVel = Vector2.zero;

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

        // Walking
        Vector3 vel = (transform.forward * currDir.y + transform.right * currDir.x) * (isSprinting ? sprintSpeed : walkSpeed) + 
            (Vector3.up * downwardVel);

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            vel.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            //rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }

        // Free-fall Gravity
        downwardVel += gravity * Time.deltaTime;
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
