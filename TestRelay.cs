using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class TestRelay : MonoBehaviour
{
    public NetworkManagerUI networkManagerUI;
    public GameObject lobbyScreen;
    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // Test with saving data to cloud
        var data = new Dictionary<string, object> { { "MySaveKey", "HelloWorld" } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);

        // After we signed in we can start the Lobby functions
        LobbyManager lobbyManager = GameObject.Find("LobbyCanvas").GetComponent<LobbyManager>();
        lobbyManager.StartLobbyFunctions();

    }

    
    public async void CreateRelay()
    {
        Debug.Log("Inside create relay");
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("joinCode is: " + joinCode);
            networkManagerUI.inputField.text = joinCode;

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            // Need to get/send all this data
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            lobbyScreen.SetActive(false);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinRelay(string inputJoinCode)
    {
        // Get the code from a gameobject joinCodes childrens name
        //string joinCode = GameObject.Find("joinCode").transform.GetChild(0).name;
        Debug.Log("inputJoinCode from scene obj is: " + inputJoinCode);
        try
        {
            inputJoinCode = inputJoinCode.Substring(0, 6);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(inputJoinCode);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            lobbyScreen.SetActive(false);

        }
        catch (RelayServiceException e)
        {

            Debug.Log(e);
        }
    }

}
