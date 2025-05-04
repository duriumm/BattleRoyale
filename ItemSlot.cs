using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Tooltip 
    public GameObject toolTip;
    private Vector2 toolTipPosition;
    public TextMeshProUGUI tooltipName;
    public TextMeshProUGUI tooltipDescription;
    public TextMeshProUGUI tooltipUsageInfo;

    public bool isItemSlotEmpty;

    private ItemData itemData;
    // This item slots current image component
    private Image itemImage;
    public Sprite emptyItemSlotImage;

    // test item
    private ItemData testItemToAdd;

    void Start()
    {
        toolTip = transform.parent.transform.parent.Find("ItemslotTooltip").gameObject;
        toolTipPosition = new Vector2(this.transform.position.x, this.transform.position.y + 125.0f);
        // Get current itemslots data
        itemData = gameObject.GetComponent<ItemData>();
        tooltipName = toolTip.transform.Find("ItemNameBG").transform.Find("NameText").GetComponent<TextMeshProUGUI>();

        tooltipDescription = toolTip.transform.Find("DescriptionBG").transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();

        tooltipUsageInfo = toolTip.transform.Find("UsageBG").transform.Find("UsageText").GetComponent<TextMeshProUGUI>();

        itemImage = gameObject.GetComponent<Image>();

        testItemToAdd = GameObject.Find("TEstItemaddData").GetComponent<ItemData>();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Clear item from item slot
    public void ClearItemSlot()
    {
        itemData.itemName = "";
        itemData.itemDescription = "";
        itemData.itemDamage = 0;
        itemData.itemArmor = 0;
        itemData.itemSprite = null;
        itemData.itemUsageText = "";
        itemData.healingAmount = 0;
        itemImage.sprite = null;
        // Set image to 0 alpha as to look invisible
        Color c = itemImage.color;
        c.a = 0;
        itemImage.color = c;


    }
    // WORKS, just not used yet!! Choose item data to add here for it to work :) 
    public void AddItemToItemSlot(ItemData addedItemsItemData)
    {

        itemData.itemName = addedItemsItemData.itemName;
        itemData.itemDescription = addedItemsItemData.itemDescription;
        itemData.itemDamage = addedItemsItemData.itemDamage;
        itemData.itemArmor = addedItemsItemData.itemArmor;
        itemData.itemSprite = addedItemsItemData.itemSprite;
        itemData.itemUsageText = addedItemsItemData.itemUsageText;
        itemData.healingAmount = addedItemsItemData.healingAmount;
        itemImage.sprite = itemData.itemSprite;

        // Set image to 0 alpha as to look invisible
        Color c = itemImage.color;
        c.a = 1;
        itemImage.color = c;
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetToolTipData();
        toolTip.SetActive(true);
        toolTip.transform.position = toolTipPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("EXIT");
        toolTip.SetActive(false);
    }

    // Set data to tooltip that is in this current item slot
    public void SetToolTipData()
    {
        itemData = gameObject.GetComponent<ItemData>();
        tooltipName.text = itemData.itemName;
        tooltipDescription.text = itemData.itemDescription;

        //var itemDamage = gameObject.GetComponent<ItemData>().itemDamage.ToString(); // Not used yet
        //var itemArmor = gameObject.GetComponent<ItemData>().itemArmor.ToString(); // Not used yet
        if (itemData.itemType == ItemData.ItemType.Healing)
        {
            tooltipUsageInfo.text = "Health " + ToGreenString("+" + itemData.healingAmount.ToString());
        }
    }

    private string ToGreenString(string inputString)
    {
        return $"<color=green>{inputString}";
    }
    private string ToRedString(string inputString)
    {
        return $"<color=red>{inputString}";
    }


}
