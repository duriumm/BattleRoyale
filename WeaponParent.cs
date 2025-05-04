using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;

public class WeaponParent : NetworkBehaviour
{
    public bool isMeleeWeaponEquipped = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            Vector2 mousePointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.right = (mousePointerPos - (Vector2)transform.position).normalized;

            //if (isMeleeWeaponEquipped)
            //{
            //    Vector2 direction = (mousePointerPos - (Vector2)transform.position).normalized;

            //    // Flipping melee weapon sprite right/left
            //    Vector2 scale = transform.localScale;
            //    if (direction.x > 0) 
            //    { 
            //        scale.y = -1; 
            //    }
            //    else 
            //    {
            //        scale.y = 1;
            //    }

            //    transform.localScale = scale;
            //}
        }
    }
}
