using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using static Player;

public class PlayerController : MonoBehaviour, IcanPing
{
    float horizontalInput;
    float verticalInput;

    [Header("References")]
    public Transform orientation;
    private CharacterController characterController;
    public Camera playerCam;
    public AudioSource audioSource;

    [Header("Controller Parameters")]
    public float walkSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float gravity;
    public float jumpForce;
    public float jumpCoolDown;

    [Header("Crouch Parameters")]
    public bool crouchToggle = false;
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

    [Header("Audio")]
    public AudioClip[] footstepsArray;
    public float footStepWalkInterval;
    public float footStepSprintInterval;
    public float footStepCrouchInterval;

    private float footStepTimer;

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
    public bool canSprint;
    public bool isMoving;

    [Header("Coroutines")]
    public Coroutine crouchStart;
    public Coroutine crouchEnd;

    public SoundLevel currentLevel = SoundLevel.level0;
    private float currentAlert = 0;
    private float currentAttack = 0;

    public float crouchAlert = 5;
    public float crouchAttack = 3;
    public float walkingAlert = 10;
    public float walkingAttack = 5;
    public float sprintingAlert = 15;
    public float sprintingAttack = 10;
    public enum SoundLevel
    {
        level0 = 0, level1 = 1, level2 = 2, level3 = 3,
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Vector4(0, 0, 1, .5f);

        Gizmos.DrawSphere(transform.position, currentAlert);
        Gizmos.color = new Vector4(1, 1, 0, .5f);

        Gizmos.DrawSphere(transform.position, currentAttack);
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        normalCCHeight = characterController.height;
        characterController.detectCollisions = true;
    }

    public void PingForEnemy(Vector3 pos, float allertRad, float attackRad)
    {
        EnemyMovement enemy = null;
        Collider[] collisions = Physics.OverlapSphere(pos, allertRad);

        foreach (Collider x in collisions)
        {
            if (x.TryGetComponent<EnemyMovement>(out enemy))
            {
                break;
            }
        }

        if (enemy != null)
        {
            float dist = Vector3.Distance(pos, enemy.transform.position);
            if (dist < allertRad)
            {
                enemy.alertMe(transform.position);
            }
            if (dist < attackRad)
            {
                enemy.attackDecoy(pos);
            }

        }

    }

    // Start is called before the first frame update
    void Start()
    {
        canUnCrouch = true;
        canJump = true;
        canSprint = true;
        currentMoveSpeed = walkSpeed;
        footStepTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(characterController.isGrounded);
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

        if(!isCrouching && !isSprinting && !isJumping && (horizontalInput != 0 || verticalInput != 0))
        {
            currentLevel = SoundLevel.level2;
        }
        else if (isSprinting)
        {
            currentLevel = SoundLevel.level3;
        }
        else if(isCrouching && (horizontalInput != 0 || verticalInput != 0))
        {
            currentLevel = SoundLevel.level1;
        }
        else if(horizontalInput == 0 || verticalInput == 0)
        {
            currentLevel = SoundLevel.level0;
        }


        switch (currentLevel)
        {
            case SoundLevel.level0:
                currentAlert = 0;
                currentAttack = 0;
                break;
            case SoundLevel.level1:
                currentAlert = crouchAlert;
                currentAttack = crouchAttack;
                PingForEnemy(transform.position, crouchAlert, crouchAttack);

                break;
            case SoundLevel.level2:
                currentAlert = walkingAlert;
                currentAttack = walkingAttack;
                PingForEnemy(transform.position, walkingAlert, walkingAttack);
                break;
            case SoundLevel.level3:
                currentAlert = sprintingAlert;
                currentAttack = sprintingAttack;
                PingForEnemy(transform.position, sprintingAlert, sprintingAttack);
                break;


        }
            

        FootSteps();
        ApplyMovement();
    }

    private void CalculateMovement()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(horizontalInput > 0 || verticalInput > 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (Input.GetKey(sprintKey) && isGrounded /*&& !isCrouching*/ && (horizontalInput != 0 || verticalInput != 0) && canSprint)
        {
            if(isCrouching)
            {
                isCrouching = false;
                EndCrouch();
            }

            isSprinting = true;
            currentMoveSpeed = sprintSpeed;
        }
        else if(isSprinting && !isCrouching)
        {
            isSprinting = false;
            currentMoveSpeed = walkSpeed;
        }

        if(Input.GetKeyDown(crouchKey) && isGrounded && !isSprinting)
        {

            if(crouchToggle && isCrouching == false)
            {
                isCrouching = true;
                currentMoveSpeed = crouchSpeed;
                BeginCrouch();
            }
            else if (crouchToggle && isCrouching)
            {
                isCrouching = false;
                currentMoveSpeed = walkSpeed;
                EndCrouch();
            }
            else
            {
                isCrouching = true;
                currentMoveSpeed = crouchSpeed;
                BeginCrouch();
            }

        }


        if(Input.GetKeyUp(crouchKey) && isCrouching && canUnCrouch && !crouchToggle || isCrouching && !isGrounded && canUnCrouch || isCrouching && canUnCrouch && !Input.GetKey(crouchKey) && !crouchToggle)
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

    private void FootSteps()
    {
        if(horizontalInput == 0 && verticalInput == 0 || !isGrounded)
        {
            footStepTimer = 0.1f;
        }

        if(isGrounded && (horizontalInput != 0 || verticalInput != 0))
        {
            footStepTimer -= Time.deltaTime;
            if (footStepTimer < 0)
            {
                if(isSprinting)
                {
                    footStepTimer = footStepSprintInterval;
                }
                else if(isCrouching)
                {
                    footStepTimer = footStepCrouchInterval;

                }
                else
                {
                    footStepTimer = footStepWalkInterval;

                }
                if (isGrounded && ((horizontalInput != 0 || verticalInput != 0)))
                {
                    audioSource.PlayOneShot(footstepsArray[Random.Range(0, footstepsArray.Length)], 0.1f);
                }

                //audioSource.PlayOneShot(footstepsArray[Random.Range(0, footstepsArray.Length)], 0.1f);


            }
        }
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
        //transform.position = transform.position + new Vector3(0, -0.5f, 0);
        characterController.enabled = false;
        transform.localPosition = new Vector3(transform.position.x, transform.localPosition.y - 0.5f, transform.position.z);
        characterController.enabled = true;
        //characterController.center = crouchCenter;

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
        //transform.position = transform.position + new Vector3(0, 0.5f, 0);
        characterController.enabled = false;
        transform.localPosition = new Vector3(transform.position.x, transform.localPosition.y + 0.5f, transform.position.z);
        characterController.enabled = true;
        //characterController.center = normalCCCenter;
    }

    private void CrouchCheck()
    {
        if(Physics.Raycast(playerCam.transform.position, Vector3.up, 1f))
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
