using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{

    [SerializeField] private bool _enabled = true;

    [SerializeField ,Range(0, 0.002f)] private float amplitude = 0.001f;
    [SerializeField, Range(0, 30)] private float frequency = 10.0f;

    [SerializeField] private Transform playerCam = null;
    [SerializeField] private Transform cameraHolder = null;

    private float toggleSpeed = 3.0f;
    private Vector3 startPos;
    private CharacterController cc;
    private PlayerController pc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        pc = GetComponent<PlayerController>();
        startPos = playerCam.localPosition;
    }

    private void Update()
    {
        if(enabled)
        {
            CheckMotion();
            playerCam.LookAt(FocusTarget());
        }
    }

    private void CheckMotion()
    {
        ResetPosition();
        float speed = new Vector3(cc.velocity.x, 0, cc.velocity.z).magnitude;
        if(speed < toggleSpeed)
        {
            return;
        }
        if (!cc.isGrounded)
        {
            return;
        }
        PlayMotion(FootStepMotion());
    }

    private void ResetPosition()
    {
        if(playerCam.localPosition == startPos)
        {
            return;
        }

        if (pc.isCrouching)
        {
            playerCam.localPosition = Vector3.Lerp(playerCam.localPosition, pc.crouchCameraHeight, 2 * Time.deltaTime);

        }
        else
        {
            playerCam.localPosition = Vector3.Lerp(playerCam.localPosition, startPos, 2 * Time.deltaTime);

        }
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x += Mathf.Sin(Time.time * frequency/ 2) * amplitude * 2;
        return pos;
    }

    private void PlayMotion(Vector3 motion)
    {
        playerCam.localPosition += motion;
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraHolder.transform.localPosition.y, transform.position.z);
        pos += cameraHolder.forward * 15.0f;
        return pos;
    }
}
