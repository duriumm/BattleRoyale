using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Portal : NetworkBehaviour
{
    private bool isPlayerCloseToPortal = false;
    private Color portalStartingColor;
    private GameObject interactionIcon;
    private TextMeshProUGUI extractionText;
    private int extractionSecondsLeft = 10;
    private bool hasPlayerStartedExtraction = false;
    private bool isTimerReadyToCountdownOneSecond = false;
    Coroutine lastCoRoutine = null;
    private ServerObjectManager serverObjectManager;
    private GameObject playerGameObjectToExtract;
    private ulong functionCallerClientId;

    void Start()
    {
        portalStartingColor = GetComponent<Light2D>().color;
        interactionIcon = transform.Find("InteractionIcon").gameObject;
        extractionText = transform.Find("ExtractCanvas").transform.Find("ExtractText").GetComponent<TextMeshProUGUI>();
        serverObjectManager = GameObject.Find("ServerObjectSpawner").GetComponent<ServerObjectManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!isPlayerCloseToPortal) return;
        if (Input.GetKeyDown(KeyCode.E) && hasPlayerStartedExtraction == false)
        {
            extractionText.gameObject.SetActive(true);
            print("Pressed E on player with client id: " + playerGameObjectToExtract.GetComponent<PlayerData>().ownerClientId);
            //StartExitAnimation();
            PrepareStartPortalAnimation();
            hasPlayerStartedExtraction = true;
            isTimerReadyToCountdownOneSecond = true;


        }
        if (isTimerReadyToCountdownOneSecond)
        {
            if (extractionSecondsLeft <= 0) ExtractPlayer(playerGameObjectToExtract);
            else lastCoRoutine = StartCoroutine(StartExtractionTimerCountdown());
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayerCloseToPortal = true;
            print("PORTAL enter collided with client: " + collision.GetComponent<PlayerData>().ownerClientId);
            interactionIcon.SetActive(true);
            playerGameObjectToExtract = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayerCloseToPortal = false;
            //print("Portal EXIT collided with: " + collision.gameObject.GetComponent<PlayerData>().ownerClientId);
            interactionIcon.SetActive(false);
            //StopExitAnimation();
            //StopExtractAnimationForAllClientsServerRpc();
            isTimerReadyToCountdownOneSecond = false;
            hasPlayerStartedExtraction = false;
            PrepareStopPortalAnimation();
            ResetExtractionTimer();
        }
    }


    // This should happen when the player presses "E" key. To show that we are starting an extraction
    public void StartExitAnimation()
    {
        print("StartExitAnimation on player with client id: " + playerGameObjectToExtract.GetComponent<PlayerData>().ownerClientId);
        gameObject.GetComponent<Light2D>().color = Color.red;
        // TODO: Play extraction sound
    }
    public void StopExitAnimation()
    {
        gameObject.GetComponent<Light2D>().color = portalStartingColor;
        // TODO: Play extraction sound
    }

    IEnumerator StartExtractionTimerCountdown()
    {
        isTimerReadyToCountdownOneSecond = false;
        extractionSecondsLeft -= 1;
        yield return new WaitForSeconds(1.0f);
        extractionText.text = $"Extracting in: {extractionSecondsLeft}";
        isTimerReadyToCountdownOneSecond = true;
    }

    private void ResetExtractionTimer()
    {
        StopCoroutine(lastCoRoutine);
        extractionSecondsLeft = 10;
        extractionText.text = $"Extracting in: {extractionSecondsLeft}";
        extractionText.gameObject.SetActive(false);

    }

    private void ExtractPlayer(GameObject gameObjectToDestroy)
    {
        gameObjectToDestroy.GetComponent<CloudSave>().SavePlayersCoinsToCloud();
        serverObjectManager.DestroyObject(gameObjectToDestroy); // Destroy the gameobject extracted
        serverObjectManager.DestroyObject(gameObject); // Destroy this portal
    }

    #region Starting Portal Animation over network
    public void PrepareStartPortalAnimation()
    {
        print("PrepareStartPortalAnimation on player with client id: " + playerGameObjectToExtract.GetComponent<PlayerData>().ownerClientId);
        RequestStartPortalAnimationServerRpc();
        StartExitAnimation();
    }

    [ServerRpc(RequireOwnership =false)]
    public void RequestStartPortalAnimationServerRpc()
    {
        StartExtractAnimationForAllClientsClientRpc();
    }

    // Only server can call this. Will run on all clients to  start animation
    [ClientRpc]
    private void StartExtractAnimationForAllClientsClientRpc()
    {
        StartExitAnimation();
    }
    #endregion

    #region Stop Portal Animation over network
    public void PrepareStopPortalAnimation()
    {
        RequestStopExtractAnimationForAllClientsServerRpc();
        StopExitAnimation();
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestStopExtractAnimationForAllClientsServerRpc()
    {
        StoptExtractAnimationForAllClientsClientRpc();
    }

    // Only server can call this. Will run on all clients to destroy their local gameobject
    [ClientRpc]
    private void StoptExtractAnimationForAllClientsClientRpc()
    {
        StopExitAnimation();
    }
    #endregion

}
