using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootProjectile : NetworkBehaviour
{
    public GameObject projectilePrefab;
    public GameObject projectileTemplateInScene;
    public Transform projectileStartPosition;
    public PlayerNetwork playerNetwork;
    public Transform playerFaceTransform;
    public Animator animator;
    public bool isWeaponReadyToShoot = false;
    public Sound chargeSound;
    public Sound shootSound;
    public CharacterMovement characterMovement;
    public float shootForce = 10.0f;
    private ServerObjectManager ServerObjectSpawner;
   




    private void Start()
    {
        ServerObjectSpawner = GameObject.Find("ServerObjectSpawner").GetComponent<ServerObjectManager>();
        //playerNetwork = gameObject.GetComponent<PlayerNetwork>();
        //projectileStartPosition = transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.position;
        //projectileStartPosition = transform.Find("ProjectileSpawnPoint").transform.position;
        //projectileStartPosition = transform.Find("WeaponParent").transform.Find("Face").transform.Find("ProjectileSpawnPoint").transform.position;
    }
    // Mage shoots with wand instead of bow
    public void ChargeWeapon(string bowOrWand)
    {

        print("chargeweapon: "+ bowOrWand);
        animator.SetTrigger($"charge_{bowOrWand}");
        //chargeSound.audioSource.loop = true; // TODO: Set this to false somewhere
        chargeSound.PlayRandomSoundEffectFromList();
        if (bowOrWand == "bow")
        {
            characterMovement.SetHalvedMovementSpeed();
        }
        else if(bowOrWand == "wand")
        {
            characterMovement.SetZeroMovementSpeed();
        }
    }
    public void ResetChargeAnimation(string bowOrWand)
    {
        chargeSound.audioSource.Stop();
        string trigger = $"from_charge_to_idle_{bowOrWand}";
        animator.SetTrigger(trigger);
        characterMovement.moveSpeed = characterMovement.originalMovementSpeed; 
    }

    public void PrepareFireProjectile(string bowOrWand)
    {
        chargeSound.audioSource.Stop();
        animator.SetTrigger($"shooting_{bowOrWand}");
        shootSound.PlayRandomSoundEffectFromList();
        characterMovement.moveSpeed = characterMovement.originalMovementSpeed;
        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 shotDirection = new Vector2(gameObject.transform.right.x, gameObject.transform.right.y);//  This might feel better? --old way:mousePos - projectileStartPosition;

        // Need to make it a vector to for conversion later
        Vector3 projectileStartPosVector2 = projectileStartPosition.position;
        //print("projectileStartPosition is: " + projectileStartPosition);
        Vector2 mousePointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shotDirection = (mousePointerPos - (Vector2)projectileStartPosVector2).normalized;

        //print("Preparing fire projectile");
        //RequestToFireProjectileServerRpc(shotDirection); // Send request to server for firing the projectile to all clients
        FireProjectile(shotDirection, bowOrWand);                   // Fire the projectile locally for this user
    }

    // This has to happen on the server since only server can spawn network objects
    public void FireProjectile(Vector2 shotDirection, string bowOrWand)
    {
        ServerObjectSpawner.ShootProjectileServerRpc(shotDirection, shootForce, projectileTemplateInScene.transform.position, projectileTemplateInScene.transform.rotation, bowOrWand);
       // ServerObjectSpawner.SpawnArrowProjectileAndFireOnServer(shotDirection, shootForce, projectileTemplateInScene);
        //projectileStartPosition = transform.Find("WeaponParent").transform.Find("Face").transform.Find("ProjectileSpawnPoint").transform.position;
        // Spawn gameobject on server and instantiate
        //print("--- TEST --- . projectilePrefab.transform.rotation is: " + projectilePrefab.transform.rotation);
        //print("--- TEST --- . projectilePrefab.transform.localRotation is: " + projectilePrefab.transform.localRotation);


    }

    //[ServerRpc]
    //private void RequestToFireProjectileServerRpc(Vector2 shotDirection)
    //{
    //    FireProjectileForClientClientRpc(shotDirection);
    //}

    //[ClientRpc]
    //private void FireProjectileForClientClientRpc(Vector2 shotDirection)
    //{
    //    if (!IsOwner)
    //    {
    //        FireProjectile(shotDirection);
    //        print("In clientRPC Firing shot for client: " + OwnerClientId);
    //    }
    //}
}
