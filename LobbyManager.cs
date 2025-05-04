using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class LobbyManager : NetworkBehaviour
{
    // Lobby manager keeps track of what class you chose and what equipment you selected to start with
    // All of this happens before the game starts
    public enum ChosenClass
    {
        Warrior,
        Ranger,
        Mage
    };
    public ChosenClass chosenClass;
    public TextMeshProUGUI coinsAmountText;
    public TextMeshProUGUI playerNameText;
    public TMP_InputField nameInputField;
    public Sprite[] playerClassSprites; // 0=warrior, 1=ranger, 2=mage
    public Sprite selectedClassSprite;

    public AudioListener lobbyAudioListener; // This gets removed when game starts and is put on player instead

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetClass(string classToSet)
    {


        switch (classToSet)
        {
            case "Warrior":
                chosenClass = ChosenClass.Warrior;
                selectedClassSprite = playerClassSprites[0];
                break;
            case "Ranger":
                chosenClass = ChosenClass.Ranger;
                selectedClassSprite = playerClassSprites[1];
                break;
            case "Mage":
                chosenClass = ChosenClass.Mage;
                selectedClassSprite = playerClassSprites[2];
                break;
            default:
                chosenClass = ChosenClass.Warrior;
                selectedClassSprite = playerClassSprites[0];
                break;
        }
    }

    public async void SetPlayerCoinsFromDB()
    {
        Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "coins" });

        Debug.Log("Coins gotten from DB is: " + savedData["coins"]);
        coinsAmountText.text = $"x {savedData["coins"]}";
    }
    public async void SetPlayerNameFromDB()
    {
        string key = "name";
        Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { key });

        if (savedData.ContainsKey(key))
        {
            print("Loaded player name is:" + savedData[key]);
            playerNameText.text = savedData[key];
        }
        else
        {
            print("Could not get player name. Setting it to Unnamed and saving Unnamed to DB");
            playerNameText.text = "Unnamed";
            SavePlayerNameToCloud();
            return;
        }
    }

    public async void SavePlayerNameToCloud()
    {
        string key = "name";
        string value = nameInputField.text;
        var data = new Dictionary<string, object> { { key, value } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        playerNameText.text = value;
    }
    public async void SavePlayerDataToCloud(string key, string value)
    {
        var data = new Dictionary<string, object> { { key, value } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
    }


    // We only want to start the lobby functions AFTER we have signed in from "TestRelay" script
    public void StartLobbyFunctions()
    {
        SetPlayerCoinsFromDB();
        SetPlayerNameFromDB();
    }

}
