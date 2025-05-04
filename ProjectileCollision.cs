using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;


public class ProjectileCollision : MonoBehaviour
{
    public ulong playerOwnerClientId;
    private ServerObjectManager serverObjectManager;
    HeartManager heart_manager;
    private bool didProjectileCollideOnce = false;
    public ulong shootersOwnerCliendId;
    public float timeUntilArrowMomentumReached = 0.2f; // Arrows need certain momentum untill they can deal damage
    public bool canDamagePlayers = false;
    public int projectileDamage = 1;


    // Gameobjects with these tags will break the projectile on impact
    List<string> collisionTagsBreakingList = new List<string>
        {
            "Trap",
            "Wall"
        };


    private void Start()
    {
        serverObjectManager = GameObject.Find("ServerObjectSpawner").GetComponent<ServerObjectManager>();
        heart_manager = GameObject.Find("HeartContainer").GetComponent<HeartManager>();
    }
    void Update()
    {
        timeUntilArrowMomentumReached -= Time.deltaTime;

        if (timeUntilArrowMomentumReached < 0)
        {
            // Projectiles cant damage players straight after spawning.
            // They need to gain momentum before they can deal damage
            canDamagePlayers = true;
        }
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (didProjectileCollideOnce) return;
        if (collider.gameObject.name == "SafeZone") return;
        if(collider.gameObject.tag == "Enemy")
        {
            didProjectileCollideOnce = true;
            collider.gameObject.GetComponent<EnemyData>().DecreaseEnemyHealthPoints(1);
            serverObjectManager.DestroyObject(gameObject);
            didProjectileCollideOnce = false;
            return;
        }
        else if (collider.gameObject.tag == "Player")
        {
            if (canDamagePlayers)
            {
                print("GOOD MOMENTUM, DEALING DAMAGE \nObject we collided with: " + collider.gameObject.name);
                ulong colliderOwnerClientId = collider.GetComponent<NetworkObject>().OwnerClientId;

                // Get the first child GameObject where playerhealth is in
                Transform firstChild = collider.transform.GetChild(0);

                // Do something with the firstChild GameObject
                Debug.Log("First child name: " + firstChild.name);
                firstChild.GetComponent<PlayerHealth>().PrepareRequestToDealDamageToPlayer(projectileDamage, colliderOwnerClientId);
            }
            else
            {
                print("TOO LITTLE MOMENTUM TO DEAL DAMAGE \nObject we collided with: " + collider.gameObject.name);
            }
        }
 
        else if (collisionTagsBreakingList.Contains(collider.gameObject.tag))
        {
            didProjectileCollideOnce = true;
            print("Object we collided with: " + collider.gameObject.name + " destroying projectile!!");

            serverObjectManager.DestroyObject(gameObject);           
        }
        //try
        //{
        //    didProjectileCollideOnce = true;
        //    // Get the client id so we can shoot everything but ourselves
        //    ulong collidedObjectsOwnerClientId = collider.gameObject.GetComponent<PlayerNetwork>().OwnerClientId;
        //    print("Collided objects client id is: " + collidedObjectsOwnerClientId);
        //    print("Shooters client id is: " + playerOwnerClientId);

        //    if (playerOwnerClientId != collidedObjectsOwnerClientId && gameObject != null)
        //    {
        //        print("Shooters ClientID is not the same as Colliders ClientID. Destroying projectile");
        //        collider.GetComponent<PlayerHealth>().PrepareRequestToDealDamageToPlayer(1, collidedObjectsOwnerClientId);
        //        heart_manager.RemoveHeartServerRpc(collidedObjectsOwnerClientId); // Removing heart for one specific client or server
        //        serverObjectManager.DestroyObject(gameObject);
        //    }
        //}
        //catch (System.Exception e)
        //{
        //    print(e);
        //    print("Object we collided with: " + collider.gameObject.name + " does not have PlayerNetwork class == Not a player. Destroying projectile");
        //    if(gameObject != null)
        //    {
        //        serverObjectManager.DestroyObject(gameObject);
        //    }
        //}
        //print("Object we collided with: " + collider.gameObject.name + " does not have PlayerNetwork class == Not a player. Destroying projectile");
        //if (gameObject != null)
        //{
        //    serverObjectManager.DestroyObject(gameObject);
        //}
        didProjectileCollideOnce = false;

    }
}
