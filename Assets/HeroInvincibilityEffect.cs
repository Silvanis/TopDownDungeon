using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroInvincibilityEffect : MonoBehaviour
{
    [SerializeField]
    private Material baseMaterial;
    [SerializeField]
    private Material invincibilityMaterial;
    [SerializeField]
    private float invincibilityDuration;
    [SerializeField]
    private float invincibilityFrameTime;
    private float invincibilityFrameTimeAccumulator = 0.0f;
    private float invincibilityTimeAccumulator = 0.0f;
    private bool isRunning = false;
    private List<float> colorMultipliers = new List<float> { 0.0f, 0.33f, 0.66f, 1.0f, 15.0f, 30.0f };
    private int colorIndexR;
    private int colorIndexG;
    private int colorIndexB;

    // Start is called before the first frame update
    void Start()
    {
        if (baseMaterial == null)
        {
            Debug.Log("Base Material not set.");
        }
        if (invincibilityMaterial == null)
        {
            Debug.Log("Invuln Material not set.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            if (invincibilityTimeAccumulator >= invincibilityDuration)
            {
                StopRunning();
                return;
            }
            else
            {
                invincibilityTimeAccumulator += Time.deltaTime;
                if (invincibilityFrameTimeAccumulator >= invincibilityFrameTime)
                {
                    invincibilityFrameTimeAccumulator = 0.0f;
                    colorIndexR--;
                    colorIndexG++;
                    colorIndexB++;
                    if (colorIndexR < 0)
                    {
                        colorIndexR = colorMultipliers.Count - 1;
                    }
                    if (colorIndexG == colorMultipliers.Count)
                    {
                        colorIndexG = 0;
                    }
                    if (colorIndexB == colorMultipliers.Count)
                    {
                        colorIndexB = 0;
                    }
                    invincibilityMaterial.SetFloat("_ColorStepIncrementR", colorMultipliers[colorIndexR]);
                    invincibilityMaterial.SetFloat("_ColorStepIncrementG", colorMultipliers[colorIndexG]);
                    invincibilityMaterial.SetFloat("_ColorStepIncrementb", colorMultipliers[colorIndexB]);
                }
                else
                {
                    invincibilityFrameTimeAccumulator += Time.deltaTime;
                }
            }
        }
    }

    public void StartRunning (float duration)
    {
        invincibilityDuration = duration;
        invincibilityTimeAccumulator = 0.0f;
        invincibilityFrameTimeAccumulator = 0.0f;
        isRunning = true;
        gameObject.GetComponent<SpriteRenderer>().material = invincibilityMaterial;
        colorIndexR = 1;
        colorIndexG = 3;
        colorIndexB = 5;
    }

    private void StopRunning ()
    {
        isRunning = false;
        gameObject.GetComponent<SpriteRenderer>().material = baseMaterial;
        

    }
}
