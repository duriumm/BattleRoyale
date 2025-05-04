using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickerLight : MonoBehaviour
{
    Light2D torch2dLightComponent;

    public float minOuterRadius;  // 6.9
    public float maxOuterRadius;  // 7.05
                                  // 
    public float minInnerRadius;  // -0.3
    public float maxInnerRadius;  // 0.05
                                  //
    public float minIntensity;    // 0.97
    public float maxIntensity;    // 1.02

    //// Change these values in inspector to test
    //public float intensity = 0.02f;
    //public float radius = 0.05f;



    void Start()
    {

        //fireParticleEffect = this.gameObject.GetComponent<ParticleSystem>();
        torch2dLightComponent = this.gameObject.GetComponent<Light2D>();
        minOuterRadius = torch2dLightComponent.pointLightOuterRadius - 0.05f;
        maxOuterRadius = torch2dLightComponent.pointLightOuterRadius + 0.05f;

        minInnerRadius = torch2dLightComponent.pointLightInnerRadius - 0.05f;
        maxInnerRadius = torch2dLightComponent.pointLightInnerRadius + 0.05f;

        minIntensity = torch2dLightComponent.intensity - 0.02f;
        maxIntensity = torch2dLightComponent.intensity + 0.02f;



        InvokeRepeating("Flicker", 0.0f, 0.1f);
    }

    public void Flicker()
    {

        // This random number generator is to make the light radius flicker
        float rndNumber = Random.Range(minOuterRadius, maxOuterRadius);
        torch2dLightComponent.pointLightOuterRadius = rndNumber;
        float rndNumber2 = Random.Range(minInnerRadius, maxInnerRadius);
        torch2dLightComponent.pointLightInnerRadius = rndNumber2;
        float rndNumber3 = Random.Range(minIntensity, maxIntensity);
        torch2dLightComponent.intensity = rndNumber3;

    }
    public void Pulsate()
    {

        // This random number generator is to make the light radius flicker
        float rndNumber = Random.Range(minOuterRadius, maxOuterRadius);
        torch2dLightComponent.pointLightOuterRadius = rndNumber;

        float rndNumber2 = Random.Range(minInnerRadius, maxInnerRadius);
        torch2dLightComponent.pointLightInnerRadius = rndNumber2;

        float rndNumber3 = Random.Range(minIntensity, maxIntensity);
        torch2dLightComponent.intensity = rndNumber3;

    }

    //private void StopFlicker()
    //{
    //    CancelInvoke();
    //    fireParticleEffect.Stop();
    //    torch2dLightComponent.enabled = false;
    //    isTorchActive = false;
    //}




}
