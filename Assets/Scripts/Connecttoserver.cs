using NUnit.Framework;
using Unity.Multiplayer.Playmode;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class Connecttoserver : MonoBehaviour
{
    public HostStarter hoststarter;
    public ClientConnector connector;
    [SerializeField] private Button host;
    [SerializeField] private Button client;
    [SerializeField] private Button passnplay;
    [SerializeField] private Button AIplay;
    public GameObject tosser;
    [SerializeField] private GameObject usernamein;
    [SerializeField] private GameObject emailin;
    [SerializeField] private GameObject passswordin;
    [SerializeField] private Button Registerbt;
    [SerializeField] private GameObject emaillogin;
    [SerializeField] private GameObject passswordlogin;
    [SerializeField] private Button loginbt;

    [SerializeField] private Button Playhost;
    [SerializeField] private Button Playclient;
    [SerializeField] private Button togame;
    public Player ng;
    public NetworkVariable<bool> blackready =
        new NetworkVariable<bool>();
    public NetworkVariable<bool> whiteready =
        new NetworkVariable<bool>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (host != null)
        {
            host.onClick.AddListener(hostclick);
        }

        if (client != null)
        {
            client.onClick.AddListener(clientclick);
        }

        if (passnplay != null)
        {
            passnplay.onClick.AddListener(offclick);
        }

        if (Registerbt != null)
        {
            Registerbt.onClick.AddListener(test);
        }

        if (loginbt != null)
        {
            loginbt.onClick.AddListener(login);
        }

        if (AIplay != null)
        {
            AIplay.onClick.AddListener(playAI);
        }

        if (Playhost != null)
        {
            Playhost.onClick.AddListener(hostapplycolor);
            Playclient.onClick.AddListener(clientapplycolor);
        }
        if (togame != null)
        {
            togame.onClick.AddListener(togameaServerRpc);
        }

        blackready.Value = false;
        whiteready.Value = false;

    }

    private void hostapplycolor()
    {
        tosser.GetComponent<blackorwhite>().ng.pColor = null;
        tosser.GetComponent<blackorwhite>().ng.status = "host";
        tosser.GetComponent<blackorwhite>().PlayerAdd();
        if(tosser.GetComponent<blackorwhite>().ng.pColor != null)
        {
            if (tosser.GetComponent<blackorwhite>().ng.pColor == "Black")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    "VarianSelection",
                    UnityEngine.SceneManagement.LoadSceneMode.Single
                );
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    "VarianSelectionbl",
                    UnityEngine.SceneManagement.LoadSceneMode.Single
                );
            }
        }
    }
    private void clientapplycolor()
    {
        tosser.GetComponent<blackorwhite>().ng.pColor = null;
        tosser.GetComponent<blackorwhite>().ng.status = "client";
        tosser.GetComponent<blackorwhite>().PlayerAdd();
        if (tosser.GetComponent<blackorwhite>().ng.pColor != null)
        {
            if (tosser.GetComponent<blackorwhite>().ng.pColor == "Black")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    "VarianSelection",
                    UnityEngine.SceneManagement.LoadSceneMode.Single
                );
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    "VarianSelectionbl",
                    UnityEngine.SceneManagement.LoadSceneMode.Single
                );
            }
        }
    }
    private void playAI()
    {
        Debug.Log("play AI");
        SceneManager.LoadScene("GameAI");
    }

    [ServerRpc(RequireOwnership = false)]
    private void togameaServerRpc()
    {
    }

    private void login()
    {
        Debug.Log("logging in");
        StartCoroutine(ng.GetUserFromServer(emaillogin.GetComponent<TMPro.TMP_InputField>().text, passswordlogin.GetComponent<TMPro.TMP_InputField>().text));
    }

    private void test()
    {
        Register(usernamein.GetComponent<TMPro.TMP_InputField>().text, emailin.GetComponent<TMPro.TMP_InputField>().text, passswordin.GetComponent<TMPro.TMP_InputField>().text);
    }

    private void offclick()
    {
        Debug.Log("Pass n Play");
        SceneManager.LoadScene("GameOffline");
    }

    private void hostclick()
    {
        Debug.Log("hosting");
        hoststarter.StartHost();
        tosser.GetComponent<blackorwhite>().PlayerAdd();
        NetworkManager.Singleton.SceneManager.LoadScene("VarianSelection", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    private void clientclick()
    {
        Debug.Log("joining");
        TMP_InputField a = client.transform.parent.GetComponentInChildren<TMP_InputField>();
        connector.serverIP = a.text;

        // subscribe once before connecting
        NetworkManager.Singleton.OnClientConnectedCallback += OnJoinedAsClient;

        connector.ConnectToHost();
    }

    private void OnJoinedAsClient(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Client successfully connected, running PlayerAdd...");

            tosser.GetComponent<blackorwhite>()?.PlayerAdd();

            // unsubscribe so it only fires once
            NetworkManager.Singleton.OnClientConnectedCallback -= OnJoinedAsClient;
        }
    }

    public void Register(string username, string email, string password)
    {
        StartCoroutine(PostRegister(username, email, password));
    }

    private IEnumerator PostRegister(string username, string email, string password)
    {
        UserRegisterData data = new UserRegisterData
        {
            username = username,
            email = email,
            password = password
        };

        string jsonData = JsonUtility.ToJson(data);
        Debug.Log("Sending JSON: " + jsonData);

        using (UnityWebRequest www = new UnityWebRequest("http://localhost/testdating/postuser.php", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Success: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + www.responseCode + " " + www.error);
            }
        }
    }
    private IEnumerator Delayedconnection()
    {
        yield return new WaitForSeconds(3f);
    }

    [System.Serializable]
    public class UserRegisterData
    {
        public string username;
        public string email;
        public string password;
    }

}
