using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private CircleCollider2D safeZoneCircleCollider;
    private bool isSafeZoneReadyToTrigger = true;
    private bool isPlayerSafeFromDamage = true;
    public PlayerHealth playerHealth;
    public PlayerData playerData;

    private List<ulong> playerClientIdsOutsideZone;
    void Start()
    {
        safeZoneCircleCollider = gameObject.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Only runs every 0,5 sec to make if more efficient
        if(isSafeZoneReadyToTrigger) 
        {
            StartCoroutine(DealDamageToPlayersOutsideSafeZoneAndShrinkZone());
        }
    }

    IEnumerator DealDamageToPlayersOutsideSafeZoneAndShrinkZone()
    {
        //print("DEALING DAMAGE TO PLAYERS OUTSIDE SAFE ZONE (JUST PRINTING FOR NOW)");
        isSafeZoneReadyToTrigger = false;
        //safeZoneCircleCollider.enabled = true;

        // Shrinking zone by 0.05f every 0,5 sec
        Vector3 newScale = transform.localScale;
        newScale.x -= 0.05f;
        newScale.y -= 0.05f;
        transform.localScale = newScale;
        // TODO: Damage player here instead :)
        yield return new WaitForSeconds(0.5f);

        isSafeZoneReadyToTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //print("Player: " + collision.gameObject.name + " with ownerCliendId: " + collision.gameObject.GetComponent<PlayerData>().ownerClientId + " ENTERED SAFEZONE");
            isPlayerSafeFromDamage = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            //print("Player: " + collision.gameObject.name + " with ownerCliendId: " + collision.gameObject.GetComponent<PlayerData>().ownerClientId + " EXITED SAFEZONE");
            collision.gameObject.GetComponent<PlayerHealth>().PrepareRequestToDealDamageToPlayer(1, collision.gameObject.GetComponent<PlayerData>().ownerClientId); // TODO: take damage here instead!½!!
            //collision.gameObject.GetComponent<PlayerHealth>().DecreasePlayerHealthPoints(1);
            isPlayerSafeFromDamage = false;
        }
    }

    //public void AddPlayerToPlayerList(GameObject playerGameObjectToAdd)
    //{
    //    playersInScene.Add(playerGameObjectToAdd);
    //}
}
