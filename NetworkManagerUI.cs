using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    public TestRelay testRelay;
    public TMP_InputField inputField;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button createRelayButton;
    [SerializeField] private Button joinRelayButton;

    private void Awake()
    {
        testRelay = GameObject.Find("TestRelay").GetComponent<TestRelay>();
        inputField = GameObject.Find("InputField").GetComponent<TMP_InputField>();
        print("Input field in awake is: " + inputField);

        //serverButton.onClick.AddListener(() =>
        //{
        //    NetworkManager.Singleton.StartServer();
        //});

        //hostButton.onClick.AddListener(() =>
        //{
        //    NetworkManager.Singleton.StartHost();
        //});

        //clientButton.onClick.AddListener(() =>
        //{
        //    NetworkManager.Singleton.StartClient();
        //});

        createRelayButton.onClick.AddListener(() =>
        {
            testRelay.CreateRelay();
        });

        joinRelayButton.onClick.AddListener(() =>
        {
            string textVal = inputField.text;
            Debug.Log("inputfield text is: " + textVal);
            testRelay.JoinRelay(inputField.text);
        });


    }
}
