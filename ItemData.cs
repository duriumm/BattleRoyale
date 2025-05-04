using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ItemData : MonoBehaviour
{
    // Data about the item
    public string itemName;
    public string itemDescription;
    public int itemDamage;
    public int itemArmor;
    public Sprite itemSprite;
    public string itemUsageText;
    public int healingAmount;

    // Other scripts references
    private PlayerHealth playerHealth;
    private NetworkManager networkManager;

    public enum ItemType
    {
        Coin,
        Healing,
        Weapon,
        Helmet,
        Armor,
        Gloves
    };
    public ItemType itemType;


    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        // TODO: Try get local owner player to assign health.
        // TODO: There is several other ways of doing this!!! do it better
        var myPlayer = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject;
        print("LOCAL PLAYER IS: "+myPlayer.name);
        var playerObject = GameObject.Find(myPlayer.name).gameObject;
        print("player object found is: "+playerObject.name);
    }
    public void UseHotbarItem()
    {
        if(itemName == null || itemName.Length == 0 || itemName == "")
        {
            print("ITEM NAME IS NULL, LEN(0) OR ''. DONT GET ITEM");
            return;
        }
        if(itemType == ItemType.Healing) 
        {
            // TODO: Set healiong items to heal player
            // TODO: remove item from hotbars
        }
        print("ITEM IS USED IN ITEMDATA for item: "+itemName);
        
    }

}
