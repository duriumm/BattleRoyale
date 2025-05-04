using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemHotbarMarker : MonoBehaviour
{
    private GameObject itemHotbarMarker;
    public GameObject hotbarContainer;
    public Transform[] allItemSlots;
    public ItemData currentItemData;
    public int currentMarkerIndex = 0;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        itemHotbarMarker = this.gameObject;
        animator = GetComponent<Animator>();
    }
    private void UpdateMarkerData(int index)
    {
        if (currentMarkerIndex == index)
        {
            UseHotbarItem();
            allItemSlots[index].GetComponent<ItemSlot>().ClearItemSlot();
            return;
        }
        itemHotbarMarker.transform.position = allItemSlots[index].transform.position;
        currentItemData = allItemSlots[index].GetComponent<ItemData>();
        currentMarkerIndex = index;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateMarkerData(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateMarkerData(1);
        }    
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdateMarkerData(2);
        }    
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UpdateMarkerData(3);
        }    
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UpdateMarkerData(4);
        }    
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            UpdateMarkerData(5);
        }    
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            UpdateMarkerData(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            UpdateMarkerData(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            UpdateMarkerData(8);
        }
    }
    public void UseHotbarItem()
    {
        animator.SetTrigger("UseItem");
        currentItemData.UseHotbarItem();
    }
}
