using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
///  THIS SCRIPT IS NOT USED, WE WORK WITH SPRITE MASKS INSTEAD
///  IF YOU NEED TO CHECK IF AN OBJECT IS IN YOUR LINE OF SIGHT YOU CAN USE THIS SCRIPT
/// </summary>
public class VisionCone : MonoBehaviour
{
    public bool isPlayerRenderingEnabled = true;
    public bool isRenderScriptReadyToRun = false;
    public GameObject topParentGameobject;


    // Be sure that the fovAngle is set to the same of your point light
    public float fovAngle = 90f;
    public Transform fovPoint;
    public float range = 8;

    public Transform target;

    public GameObject[] listOfPlayers; // List of players so we can switch on/off their sprite renderers

    // TODO: Currently we only act on one enemy player.
    // In the future we will need to act on all the players in the scene
    private void Start()
    {
        // STart coroutine to run 5 times / second instead of 60
        StartCoroutine(CheckIfPlayerIsInLineOfSightCoroutine());
    }
    void Update()
    {
        //CheckIfPlayerIsInLineOfSight();
        //if (!isRenderScriptReadyToRun)
        //{
        //    return;
        //}

        //if(!isPlayerRenderingEnabled)
        //{
        //    SetTargetGameObjectInvisible(topParentGameobject);
        //}
        //else if (isPlayerRenderingEnabled)
        //{
        //    SetTargetGameObjectVisible(topParentGameobject);
        //}

        ////////////////////////////////
        ///

    }
    IEnumerator CheckIfPlayerIsInLineOfSightCoroutine()
    {
        {
            while (true)
            {
                // Do your code
                CheckIfPlayerIsInLineOfSight();

                // wait for seconds
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    // When player walks out from vision field we should not see him
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    print("Collided!!!");
    //    if (collision.CompareTag("Player"))
    //    {
    //        SetTargetGameObjectInvisible(collision.gameObject);
    //    }
    //}
    // TODO: These parts below can be optimized instead of get object each time
    // and declaring variables
    void CheckIfPlayerIsInLineOfSight()
    {

        Vector2 dir = target.position - transform.position;
        float angle = Vector3.Angle(dir, fovPoint.up);
        RaycastHit2D r = Physics2D.Raycast(fovPoint.position, dir, range);

        print("We collided with: " + r.collider.name);
        if (angle < fovAngle / 2)
        {
            if (r.collider.CompareTag("Player"))
            {
                
                GameObject collidedGameobject = r.collider.gameObject;
                SetTargetGameObjectVisible(collidedGameobject);
                // WE SPOTTED THE PLAYER!
                print("SEEN!");
                Debug.DrawRay(fovPoint.position, dir, Color.red);
            }
            else
            {
                print("we dont seen");
                
            }
        }
    }
    void SetTargetGameObjectInvisible(GameObject target)
    {
        foreach (SpriteRenderer r in target.GetComponentsInChildren(typeof(SpriteRenderer)))
        {
            print("setting player sprite renderer to OFF");
            r.enabled = false;
            target.GetComponent<ShadowCaster2D>().castsShadows = false;
        }
    }
    void SetTargetGameObjectVisible(GameObject target)
    {
        foreach (SpriteRenderer r in target.GetComponentsInChildren(typeof(SpriteRenderer)))
        {
            print("setting player sprite renderer to ON");
            r.enabled = true;
            target.GetComponent<ShadowCaster2D>().castsShadows = true;

        }
    }

}
