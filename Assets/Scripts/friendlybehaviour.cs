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
    // -------------------- Data Models --------------------
    [System.Serializable]
    public class ConfirmRequest
    {
        public string user_id;
        public string friend_id;
    }

    [System.Serializable]
    public class DeleteRequest
    {
        public string user_id;
        public string friend_id;
    }

    // -------------------- Fields --------------------
    public Player f;
    public List<fade> fades = new List<fade>();
    public GameObject button;
    public Friend data;
    public GameObject ipbox;
    public HostStarter hoststarter;
    public ClientConnector connector;

    // -------------------- Unity Methods --------------------
    void Start()
    {
        ipbox = GameObject.Find("ipbox");

        if (data.confirmed == 1)
        {
            StartCoroutine(UpdateProblemListLoop());
        }
        else
        {
            if (f.data.user_id == data.user_id.ToString())
            {
                transform.Find("waittext").GetComponent<TMP_Text>().text = "wait for bro...";
            }
            else
            {
                this.transform.Find("name").GetComponent<TMP_Text>().text = data.username;
                this.transform.Find("Id").GetComponent<TMP_Text>().text = data.friend_id.ToString();

                GameObject buttonyes = Instantiate(button, this.transform.Find("Image"));
                buttonyes.transform.GetComponentInChildren<TMP_Text>().text = "Confirm";
                buttonyes.GetComponent<Button>().onClick.AddListener(() => conf(f.data.user_id, data.user_id.ToString()));

                GameObject buttonno = Instantiate(button, this.transform.Find("Image"));
                buttonno.transform.GetComponentInChildren<TMP_Text>().text = "Reject";
                buttonno.GetComponent<Button>().onClick.AddListener(() => rej(f.data.user_id, data.user_id.ToString()));
            }
        }
    }

    void Update()
    {
        transform.Find("name").GetComponent<TMP_Text>().text = data.username;
        transform.Find("Id").GetComponent<TMP_Text>().text = data.user_id.ToString();
    }

    // -------------------- Coroutines --------------------
    IEnumerator UpdateProblemListLoop()
    {
        foreach (UnityEngine.Transform child in transform.Find("Image"))
        {
            Destroy(child.gameObject);
        }

        while (true)
        {
            yield return StartCoroutine(GetProblemFromServer(f.data.user_id));

            foreach (fade fade in fades)
            {
                Debug.Log(fade.friend_id);

                if (f.data.user_id == fade.user_id.ToString())
                {
                    transform.Find("waittext").GetComponent<TMP_Text>().text = "wait for bro...";
                }
                else if (f.data.user_id == fade.friend_id.ToString())
                {
                    string scrapip = fade.ip;

                    transform.Find("message").GetComponent<TMP_Text>().text = "run the fade?(...)";

                    GameObject buttonyes = Instantiate(button, this.transform.Find("Image"));
                    buttonyes.GetComponent<Button>().onClick.AddListener(() => runthefade(scrapip));
                    buttonyes.transform.GetComponentInChildren<TMP_Text>().text = "Bet";

                    GameObject buttonno = Instantiate(button, this.transform.Find("Image"));
                    buttonno.transform.GetComponentInChildren<TMP_Text>().text = "duck like a Bitch";
                    buttonno.GetComponent<Button>().onClick.AddListener(() => duck());
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
            string json = www.downloadHandler.text;
            fade[] pulledFriends = JsonHelper.FromJson<fade>(json);

            fades.Clear();
            if (pulledFriends != null)
            {
                fades.AddRange(pulledFriends);
            }

            Debug.Log($"Loaded {fades.Count} friends from server.");
        }
    }

    IEnumerator settleFade(string userId)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://localhost/testdating/dropafade.php?user_id={userId}");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to contact server: " + www.error);
        }
        else
        {
            Debug.Log("Fade settled");
        }
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

    IEnumerator DeleteFriend(string userId, string friendId)
    {
        DeleteRequest requestData = new DeleteRequest
        {
            user_id = userId,
            friend_id = friendId
        };

        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log(jsonData);

        using (UnityWebRequest www = new UnityWebRequest("http://localhost/testdating/delete_friend.php", "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("Server: " + www.downloadHandler.text);
            else
                Debug.LogError("Error: " + www.error);
        }
    }

    IEnumerator ConfirmFriend(string userId, string friendId)
    {
        ConfirmRequest requestData = new ConfirmRequest
        {
            user_id = friendId,
            friend_id = userId
        };

        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://localhost/testdating/conf.php", jsonData))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log(www.downloadHandler.text);
            else
                Debug.LogError(www.error);
        }

        Destroy(gameObject);
    }

    // -------------------- Actions --------------------
    public void duck()
    {
        StartCoroutine(settleFade(f.data.user_id));
        StartCoroutine(UpdateProblemListLoop());
    }

    public void runthefade(string ip)
    {
        Debug.Log(ip);
        StartCoroutine(settleFade(f.data.user_id));
        connector.serverIP = ip;
        connector.ConnectToHost();
    }

    public void startsomething()
    {
        Destroy(GameObject.FindGameObjectWithTag("skillbutt"));

        if (f.data.user_id == data.user_id.ToString())
        {
            StartCoroutine(PostFade(int.Parse(f.data.user_id), data.friend_id, ipbox.GetComponent<TMP_Text>().text, "suck it"));
        }
        else
        {
            StartCoroutine(PostFade(int.Parse(f.data.user_id), data.user_id, ipbox.GetComponent<TMP_Text>().text, "suck it"));
        }

        if (!NetworkManager.Singleton.IsHost)
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            hoststarter.StartHost();
    }

    public void conf(string userId, string friendId)
    {
        StartCoroutine(ConfirmFriend(userId, friendId));
    }

    public void rej(string userId, string friendId)
    {
        Debug.Log(userId + friendId);
        StartCoroutine(DeleteFriend(userId, friendId));
        Destroy(gameObject);
    }

    public void OnClientDisconnected(ulong clientId)
    {
        StartCoroutine(settleFade(f.data.user_id));
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}
