using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private ServerObjectManager serverObjectManager;
    private PlayerData playerData;
    private GameObject interactionIcon;
    private bool canPlayerCollectItem = false;
    private SpriteRenderer collectibleItemSprite;
    private SpriteRenderer playerHelmetSprite;
    private PlayerHealth playerHealth;
    private ItemData itemData;
    public enum CollectibleType
    {
        Coin,
        HealingPotion,
        Weapon,
        Helmet,
        Armor,
        Gloves
    };
    public CollectibleType collectibleType;
    void Start()
    {
        serverObjectManager = GameObject.Find("ServerObjectSpawner").GetComponent<ServerObjectManager>();
        interactionIcon = transform.Find("InteractionIcon").gameObject;
        collectibleItemSprite = gameObject.GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        // This is not for coins since coins picks up instantly. This is for equipment etc :)
        if (!canPlayerCollectItem) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUpItem();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            interactionIcon.SetActive(true);
            canPlayerCollectItem = true;
            playerData = collider.GetComponent<PlayerData>();
            playerHealth = collider.GetComponent<PlayerHealth>();
            if (collectibleType == CollectibleType.Coin)
            {
                print("Player picked up a coin!");
                playerData.IncreasePlayerCoins(1);
                serverObjectManager.DestroyObject(gameObject);
            }
            else if (collectibleType == CollectibleType.Helmet) // TODO: we can just put elsehere
            {
                itemData = collider.transform.Find("Helmet").GetComponent<ItemData>();
                playerHelmetSprite = collider.transform.Find("Helmet").GetComponent<SpriteRenderer>();
                print("HELMET");


            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            interactionIcon.SetActive(false);
            canPlayerCollectItem = false;
        }

    }

    public void PickUpItem()
    {

        if (collectibleType == CollectibleType.Helmet)
        {

            //playerHelmetSprite.sprite = collectibleItemSprite.sprite;
            //print("itemArmor is: " + itemData.itemArmor);
            //playerHealth.IncreasePlayerArmorPoints(itemData.itemArmor);
        }
    }
}
