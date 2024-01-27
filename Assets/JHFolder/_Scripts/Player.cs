using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("References")]
    private PlayerController controller;
    public Camera playerCam;

    [Header("Player Stats")]
    public float maxHealth = 100;
    public float maxOxygen = 100;
    public float oxygenRegenRate;
    public float oxygenDecayRateWhileSprinting;
    public float oxygenDecayRateWhileHoldingBreath;
    public float maxLaughter;
    public float laughterHoldBreathMultiplier = 0.5f;
    public float laughterDecayRate;
    public float laughterDecayDelay = 0.5f;
    private float laugherDecayDelayTimer;

    [Header("UI References")]
    public Slider oxygenSlider;
    public Slider laughSlider;

    [Header("Keycodes")]
    public KeyCode holdBreathBind = KeyCode.Mouse1;

    [Header("Debug")]
    public float currentHealth = 100;
    public float currentOxygen = 100;
    public float currentLaughter = 0;
    public bool isHoldingBreath;
    public bool isConsumingOxygen;
    public bool isLaughing;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        oxygenSlider.maxValue = maxOxygen;
        oxygenSlider.value = maxOxygen;
        currentOxygen = maxOxygen;

        laugherDecayDelayTimer = laughterDecayDelay;
    }

    // Update is called once per frame
    void Update()
    {
        SprintOxygen();
        if(Input.GetMouseButton(1)) 
        {
            HoldBreath();
        }
        else if(!isConsumingOxygen)
        {
            RegenBreath();
        }

        if(currentOxygen <= 0)
        {
            controller.canSprint = false;
        }
        else if (currentOxygen > 0) 
        {
            controller.canSprint = true;
        }

        if(currentLaughter > 0)
        {
            laugherDecayDelayTimer -= Time.deltaTime;
            if(laugherDecayDelayTimer <= 0)
            {
                currentLaughter -= laughterDecayRate * Time.deltaTime;
                laughSlider.value = currentLaughter;
            }
        }

    }

    public void Laughter(float laughterGained)
    {
        if(currentLaughter < 100)
        {
            if(isHoldingBreath) 
            {
                currentLaughter += laughterGained * laughterHoldBreathMultiplier * Time.deltaTime;
            }
            else
            {
                currentLaughter += laughterGained * Time.deltaTime;
            }
            laughSlider.value = currentLaughter;
            laugherDecayDelayTimer = laughterDecayDelay;
        }
    }

    private void SprintOxygen()
    {
        if(controller.isSprinting)
        {
            if (currentOxygen >= 0)
            {
                isConsumingOxygen = true;
                currentOxygen -= oxygenDecayRateWhileHoldingBreath * Time.deltaTime;
            }

            oxygenSlider.value = currentOxygen;
        }
        else
        {
            isConsumingOxygen = false;
        }
        
    }

    private void HoldBreath()
    {
        isHoldingBreath = true;
        if (currentOxygen >= 0)
        {
            isConsumingOxygen = true;
            currentOxygen -= oxygenDecayRateWhileHoldingBreath * Time.deltaTime;
        }

        oxygenSlider.value = currentOxygen;
    }

    private void RegenBreath()
    {
        isHoldingBreath = false;
        if (currentOxygen < maxOxygen && !isConsumingOxygen)
        {
            currentOxygen += oxygenRegenRate * Time.deltaTime;
        }
        oxygenSlider.value = currentOxygen;
    }



}
