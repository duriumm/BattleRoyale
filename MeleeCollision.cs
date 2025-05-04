using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class MeleeCollision : NetworkBehaviour
{
    public ulong playerOwnerClientId;
    public int meleeWeaponsDamage = 1;
    public Sound attackSound;
    public GameObject myPlayer;
    
    void Start()
    {
        playerOwnerClientId = transform.root.transform.GetComponent<PlayerData>().ownerClientId;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // TODO: Only damage player once
        //print("We -- "+ playerOwnerClientId + " -- collided in melee with: -- " + collider.gameObject.GetComponent<PlayerData>().OwnerClientId + " --");
        // TODO: Deal damage to collided object

        if (collider.gameObject.tag == "Enemy")
        {
            collider.gameObject.GetComponent<EnemyData>().DecreaseEnemyHealthPoints(1);
        }
        // If we collide with a player that is not ourselves
        else if(collider.gameObject.tag == "Player") 
        {
            // This should then only run on MY local script. Not on the ones attacked???
            if (!IsOwner) return;
            print("colluided wuith: " + collider.gameObject.name); 
            // WE ARE NOW GETTING THE CORRECT OWNERCLIENTID!! YEY
   
            
            ulong collidedPlayersNetworkObjectClientId = collider.transform.Find("TestBuggyAnimPlayer").GetComponent<PlayerNetwork>().OwnerClientId;
            print("-- Collided players PlayerNetwork client id is: " + collidedPlayersNetworkObjectClientId);
            print("-- MY players PlayerNetwork client id is: " + myPlayer.GetComponent<PlayerNetwork>().OwnerClientId);




            collider.transform.Find("TestBuggyAnimPlayer").GetComponent<PlayerHealth>().PrepareRequestToDealDamageToPlayer(meleeWeaponsDamage, collidedPlayersNetworkObjectClientId);
            attackSound.PlayRandomSoundEffectFromList();

            //Vector3 playerKnockBackDirection = (gameObject.transform.position - collider.transform.position).normalized;
            //collider.gameObject.GetComponent<PlayerHealth>().Knockback(playerKnockBackDirection); // TODO: Add knockback effect based on weapons knockback force
        }
    }

}
