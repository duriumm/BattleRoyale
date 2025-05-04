using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDebug : NetworkBehaviour
{
    public Image playerPingLight;
    public GameObject playerWeapon; // WeaponParent gameobj
    public WeaponParent weaponParent;
    public PlayerHealth playerHealth;
    public TextMeshProUGUI healthText;

    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region SPECIFIC FUNCTION BUTTON WHICH ROTATES SPEAR POSITION
    // Prepare Specific button to be used locally and send to others that my character shall use specific button on their clients
    public void PrepareSpecificButton()
    {
        Debug.Log("Preparing specific button");
        RequestToUseSpecificButtonServerRpc();
        UseSpecificButton();
    }
    // Set my own UseSpecificButton
    public void UseSpecificButton()
    {
        Debug.Log("SPECIFIC BUTTON USED TO LOWER HP");
        // ENTER WHATEVER FUNCTIONALITY YOU WANT TO TEST HERE //
        playerHealth.DecreasePlayerHealthPoints(1);
    }
    // Request the server to send out my usage of UseSpecificButton to all other cliends
    [ServerRpc]
    private void RequestToUseSpecificButtonServerRpc()
    {
        UseSpecificButtonClientRpc();
        Debug.Log("Server is UseSpecificButtonClientRpct client rpc");
    }
    // Set MY UseSpecificButton for all other clients (but not myself since i already did it)
    [ClientRpc]
    private void UseSpecificButtonClientRpc()
    {
        if (!IsOwner)
        {
            UseSpecificButton();
            Debug.Log("Client is GETTING UseSpecificButton client rpc");
        }
    }
    #endregion

    #region SET PLAYER RED PING LIGHT + OVER NETWORK
    // Prepare by sending red light rpc request over network and then setting my own light red
    public void PrepareSetPlayerPingLightRed()
    {
        Debug.Log("Preparing red light");
        RequestToSetRedLightServerRpc();
        SetPlayerPingLightRed();
    }
    // Set my own light red
    public void SetPlayerPingLightRed()
    {
        Debug.Log("Setting red light");
        playerPingLight.GetComponent<Image>().color = Color.red;
    }
    // Request the server to send out my usage of red light to all other cliends
    [ServerRpc]
    private void RequestToSetRedLightServerRpc()
    {
        SetPlayerPingLightRedClientRpc();
        Debug.Log("Server is sending red light client rpc");
    }
    // Set MY light red for all other clients (but not myself since i already did it)
    [ClientRpc]
    private void SetPlayerPingLightRedClientRpc()
    {
        if (!IsOwner)
        {
            SetPlayerPingLightRed();
            Debug.Log("Client is GETTING red light client rpc");
        }
    }
    #endregion

    #region SET PLAYER GREEN PING LIGHT + OVER NETWORK
    // Prepare by sending green light rpc request over network and then setting my own light green
    public void PrepareSetPlayerPingLightGreen()
    {
        RequestToSetGreenLightServerRpc();
        SetPlayerPingLightGreen();
    }
    // Set my own light green
    public void SetPlayerPingLightGreen()
    {
        playerPingLight.GetComponent<Image>().color = Color.green;
    }
    // Request the server to send out my usage of green light to all other cliends
    [ServerRpc]
    private void RequestToSetGreenLightServerRpc()
    {
        SetPlayerPingLightGreenClientRpc();
    }
    // Set MY light green for all other clients (but not myself since i already did it)
    [ClientRpc]
    private void SetPlayerPingLightGreenClientRpc()
    {
        if (!IsOwner)
        {
            SetPlayerPingLightGreen();
        }
    }
    #endregion
}
