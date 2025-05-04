using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class PlayerData : NetworkBehaviour
{
    
    public int playerCoins = 0;
    private TextMeshProUGUI coinsAmountText;
    public string playerName;
    public TextMeshProUGUI debugWindowPlayerNameText;


    public ulong ownerClientId; // We can always get the local players ownerClient id here since THIS SCRIPT is attached to our player
    

    void Start()
    {
        if (IsOwner)
        {
            coinsAmountText = GameObject.Find("CoinsAmountText").GetComponent<TextMeshProUGUI>();
            // Only set client it to this is you are owner. Otherwise you set others client id to your??
            // Not working currently. We need to get the cliend id other way
            ownerClientId = OwnerClientId;

            // Set lobby canvas chosen name to player
            LobbyManager lobbyManager = GameObject.Find("LobbyCanvas").GetComponent<LobbyManager>();
            playerName = lobbyManager.playerNameText.text;

            transform.parent.Find("DebugCanvas").transform.Find("DebugBackground").transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text = playerName;

            // Set gameobject to playername
            transform.parent.name = $"Player - " + playerName;

        }
    }


    public void IncreasePlayerCoins(int amount)
    {
        if (IsOwner)
        {
            playerCoins += amount;
            coinsAmountText.text = "x " + playerCoins;
        }
    }

}
