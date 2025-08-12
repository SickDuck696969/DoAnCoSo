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
    public bool hostready = false;
    public bool clientready = false;

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
            togame.onClick.AddListener(togamea);
        }

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

    private void togamea()
    {
        if(ng.status == "host")
        {
            hostready = true;
        }
        else
        {
            {
                clientready = true;
            }
        }
    }

    private void Update()
    {
        if (hostready && clientready)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
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
        NetworkManager.Singleton.StartHost();
        ng.pColor = "White";
        NetworkManager.Singleton.SceneManager.LoadScene("VarianSelection", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    IEnumerator GetDateFromServer()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/testdating/getthedate.php");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to contact server: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            Debug.Log("Server time is: " + json);
        }
    }
    private void clientclick()
    {
        Debug.Log("joining");
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.StartClient();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Client connected to host.");
            ng.pColor = "Black";
        }
    }

    private IEnumerator WaitForTosserSpawn()
    {
        float timeout = 5f;
        float elapsed = 0f;

        NetworkObject tosserNetworkObject = tosser.GetComponent<NetworkObject>();

        while (!tosserNetworkObject.IsSpawned && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (tosserNetworkObject.IsSpawned)
        {
            tosser.GetComponent<blackorwhite>().PlayerAdd();
        }
        else
        {
            Debug.Log("? Tosser not spawned within timeout.");
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

    [System.Serializable]
    public class UserRegisterData
    {
        public string username;
        public string email;
        public string password;
    }

}
