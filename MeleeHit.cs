using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.UI;

// Melee attack script for hitting other players or enemies with melee weapons
public class MeleeHit : NetworkBehaviour
{
    private Animator animator;
    //public GameObject weaponParentTransformGameobject;
    private Transform weaponParentTransform;
    private Sound attackSound;
    public WeaponParent weaponParent;

    public float clientsWeaponParentRotation;

    // Weapon cooldown etc
    public float weaponUseCooldown = 2.0f;
    public float weaponCooldownCurrentValue = 0.0f;
    public Slider weaponCooldownSlider;
    public float weaponCooldownTickSpeed = 0.1f; // TODO: Set based on what weapon you have equipped

    // Weapon stats
    public float weaponDamage = 1;

    public PolygonCollider2D weaponCollider;
    public CharacterMovement characterMovement;

    void Start()
    {
        // NOT USING ATTACK COOLDOWN TIMER ATM, ADD LATER IF NEEDED
        //weaponCooldownSlider.maxValue = 100.0f;
        //weaponCooldownSlider.value = weaponCooldownCurrentValue;
        //weaponParentTransform = weaponParentTransformGameobject.transform;
        //print("THIS IS weaponParentGameobject " + weaponParentTransform); // TODO: something is off here

        animator = gameObject.GetComponent<Animator>();
        weaponParentTransform = transform.Find("WeaponParent");
        attackSound = transform.Find("WeaponParent").transform.Find("Face").transform.Find("Spear").GetComponent<Sound>();
        //weaponCooldownSlider.maxValue = 100.0f;
        //weaponCooldownSlider.value = weaponCooldownCurrentValue;
    }

    private void Update()
    {
        // NOT USING ATTACK COOLDOWN TIMER ATM, ADD LATER IF NEEDED
        //if (weaponCooldownCurrentValue >= 100.0f) return;
        //weaponCooldownCurrentValue += weaponCooldownTickSpeed;
        //weaponCooldownSlider.value = weaponCooldownCurrentValue;
    }


    public void ResumeMovement()
    {
        characterMovement.ResumeMovementSpeed();
    }
    public void PauseMovement()
    {
        characterMovement.SetZeroMovementSpeed();
    }
    // Prepares melee attack to use locally and send to all clients
    public void PrepareMeleeHit()
    {
        // NOT USING ATTACK COOLDOWN TIMER ATM, ADD LATER IF NEEDED
        //// Can only attack if weapon cooldown is over 100 == ready to attack
        //if (weaponCooldownCurrentValue < 100) return;
        RequestToMeleeHitServerRpc();
        ActivateMeleeHit();
    }
    // Main function to activate melee attack. 
    // Activates melee locally
    public void ActivateMeleeHit()
    {
        if (IsOwner)
        {
            print("attacking melee");
            //attackSound.PlayRandomSoundEffectFromList(); // Bugged, fix it

            try
            {
                Vector2 mousePointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                weaponParentTransform.right = (mousePointerPos - (Vector2)transform.position).normalized;
                //Vector2 direction = (mousePointerPos - (Vector2)transform.position).normalized;

            }
            catch (System.Exception e)
            {
                Debug.Log(e);

            }

            weaponParent.enabled = false;

            animator.SetTrigger("MeleePokeAttack");
            // NOT USING ATTACK COOLDOWN TIMER ATM, ADD LATER IF NEEDED
            //weaponCooldownCurrentValue = 0.0f;
        }
    }

    // Sent to server that we want RPC activation to all clients so they all play the melee hit correctly for whoever attacked melee
    [ServerRpc]
    private void RequestToMeleeHitServerRpc()
    {
        MeleeHitForClientRpc();
    }

    // All client activate melee hit for correct attacker
    [ClientRpc]
    private void MeleeHitForClientRpc()
    {
        if (!IsOwner)
        {
            ActivateMeleeHit();
        }
    }
}
