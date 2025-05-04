
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public int health_points = 3;
    public int armor_points = 1;
    private bool canPlayerTakeDamage = true;
    HeartManager heart_manager;
    //private ArmorManager armorManager;
    private SpriteRenderer spriteRenderer;
    private ServerObjectManager serverObjectManager;
    public float knockbackDuration = 1.5f;
    public float knockbackForce = 10f;
    private CharacterMovement playerMovement;

    private Rigidbody2D playerRigidbody;
    private bool isKnockedBack = false;
    public TextMeshProUGUI debugWindowPlayerHealth;
    public SpriteRenderer[] playersTakeDamageSprites;
    void Start()
    {
        heart_manager = GameObject.Find("HeartContainer").GetComponent<HeartManager>();
        //armorManager = GameObject.Find("ArmorContainer").GetComponent<ArmorManager>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        serverObjectManager = GameObject.Find("ServerObjectSpawner").GetComponent<ServerObjectManager>();
        playerRigidbody = gameObject.transform.root.GetComponent<Rigidbody2D>();
        playerMovement = gameObject.GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Player collision 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsOwner) return; // TODO: Does this work?
        if (!canPlayerTakeDamage) return;
        if (collision.collider.tag == "Enemy" || collision.collider.tag == "Trap")
        {
            Vector2 playerKnockBackDirection = (collision.transform.position - gameObject.transform.position).normalized;
            //print("knockback vector2 is: " + playerKnockBackDirection);
            //Debug.Log("Taking damage from enemy colission or trap");
            DecreasePlayerHealthPoints(1);
            Knockback(collision.gameObject.transform.position); // TODO: Knockback doesnt work now yet :)
            StartCoroutine(ChangeColorAndEnableInvincibilityOnTakeDamage());
            PrepareRequestToDealDamageToPlayer(1, OwnerClientId);
        }
    }


    public void DecreasePlayerHealthPoints(int amount)
    {
        //print("Player lost "+amount+" hp");
        health_points -= amount; // works
        SetDebugHealthValueServerRpc(health_points); // Update debug window health for all connected users

        //heart_manager.remove_single_latest_heart();
        heart_manager.ChangeFullHeartToEmptyHeart(amount); // Test change all hearts to empty
        if (health_points <= 0)
        {
            PlayerDeath();
        }
        
    }

    // NOT USING ARMOR AT THE MOMENT
    //public void IncreasePlayerArmorPoints(int amount)
    //{
    //    armor_points += amount;
    //    armorManager.AddArmorIcon(amount);
    //}

    private void PlayerDeath()
    {
        serverObjectManager.DestroyObject(gameObject);
    }

    // Change color indicating a hit and make player invincible for 1 seconds
    IEnumerator ChangeColorAndEnableInvincibilityOnTakeDamage()
    {
        Color previousColor = Color.white; // Just some random color here we will overwrite
        foreach (SpriteRenderer playerEquipmentSR in playersTakeDamageSprites)
        {
            // Set the standard color here so we can assign it later so its not red.
            previousColor = playerEquipmentSR.color;
            playerEquipmentSR.color = Color.red;        
        }

        canPlayerTakeDamage = false;
        yield return new WaitForSeconds(1.0f);

        // Apply the previous color to all players takedamage sprites again
        foreach (SpriteRenderer playerEquipmentSR in playersTakeDamageSprites)
        {
            playerEquipmentSR.color = previousColor;
        }

        canPlayerTakeDamage = true;
    }

    public void Knockback(Vector3 objectCollidedWithsPosition)
    {
        // TODO: This knockback does not work at the moment
        if (!canPlayerTakeDamage) return;
        if (!isKnockedBack)
        {
            //print("KNOCKED BACK, collided with: " + objectCollidedWithsPosition.name);
            //isKnockedBack = true;

            print("Player rb is: " + playerRigidbody);
            float knockbackPower = 1000.0f;
            Vector2 knockbackDirection = (playerRigidbody.transform.position - objectCollidedWithsPosition).normalized;
            Debug.Log("Knockback direction: " + knockbackDirection);
            playerRigidbody.AddForce(knockbackDirection * knockbackPower, ForceMode2D.Impulse);

        }
    }

    #region UPDATING DEBUG WINDOW WITH HEALTH VALUES FOR ALL CLIENTS (INCLUDING HOST)
    // Update all players (including myself) debug window with the new health value
    [ServerRpc(RequireOwnership = false)]
    private void SetDebugHealthValueServerRpc(int health_points)
    {
        SetDebugHealthValueClientRpc(health_points);
    }
    [ClientRpc]
    private void SetDebugHealthValueClientRpc(int health_points)
    {
        debugWindowPlayerHealth.text = health_points.ToString(); // Set debug window health
    }
    #endregion

    // TODO: Move this to character movement script nstead
    IEnumerator DisableMovement(float secondsToWait)
    {
        playerMovement.enabled = false;
        yield return new WaitForSeconds(secondsToWait);
        isKnockedBack = false;
        playerMovement.enabled = true;
    }

    // TODO: Currently not in use????
    private IEnumerator KnockCoroutine(Rigidbody2D playerRigidBody2D, GameObject objectCollidedWith, float secondsToWait)
    {

        playerMovement.enabled = false;
        playerRigidBody2D.velocity = Vector2.zero;
        Vector2 forceDirection = transform.position - objectCollidedWith.transform.position;

        //print("Force direction is: " + forceDirection);
        Vector2 force = forceDirection.normalized * knockbackForce;

        playerRigidBody2D.velocity = force;
        yield return new WaitForSeconds(secondsToWait);

        playerRigidBody2D.velocity = new Vector2();

        playerMovement.enabled = true;
        isKnockedBack = false;

    }


    #region Damage a specific player based on cliend id
    // Use this function from scripts that dont have networkobject on them. otherwise unity complains

    public void PrepareRequestToDealDamageToPlayer(int damage, ulong clientId, bool damageOnlySelf = false)
    {
        if (damageOnlySelf)
        {
            print("Damaging only self, not sending over network");
            DecreasePlayerHealthPoints(damage);
        }
        else
        {
            RequestToDealDamageToPlayerServerRpc(damage, clientId);
        }
    }

    // ONLY FUNCTION TO USE WHEN WE WANT TO DEAL DAMAGE TO PLAYER
    // This script is callable by anyone
    [ServerRpc(RequireOwnership = false)]
    public void RequestToDealDamageToPlayerServerRpc(int damage, ulong clientId)
    {
        print("Inside RequestToDealDamageToPlayerServerRpc for client: " + clientId);
        
        // NOTE! In case you know a list of ClientId's ahead of time, that does not need change,
        // Then please consider caching this (as a member variable), to avoid Allocating Memory every time you run this function
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };
        DealDamageToSpecificClientClientRpc(damage, clientRpcParams);
        IndicatePlayerHasTakenDamageClientRpc();
    }
    // This script runt the DecreasePlayerHealthPoints() function locally for the assigned client
    [ClientRpc]
    private void DealDamageToSpecificClientClientRpc(int damageToTake, ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        print("Running deal damage with clientrpc params: " + clientRpcParams.ToString());
        print("This is run on client id: " + OwnerClientId);
        DecreasePlayerHealthPoints(damageToTake);      
    }
    #endregion


    // Send to all clients, player changing color to indicate he has been hit
    // Callable by server only
    [ClientRpc]
    private void IndicatePlayerHasTakenDamageClientRpc()
    {
        StartCoroutine(ChangeColorAndEnableInvincibilityOnTakeDamage());
    }
}
