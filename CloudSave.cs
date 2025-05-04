using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;

// This script is attached to the player
public class CloudSave : MonoBehaviour
{

    public TextMeshProUGUI coinsAmountText;
    private PlayerData playerData;
    private int playerCoinsFromDB = 0;
    private void Start()
    {
        playerData = gameObject.GetComponent<PlayerData>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    // This function saves player data to the cloud so we can see it in unity cloud save
    public async void SavePlayerDataToCloud(string key, string value)
    {
        var data = new Dictionary<string, object> { { key, value } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        print("Saved data to cloud! Data key and values: " + data.Keys + " -- " + data.Values);
    }

    public async void RetrieveKeys()
    {
        List<string> keys = await CloudSaveService.Instance.Data.RetrieveAllKeysAsync();
        print("PRINTING ALL KEYS FROM CLIOUD");
        for (int i = 0; i < keys.Count; i++)
        {
            Debug.Log(keys[i]);
        }
    }
    public async void LoadDataByKey(string key)
    {
        Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { key });

        Debug.Log("Done: " + savedData[key]);
    }

    public async void GetPlayersCoinsFromDB()
    {
        Dictionary<string, string> loadedPlayerCoins = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "coins" });
        playerCoinsFromDB = Int32.Parse(loadedPlayerCoins["coins"]);
        print("LOADED ALL COINS FROM CLOUD: " + playerCoinsFromDB);
    }

    public async void AssignPlayersCoinsOnStart()
    {
        Dictionary<string, string> loadedPlayerCoins = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "coins" });
        playerCoinsFromDB = Int32.Parse(loadedPlayerCoins["coins"]);
        print("LOADED ALL COINS FROM CLOUD: " + playerCoinsFromDB);
        playerData.IncreasePlayerCoins(playerCoinsFromDB);
    }
    public async void SavePlayersCoinsToCloud()
    {
        var data = new Dictionary<string, object> { { "coins", playerData.playerCoins } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        print("Saved data to cloud! Data key and values: " + data.Keys + " -- " + data.Values);
    }


}
