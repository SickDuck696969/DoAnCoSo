using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using static Connecttoserver;

public class friendss : MonoBehaviour
{
    public PlayerData playerData;
    public Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.name = playerData.username;
        this.transform.Find("name").GetComponent<TMP_Text>().text = playerData.username;
        this.transform.Find("name (1)").GetComponent<TMP_Text>().text = playerData.user_id;
        this.transform.Find("Request").GetComponent<Button>().onClick.AddListener(sendrequest);
    }


    public void sendrequest()
    {
        if(player.data.user_id != playerData.user_id)
        {
            Debug.Log(int.Parse(player.data.user_id) + "" + int.Parse(playerData.user_id));
            StartCoroutine(PostRequest(int.Parse(player.data.user_id), int.Parse(playerData.user_id)));
        }
        else
        {
            Debug.Log("sad");
        }
    }

    private IEnumerator PostRequest(int a, int b)
    {
        UserRegisterData data = new UserRegisterData
        {
            user_id = a,
            friend_id = b,
        };

        string jsonData = JsonUtility.ToJson(data);
        Debug.Log("Sending JSON: " + jsonData);

        using (UnityWebRequest www = new UnityWebRequest("http://localhost/testdating/mkmkm.php", "POST"))
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
        public int user_id;
        public int friend_id;
    }
}
