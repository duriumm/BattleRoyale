using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script will activate the gameobject its attached or a
// script of choice after a set amount of time.
// Currently used on projectiles so they wont damage the shooting player.


public class ActivationTimer : MonoBehaviour
{
    public float timeUntilActivation = 1.0f;

    private void Start()
    {

    }
    // Countdown to destroy this object
    void Update()
    {
        timeUntilActivation -= Time.deltaTime;

        if (timeUntilActivation < 0)
        {
            gameObject.SetActive(true);
            // Disable this script after its done
            this.enabled = false;
        }
    }
}
