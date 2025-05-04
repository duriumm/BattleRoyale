using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTimer : MonoBehaviour
{
    public float timeToDeath = 6f;
    private ServerObjectManager serverObjectManager;


    private void Start()
    {
        serverObjectManager = GameObject.Find("ServerObjectSpawner").GetComponent<ServerObjectManager>();
    }
    // Countdown to destroy this object
    void Update()
    {
        timeToDeath -= Time.deltaTime;

        if(timeToDeath < 0)
        {
            print("desstroying gameobject from TIMER");
            if(gameObject != null)
            {
                serverObjectManager.DestroyObject(gameObject);  

            }
        }
    }
}
