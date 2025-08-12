using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static friendlist;
using static UnityEngine.RuleTile.TilingRuleOutput;
[System.Serializable]
public class fade
{
    public int user_id;
    public int friend_id;
    public string ip;
    public string message;
}
public class friendlybehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player f;
    public List<fade> fades = new List<fade>();
    public GameObject button;
    public Friend data;
    public GameObject ipbox;
    public ClientConnector connector;
    void Start()
    {
        ipbox = GameObject.Find("ipbox");
        StartCoroutine(UpdateProblemListLoop());
    }
    IEnumerator UpdateProblemListLoop()
    {
        foreach (UnityEngine.Transform child in transform.Find("Image"))
        {
            Destroy(child.gameObject);
        }
        while (true)
        {
            yield return StartCoroutine(GetProblemFromServer(f.data.user_id));
            foreach (fade fade in fades) {
                Debug.Log(fade.friend_id);
                if(f.data.user_id == fade.user_id.ToString())
                {
                    transform.Find("waittext").GetComponent<TMP_Text>().text = "wait for bro...";
                }else if(f.data.user_id == fade.friend_id.ToString())
                {
                    transform.Find("message").GetComponent<TMP_Text>().text = "run the fade?(...)";
                    GameObject buttonyes = Instantiate(button, this.transform.Find("Image"));
                    buttonyes.GetComponent<Button>().onClick.AddListener(() => runthefade(fade.user_id.ToString()));
                    buttonyes.transform.GetComponentInChildren<TMP_Text>().text = "Bet";
                    GameObject buttonno = Instantiate(button, this.transform.Find("Image"));
                    buttonno.transform.GetComponentInChildren<TMP_Text>().text = "duck like a Bitch";
                }
            }
            if (fades.Count <= 0)
            {
                GameObject buttonscrap = Instantiate(button, this.transform.Find("Image"));
                buttonscrap.transform.GetComponentInChildren<TMP_Text>().text = "start something";
                buttonscrap.GetComponent<Button>().onClick.AddListener(() => startsomething());
            }
            yield return new WaitForSeconds(5f);
        }
    }
    public IEnumerator GetProblemFromServer(string id)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://localhost/testdating/getfade.php?user_id={id}");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to contact server: " + www.error);
        }
        else
        {
            // Parse JSON response into a list
            string json = www.downloadHandler.text;

            // Wrap array if necessary for JsonUtility
            fade[] pulledFriends = JsonHelper.FromJson<fade>(json);

            fades.Clear();
            if (pulledFriends != null)
            {
                fades.AddRange(pulledFriends);
            }

            Debug.Log($"Loaded {fades.Count} friends from server.");
        }
    }

    public void runthefade(string ip)
    {
        connector.serverIP = ip;
        connector.ConnectToHost();
    }

    IEnumerator PostFade(int userId, int friendId, string ip, string message)
    {
        fade data = new fade
        {
            user_id = userId,
            friend_id = friendId,
            ip = ip,
            message = message
        };

        string jsonData = JsonUtility.ToJson(data);
        using (UnityWebRequest www = new UnityWebRequest("http://localhost/testdating/postfade.php", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Server Response: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }
    public void startsomething()
    {
        Destroy(GameObject.FindGameObjectWithTag("skillbutt"));
        StartCoroutine(PostFade(int.Parse(f.data.user_id), data.user_id, ipbox.GetComponent<TMP_Text>().text, "suck it"));
        Debug.Log("hosting");
        if (!NetworkManager.Singleton.IsHost)
            NetworkManager.Singleton.StartHost();
    }
    // Update is called once per frame
    void Update()
    {
    }
}

public class ClientConnector : MonoBehaviour
{
    public string serverIP = "192.168.1.15"; 
    public ushort serverPort = 7777;         

    public void ConnectToHost()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = serverIP;
            transport.ConnectionData.Port = serverPort;
        }
        else
        {
            Debug.LogError("UnityTransport not found on NetworkManager.");
            return;
        }

        NetworkManager.Singleton.StartClient();
    }
}
