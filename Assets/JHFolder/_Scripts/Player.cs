using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("References")]
    private PlayerController cc;
    public Camera playerCam;
    public AudioSource audioSource;
    public AudioSource laughterAudioSource;

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
    public Image laughterOverlay;


    [Header("UI Parameters")]
    public float laughterOverlayAlpha;
    public float laughterOverlayFadeTime;

    [Header("Keycodes")]
    public KeyCode holdBreathBind = KeyCode.Mouse1;

    [Header("Audio")]
    public AudioClip holdBreathSound;
    public AudioClip releaseBreathSound;
    public AudioClip giggleSound;
    public AudioClip chuckleSound;
    public AudioClip laughSound;
    public AudioClip hystericalLaughSound;
    public float normalLaughVolume = 0.8f;
    public float suppressedLaughVolume = 0.1f;

    [Header("Debug")]
    public float currentHealth = 100;
    public float currentOxygen = 100;
    public float currentLaughter = 0;
    //1-25 giggling, 26-50 light chuckling, 51-75 laughing, 76-100 hysterical
    public bool isHoldingBreath;
    public bool isConsumingOxygen;
    public bool isLaughing;
    public bool isLaughAudioPlaying;
    public bool isInGas;
    public bool canHoldBreath;

    private Coroutine shiftAlpha;

    private void Awake()
    {
        cc = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        oxygenSlider.maxValue = maxOxygen;
        oxygenSlider.value = maxOxygen;
        currentOxygen = maxOxygen;
        canHoldBreath = true;
        isHoldingBreath = false;
        laugherDecayDelayTimer = laughterDecayDelay;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        SprintOxygen();
        if(Input.GetMouseButton(1) && canHoldBreath) 
        {
            HoldBreath();
        }
        else if(!isConsumingOxygen)
        {
            RegenBreath();
        }

        if(currentOxygen <= 0)
        {
            if(isHoldingBreath == true)
            {
                audioSource.clip = releaseBreathSound;
                audioSource.Play();
                isHoldingBreath = false;
            }
        }

        if(isHoldingBreath)
        {
            laughterAudioSource.volume = suppressedLaughVolume;
        }
        else
        {
            laughterAudioSource.volume = normalLaughVolume;
        }

        if(Input.GetMouseButtonDown(1) && canHoldBreath)
        {
            audioSource.clip = holdBreathSound;
            audioSource.Play();
        }

        if(Input.GetMouseButtonUp(1) && isHoldingBreath)
        {
            isHoldingBreath = false;
            audioSource.clip = releaseBreathSound;
            audioSource.Play();
        }

        if(currentOxygen <= 0)
        {
            cc.canSprint = false;
        }
        else if (currentOxygen > 0) 
        {
            cc.canSprint = true;
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

        LaughterAudio();

    }

    public void TakeDamage(float damage)
    {
        if(currentHealth > 0)
        {
            currentHealth -= damage;
            if(currentHealth > 0)
            {
                currentHealth = 0;
            }
        }
    }

    public void RecoverHealth(float amount)
    {
        if (currentHealth < 0)
        {
            currentHealth += amount;
            if (currentHealth < 100)
            {
                currentHealth = 100;
            }
        }
    }

    public void LaughterAudio()
    {

        if(currentLaughter > 0)
        {
            isLaughing = true;

            if(currentLaughter > 1 && currentLaughter < 25)
            {
                laughterAudioSource.clip = giggleSound;
                if (!isLaughAudioPlaying)
                {
                    isLaughAudioPlaying = true;
                    laughterAudioSource.Play();
                }
                else if(!laughterAudioSource.isPlaying)
                {
                    isLaughAudioPlaying = false;
                }
            }
            
            if (currentLaughter > 26 && currentLaughter < 50)
            {

                laughterAudioSource.clip = chuckleSound;
                if (!isLaughAudioPlaying)
                {
                    isLaughAudioPlaying = true;
                    laughterAudioSource.Play();
                }
                else if (!laughterAudioSource.isPlaying)
                {
                    isLaughAudioPlaying = false;
                }
            }

            if (currentLaughter > 51 && currentLaughter < 75)
            {

                laughterAudioSource.clip = laughSound;
                if (!isLaughAudioPlaying)
                {
                    isLaughAudioPlaying = true;
                    laughterAudioSource.Play();
                }
                else if (!laughterAudioSource.isPlaying)
                {
                    isLaughAudioPlaying = false;
                }
            }

            if (currentLaughter > 76 && currentLaughter < 100)
            {

                laughterAudioSource.clip = hystericalLaughSound;
                if (!isLaughAudioPlaying)
                {
                    isLaughAudioPlaying = true;
                    laughterAudioSource.Play();
                }
                else if (!laughterAudioSource.isPlaying)
                {
                    isLaughAudioPlaying = false;
                }
            }


        }
        else if(currentLaughter <= 0)
        {
            isLaughing = false;
            laughterAudioSource.Stop();
        }

    }

    public void Laughter(float laughterGained)
    {
        isInGas = true;
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

    public void FadeLaughterOverlay(bool doFade)
    {
        if(doFade)
        {
            if(shiftAlpha != null)
            {
                StopCoroutine(shiftAlpha);
            }

            shiftAlpha = StartCoroutine(ImageFade(laughterOverlay, laughterOverlayAlpha, laughterOverlayFadeTime));
            isInGas = true;
        }

        if(!doFade)
        {
            if (shiftAlpha != null)
            {
                StopCoroutine(shiftAlpha);
            }

            shiftAlpha = StartCoroutine(ImageFade(laughterOverlay, 0, laughterOverlayFadeTime));
            isInGas = false;
        }
    }

    private void SprintOxygen()
    {
        if(cc.isSprinting)
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
        if (currentOxygen >= 0)
        {
            isHoldingBreath = true;
            isConsumingOxygen = true;
            currentOxygen -= oxygenDecayRateWhileHoldingBreath * Time.deltaTime;
        }

        oxygenSlider.value = currentOxygen;
    }

    private void RegenBreath()
    {
        //isHoldingBreath = false;
        if (currentOxygen < maxOxygen && !isConsumingOxygen)
        {
            currentOxygen += oxygenRegenRate * Time.deltaTime;
        }
        oxygenSlider.value = currentOxygen;
    }

    public IEnumerator ImageFade(Image image, float endValue, float duration)
    {
        float elapsedTime = 0;
        float startValue = image.color.a;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
            yield return null;
        }
    }


}
