using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;

    [Header("References")]
    public Transform orientation;
    private CharacterController characterController;
    public Camera playerCam;

    [Header("Controller Parameters")]
    public float walkSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float gravity;
    public float jumpForce;
    public float jumpCoolDown;

    [Header("Crouch Parameters")]
    public float heightChangeTime = 0.2f;
    public Vector3 normalCameraHeight = new Vector3(0, 0.75f, 0);
    public Vector3 crouchCameraHeight = new Vector3(0, 0.25f, 0);
    private float normalCCHeight;
    private float crouchHeight = 1;
    private Vector3 normalCCCenter = Vector3.zero;
    private Vector3 crouchCenter = new Vector3(0, -0.5f, 0);


    [Header("Keycodes")]
    public KeyCode jumpKey;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;

    [Header("Debug")]
    public Vector3 moveDir;
    public float currentMoveSpeed;
    public float verticalVelocity;
    public bool canJump;
    public bool isJumping;
    public bool isGrounded;
    public bool isSprinting;
    public bool isCrouching;
    public bool canUnCrouch;

    [Header("Coroutines")]
    public Coroutine crouchStart;
    public Coroutine crouchEnd;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        normalCCHeight = characterController.height;
    }

    // Start is called before the first frame update
    void Start()
    {
        canUnCrouch = true;
        canJump = true;

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(characterController.isGrounded);
        isGrounded = characterController.isGrounded;
        CalculateMovement();
        ApplyGravity();
        CrouchCheck();
        moveDir.y += verticalVelocity;

        if (characterController.isGrounded && canJump)
        {
            verticalVelocity = 0;
            isJumping = false;
        }

        if (Input.GetKeyDown(jumpKey))
        {
            Jump();
        }

        if(!isGrounded)
        {
            if(characterController.collisionFlags == CollisionFlags.Above)
            {
                verticalVelocity = 0;
                characterController.stepOffset = 0;
            }
        }


        ApplyMovement();
    }

    private void CalculateMovement()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        if(isGrounded && !isCrouching && !isSprinting)
        {
            currentMoveSpeed = walkSpeed;
        }

        if (Input.GetKey(sprintKey) && isGrounded && !isCrouching && (horizontalInput != 0 || verticalInput != 0))
        {
            isSprinting = true;
            currentMoveSpeed = sprintSpeed;
        }
        else
        {
            isSprinting = false;
        }

        if(Input.GetKeyDown(crouchKey) && isGrounded && !isSprinting)
        {
            isCrouching = true;
            currentMoveSpeed = crouchSpeed;
            BeginCrouch();
        }

        if(Input.GetKeyUp(crouchKey) && isCrouching && canUnCrouch || isCrouching && !isGrounded && canUnCrouch || isCrouching && canUnCrouch && !Input.GetKey(crouchKey))
        {
            isCrouching = false;
            currentMoveSpeed = walkSpeed;
            EndCrouch();
        }

        moveDir = Vector3.ClampMagnitude(horizontalInput * orientation.right + verticalInput * orientation.forward, 1.0f) * currentMoveSpeed;
        moveDir = AdjustMovementToSlope(moveDir);
    }

    private void ApplyMovement()
    {
        characterController.Move(moveDir * Time.deltaTime);
    }

    private void Jump()
    {
        if(isGrounded && canJump)
        {
            moveDir.y = 0;
            verticalVelocity = jumpForce;
            canJump = false;
            Invoke(nameof(ResetJump), jumpCoolDown);
            isJumping = true;

        }
    }

    private void BeginCrouch()
    {
        if(crouchStart != null)
        {
            StopCoroutine(crouchStart);
        }

        if(crouchEnd != null)
        {
            StopCoroutine(crouchEnd);
        }

        crouchStart = StartCoroutine(MoveCamera(playerCam, crouchCameraHeight));
        characterController.height = crouchHeight;
        characterController.center = crouchCenter;
        
    }

    private void EndCrouch()
    {
        if (crouchStart != null)
        {
            StopCoroutine(crouchStart);
        }

        if (crouchEnd != null)
        {
            StopCoroutine(crouchEnd);
        }
        crouchEnd =  StartCoroutine(MoveCamera(playerCam, normalCameraHeight));
        characterController.height = normalCCHeight;
        characterController.center = normalCCCenter;
    }

    private void CrouchCheck()
    {
        if(Physics.Raycast(playerCam.transform.position, Vector3.up, 0.5f))
        {
            canUnCrouch = false;
        }
        else
        {
            canUnCrouch = true;
        }
    }

    private void ResetJump()
    {
        canJump = true;
    }

    private void ApplyGravity()
    {
        verticalVelocity -= gravity * Time.deltaTime;
    }

    IEnumerator MoveCamera(Camera camera, Vector3 target)
    {
        float startTime = Time.time;
        Vector3 origPos = camera.transform.localPosition;

        while (Time.time < startTime + heightChangeTime)
        {
            camera.transform.localPosition = Vector3.Lerp(origPos, target, (Time.time - startTime) / heightChangeTime);
            yield return null;
        }

        camera.transform.localPosition = target;
    }

    private Vector3 AdjustMovementToSlope(Vector3 velocity) //Allows sticking to slopes
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.3f))
        {
            if (hitInfo.normal != Vector3.up)
            {
                //Gets the rotation between the Upward Vector (0,1,0) and the normal of the slope.
                //Basically just returns the angle of slope.
                var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                //Debug.Log(slopeRotation.eulerAngles);

                //Then mutiplies the current normalized movement vector by that angle.
                //Resulting in the movement vector now aligning with the slope.
                var adjustedVelocity = slopeRotation * velocity;

                //If we're moving down a slope, return the the adjusted vector
                if (adjustedVelocity.y < 0)
                {
                    return adjustedVelocity;
                }
            }

        }
        //Otherwise do nothing
        return velocity;
    }


}
