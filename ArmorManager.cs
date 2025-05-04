using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ArmorManager : NetworkBehaviour
{
    Image[] armorContainer;

    private PlayerData playerData;


    void Start()
    {
        playerData = gameObject.GetComponent<PlayerData>();
        armorContainer = new Image[gameObject.transform.childCount];

        for (int i = 0; i < armorContainer.Length; i++)
        {
            armorContainer[i] = gameObject.transform.GetChild(i).gameObject.GetComponent<Image>();
        }
    }



    //// Didnt know how to incorporate the index into projectile collider so i made a new thing
    //public void remove_single_latest_heart()
    //{
    //    for (int i = heart_container.Length; i-- > 0;)
    //    {
    //        if (heart_container[i].sprite != empty_heart)
    //        {
    //            print("SPRITE WAS NOT EMPTY HEART, removing heart");
    //            heart_container[i].sprite = empty_heart;
    //            return;
    //        }
    //    }
    //}
    public void AddArmorIcon(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject instantiatedArmorIcon = Instantiate(armorContainer[0].gameObject); // TODO: This requires us to have an armor i slot 0, otherwise crash
            instantiatedArmorIcon.transform.parent = gameObject.transform;

        }

    }

    //[ServerRpc]
    //public void RemoveHeartServerRpc(ulong collidedObjectsOwnerClientId)
    //{
    //    ClientRpcParams clientRpcParams = new ClientRpcParams
    //    {
    //        Send = new ClientRpcSendParams
    //        {
    //            TargetClientIds = new ulong[] { collidedObjectsOwnerClientId }
    //        }
    //    }; // List of which clients should have this code run for them
    //    RemoveHeartClientRpc(clientRpcParams);
    //}

    //// Runs for one client (or the server). Based on the provided collidedObjectsOwnerClientId
    //[ClientRpc]
    //private void RemoveHeartClientRpc(ClientRpcParams clientRpcParams = default)
    //{
    //    print("Running remove heart code in client rpc for client id: " + OwnerClientId);
    //    remove_single_latest_heart();

    //}


}
