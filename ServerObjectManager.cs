using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

public class ServerObjectManager : NetworkBehaviour
{
    public GameObject serverGameObjectToSpawn;
    public Vector3 gameObjectSpawnPosition;
    public GameObject gameObjectToDestroy;

    public GameObject arrowGameObjectPrefab;
    public GameObject fireballGameObjectPrefab;
    public GameObject projectileTemplateInScene;

    private GameObject spawnedObject;


    #region ArrowShooting
    // All projectile spawning and physics etc is handled by the server.
    // This means that we cant use client id to figure out which arrow belongs to what player.
    // We will handle ALL projectiles as hostile instead with an collision activation timer 

    // Callable from SpawnObject function (both client and server)
    // This runs on the server! Therefor we can spawn the arrow here
    [ServerRpc(RequireOwnership = false)]
    public void ShootProjectileServerRpc(Vector2 shotDirection, float shootForce, Vector3 templateProjectileTransformPosition, Quaternion templateProjectileTransformRotation, string bowOrWand, ServerRpcParams serverRpcParams = default)
    {
        var shooterClientId = serverRpcParams.Receive.SenderClientId;
        print("in serverRPC, shooterClientId is: " + shooterClientId);
        // TODO: projectile isnt getting spawned
        if (bowOrWand == "bow") { spawnedObject = Instantiate(arrowGameObjectPrefab, templateProjectileTransformPosition, templateProjectileTransformRotation); }
        else if (bowOrWand == "wand") { spawnedObject = Instantiate(fireballGameObjectPrefab, templateProjectileTransformPosition, templateProjectileTransformRotation); }

        //GameObject spawnedObject = Instantiate(testSpawnGameObjectPrefab, templateProjectileTransformPosition, templateProjectileTransformRotation);
        spawnedObject.GetComponent<NetworkObject>().Spawn(true); // spawn as server ownership
        Rigidbody2D rb = spawnedObject.GetComponent<Rigidbody2D>();
        // Normalize the shot direction to get a unit vector (magnitude of 1)
        // This way the projectile speed will be the same wherever we have our mousePosition
        shotDirection.Normalize();
        // Add force to the projectile in the direction it should be shot
        rb.AddForce(shotDirection * shootForce, ForceMode2D.Impulse);
    }
    //// Only server can call this. Will run on all clients to spawn their local gameobject
    //[ClientRpc]
    //private void SpawnArrowClientRpc()
    //{
    //    if (!IsOwner)
    //    {
    //        SpawnObject(gameObjectToSpawn, gameObjectSpawnPosition);
    //    }
    //}
    #endregion

    #region Spawning Objects
    // This is not for projectile spawning (that is in ShootProjectile script)
    // Callable by both client and server. Will set input gameObject to the gameobejct to spawn
    public void SpawnObject(GameObject gameObject, Vector3 spawnPosition)
    {
        serverGameObjectToSpawn = gameObject;
        gameObjectSpawnPosition = spawnPosition;
        SpawnObjectServerRpc();
    }
    // Callable from SpawnObject function (both client and server), this will run the SpawnObjectClientRpc() to spawn object for all clients 
    [ServerRpc(RequireOwnership = false)]
    private void SpawnObjectServerRpc()
    {
        SpawnObjectClientRpc();
    }
    // Only server can call this. Will run on all clients to spawn their local gameobject
    [ClientRpc]
    private void SpawnObjectClientRpc()
    {
        if (!IsOwner)
        {
            SpawnObject(serverGameObjectToSpawn, gameObjectSpawnPosition);
        }
    }
    #endregion

    #region Destroying Objects
    // Callable by both client and server. Will set input gameObject to the gameobejct to destroy
    public void DestroyObject(GameObject gameObject)
    {
        gameObjectToDestroy = gameObject; 
        Destroy(gameObjectToDestroy); // Destroy my local gameobject
        DestroyGameObjectServerRpc(); // Send the same destroy() message to all the clients
    }
    private void DestroyObjectLocally(GameObject gameObjectToDestroy)
    {
        Destroy(gameObjectToDestroy);
    }
    // Callable from DestroyObject function (both client and server), this will run the DestroyGameObjectClientRpc() to destroy object for all clients 
    [ServerRpc(RequireOwnership = false)]
    private void DestroyGameObjectServerRpc()
    {
        //DestroyObjectLocally(gameObjectToDestroy); // Testing to destroy object here from server
        DestroyGameObjectClientRpc();
    }

    // Only server can call this. Will run on all clients to destroy their local gameobject
    [ClientRpc]
    private void DestroyGameObjectClientRpc()
    {
        if (IsServer)
        {
            DestroyObjectLocally(gameObjectToDestroy);
        }
    }
    #endregion

}
