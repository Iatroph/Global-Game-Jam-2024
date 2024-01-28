using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IcanPing
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

    [Header("Laugh Parameters")]
    public float level1AlertRadius = 15;
    public float level2AlertRadius = 20;
    public float level3AlertRadius = 25;
    public float level4AlertRadius = 30;
    public float level1AttackRadius = 5;
    public float level2AttackRadius = 10;
    public float level3AttackRadius = 15;
    public float level4AttackRadius = 20;



    [Header("Keycodes")]
    public KeyCode holdBreathBind = KeyCode.Mouse1;

    [Header("Audio")]
    public AudioClip holdBreathSound;
    public AudioClip releaseBreathSound;
    public AudioClip giggleSound;
    public AudioClip chuckleSound;
    public AudioClip laughSound;
    public AudioClip hystericalLaughSound;
    public float normalLaughVolume = 0.4f;
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
    public bool doesLaughterDecay = true;

    private Coroutine shiftAlpha;

    public LaughterLevel currentLevel;
    private float currentAlert = 0;
    private float currentAttack = 0;
    public enum LaughterLevel
    {
        level0 = 0, level1 = 1, level2 = 2, level3 = 3, level4 = 4,
    }

    private void Awake()
    {
        cc = GetComponent<PlayerController>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Vector4(1, 0, 0, .5f);

        Gizmos.DrawSphere(transform.position, currentAlert);
        Gizmos.color = new Vector4(0, 1, 0, .5f);

        Gizmos.DrawSphere(transform.position, currentAttack);
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
        if(Input.GetMouseButton(1)) 
        {
            if(isHoldingBreath && isInGas)
            {
                HoldBreath();
            }
            else if (canHoldBreath && !isInGas)
            {
                HoldBreath();
            }
        }
        else if(!isConsumingOxygen && !isInGas)
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

        if(Input.GetMouseButtonDown(1) && canHoldBreath && !isInGas)
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

        if(currentLaughter > 0 && doesLaughterDecay)
        {
            laugherDecayDelayTimer -= Time.deltaTime;
            if(laugherDecayDelayTimer <= 0)
            {
                currentLaughter -= laughterDecayRate * Time.deltaTime;
                laughSlider.value = currentLaughter;
            }
        }

        TriggerMonster();
        LaughterAudio();

    }

    private void TriggerMonster()
    {
        if (currentLevel == LaughterLevel.level1)
        {
            if (isHoldingBreath)
            {
                PingForEnemy(transform.position, level1AlertRadius / 1.5f, level1AttackRadius);
            }
            else
            PingForEnemy(transform.position, level1AlertRadius, level1AttackRadius);
        }
        else if (currentLevel == LaughterLevel.level2)
        {
            if (isHoldingBreath)
            {
                PingForEnemy(transform.position, level2AlertRadius / 1.5f, level2AttackRadius);
            }
            else
                PingForEnemy(transform.position, level2AlertRadius, level2AttackRadius);
        }
        else if (currentLevel == LaughterLevel.level3)
        {
            if (isHoldingBreath)
            {
                PingForEnemy(transform.position, level3AlertRadius / 1.5f, level3AttackRadius);
            }
            else
                PingForEnemy(transform.position, level3AlertRadius, level3AttackRadius);
        }
        else if (currentLevel == LaughterLevel.level4)
        {
            if (isHoldingBreath)
            {
                PingForEnemy(transform.position, level4AlertRadius, level4AttackRadius);
            }
            else
                PingForEnemy(transform.position, level4AlertRadius, level4AttackRadius);
        }
        else
        {

        }
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
                    currentLevel = (LaughterLevel)1;
                    currentAlert = level1AlertRadius;
                    currentAttack = level1AttackRadius;
                    //PingForEnemy(transform.position, level1AlertRadius, level1AttackRadius);
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
                    currentLevel = (LaughterLevel)2;
                    currentAlert = level2AlertRadius;
                    currentAttack = level2AttackRadius;
                    //PingForEnemy(transform.position, level2AlertRadius, level2AttackRadius);
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
                    currentLevel = (LaughterLevel)3;
                    currentAlert = level3AlertRadius;
                    currentAttack = level3AttackRadius;
                    //PingForEnemy(transform.position, level3AlertRadius, level3AttackRadius);
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
                    currentLevel = (LaughterLevel)4;
                    currentAlert = level4AlertRadius;
                    currentAttack = level4AttackRadius;
                    //PingForEnemy(transform.position, level4AlertRadius, level4AttackRadius);
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
            currentAlert = 0;
            currentAttack = 0;
            isLaughing = false;
            currentLevel = 0;
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
