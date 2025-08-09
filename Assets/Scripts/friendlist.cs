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
    public UnityEngine.Transform friendslist;
    public Button butt;
    TMP_InputField input;
    PlayerData[] sa;
    public GameObject friendprefab;
    public GameObject requestprefabe;
    PlayerData[] re;
    public Player f;

    void Start()
    {
        StartCoroutine(GetUserFromServer(f.data.user_id));
        friendslist = transform.Find("ChildButtonName");
        input = transform.Find("InputField (TMP)")?.GetComponent<TMP_InputField>();
        butt = transform.Find("Button")?.GetComponent<Button>();

        if (butt != null)
            butt.onClick.AddListener(pullplayer);
        else
            Debug.LogError("Button not found in hierarchy");
    }

    public IEnumerator GetUserFromServer(string id)
    {
        KillChildrenWithTag(transform.Find("Image 1"), "friend", int.Parse(f.data.user_id));
        UnityWebRequest www = UnityWebRequest.Get($"http://localhost/testdating/dsa.php?user_id={id}");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to contact server: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            Debug.Log(json);
            re = FromJsonArray<PlayerData>(json);
            Debug.Log($"http://localhost/testdating/dsa.php?user_id={id}");
            Debug.Log(re);
            foreach (PlayerData s in re)
            {
                GameObject a = Instantiate(requestprefabe, transform.Find("Image 1"));
                a.GetComponent<NewMonoBehaviourScript>().c = s;
                Debug.Log("Friend added: " + s.username);
            }
        }
    }

    public void pullplayer()
    {
        KillChildrenWithTag(transform.Find("Image 1"), "friend", int.Parse(input.text));
        Debug.Log("Searching for ID: " + input?.text);
        StartCoroutine(GetUserFromServer());
    }

    public static T[] FromJsonArray<T>(string json)
    {
        string wrapped = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
        return wrapper.array;
    }

    public IEnumerator GetUserFromServer()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/testdating/getalluser.php");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to contact server: " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;
        Debug.Log("Server response: " + json);

        try
        {
            // Use array parsing since PHP usually returns raw array
            sa = FromJsonArray<PlayerData>(json);
            Debug.Log("Pulled " + sa.Length + " players from server.");
        }
        catch
        {
            Debug.LogError("JSON parsing failed. Check format.");
            yield break;
        }

        foreach (PlayerData s in sa)
        {
            if (!string.IsNullOrEmpty(s.user_id) && s.user_id == input.text)
            {
                GameObject a = Instantiate(friendprefab, transform.Find("Image 1"));
                a.GetComponent<friendss>().playerData = s;
                Debug.Log("Friend added: " + s.username);
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
}

[System.Serializable]
public class Wrapper<T>
{
    public T[] array;
}
