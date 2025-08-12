using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Player a;
    public Friend c;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.name = c.username;
        this.transform.Find("name").GetComponent<TMP_Text>().text = c.username;
        this.transform.Find("name (1)").GetComponent<TMP_Text>().text = c.user_id.ToString();
        this.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(() => conf(a.data.user_id, c.user_id.ToString()));
        this.transform.Find("Reject").GetComponent<Button>().onClick.AddListener(() => rej(a.data.user_id, c.user_id.ToString()));
    }
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

    IEnumerator DeleteFriend(string userId, string friendId)
    {
        DeleteRequest requestData = new DeleteRequest
        {
            user_id = userId,
            friend_id = friendId
        };

        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log(jsonData); // Should now log {"user_id":"123","friend_id":"456"}

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
}
