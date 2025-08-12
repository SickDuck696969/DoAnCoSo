using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class friendlist : MonoBehaviour
{
    public UnityEngine.Transform friendspanel;
    public Button butt;
    TMP_InputField input;
    PlayerData[] sa;
    public GameObject friendprefab;
    public GameObject requestprefabe;
    public GameObject friendconfed;
    PlayerData[] re;
    public Player f;
    public List<Friend> friendsList = new List<Friend>();
    void Start()
    {
        friendspanel = transform.Find("ChildButtonName");
        input = transform.Find("InputField (TMP)")?.GetComponent<TMP_InputField>();
        butt = transform.Find("Button")?.GetComponent<Button>();

        if (butt != null)
            butt.onClick.AddListener(pullplayer);
        else
            Debug.LogError("Button not found in hierarchy");
        StartCoroutine(UpdateFriendListLoop());
    }

    public IEnumerator GetUserFromServer(string id)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://localhost/testdating/dsa.php?user_id={id}");
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
            Friend[] pulledFriends = JsonHelper.FromJson<Friend>(json);

            friendsList.Clear();
            if(pulledFriends != null)
            {
                friendsList.AddRange(pulledFriends);
            }

            Debug.Log($"Loaded {friendsList.Count} friends from server.");
        }
    }


    public void pullplayer()
    {
        KillChildrenWithTag(transform.Find("Image 1"), "friend", int.Parse(input.text));
        Debug.Log("Searching for ID: " + input?.text);
        StartCoroutine(GetUserFromServerlmao(input?.text));
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{ \"Items\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.Items;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
    public static T[] FromJsonArray<T>(string json)
    {
        string wrapped = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
        return wrapper.array;
    }

    public IEnumerator GetUserFromServerlmao(string id)
    {
        string url = "http://localhost/testdating/getalluser.php?user_id=" + id;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                GameObject friendpep = Instantiate(friendprefab, this.transform.Find("Image 1"));
                friendpep.GetComponent<friendss>().playerData = data;
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    public void KillChildrenWithTag(UnityEngine.Transform parent, string tag, int id)
    {
        // We make a temporary list because modifying children while iterating can break the loop
        var toDestroy = new List<GameObject>();

        foreach (UnityEngine.Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                toDestroy.Add(child.gameObject);
            }
        }

        // Destroy them after collecting
        foreach (var obj in toDestroy)
        {
            Destroy(obj);
        }
    }

    IEnumerator UpdateFriendListLoop()
    {
        while (true)
        {
            KillChildrenWithTag(transform.Find("Image 1"), "friend", int.Parse(f.data.user_id));
            yield return StartCoroutine(GetUserFromServer(f.data.user_id));
            foreach(Friend a in friendsList)
            {
                if (a.confirmed == 1)
                {
                    GameObject friendpep = Instantiate(friendconfed, this.transform.Find("Image 1"));
                    friendpep.GetComponent<friendlybehaviour>().data = a;
                }
                else if (a.confirmed == 0)
                {
                    GameObject friendpep = Instantiate(requestprefabe, this.transform.Find("Image 1"));
                    Debug.Log(a);
                    friendpep.GetComponent<NewMonoBehaviourScript>().c = a;
                }
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private void Update()
    {
    }
}

[System.Serializable]
public class Wrapper<T>
{
    public T[] array;
}


[System.Serializable]
public class Friend
{
    public int user_id;
    public string username;
    public string email;
    public string fullname;
    public string bday;
    public string createwhen;
    public string history;
    public int confirmed;
    public int rejected;
}
