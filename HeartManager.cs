using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class HeartManager : NetworkBehaviour
{

    public Sprite full_heart;
    public Sprite empty_heart;
    public List<GameObject> heartGameObjects = new List<GameObject>();
    public GameObject heartGameObjectPrefab;

    void Start()
    {

    }


    // Change full heart icon to empty heart icon with amount of hearts as input
    public void ChangeFullHeartToEmptyHeart(int amountOfHearts)
    {
        int heartsChanged = 0;
        for (int i = heartGameObjects.Count - 1; i >= 0; i--)
        {
            if(heartGameObjects[i].GetComponent<Image>().sprite == full_heart)
            {
                GameObject currentObject = heartGameObjects[i];
                Image image = currentObject.GetComponent<Image>();
                image.sprite = empty_heart;
                heartsChanged += 1;

                // When we have changed the correct amount of full hearts to empty hearts we return
                if(heartsChanged == amountOfHearts)
                {
                    return;
                }
            }
        }
    }

    // Add as many heart icons as amount is
    public void AddHeartIcons(int amount) 
    {
        for (int i = 0; i < amount; i++)
        {
            print("ADDING HEART");
            GameObject instantiatedHeartIcon = Instantiate(heartGameObjectPrefab);
            instantiatedHeartIcon.transform.parent = gameObject.transform;
        }
    }
    // Happens at start of game only. Adds all gameobjhects to a list so we can work with them
    public void AddHeartGameobjectsToHeartList()
    {
        print("Adding heart gameobjhects to heart list");
        foreach (Transform item in gameObject.transform)
        {
            heartGameObjects.Add(item.gameObject);
        }
        //for (int i = 0; i < gameObject.transform.childCount; i++)
        //{
        //    heartGameObjects.Add(transform.GetChild(i).gameObject);
        //}
    }

    [ServerRpc]
    public void RemoveHeartServerRpc(ulong collidedObjectsOwnerClientId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { collidedObjectsOwnerClientId }
            }
        }; // List of which clients should have this code run for them
        RemoveHeartClientRpc(clientRpcParams);
    }

    // Runs for one client (or the server). Based on the provided collidedObjectsOwnerClientId
    [ClientRpc]
    private void RemoveHeartClientRpc(ClientRpcParams clientRpcParams = default)
    {
        print("Running remove heart code in client rpc for client id: " + OwnerClientId);
        //remove_single_latest_heart();

    }


}
