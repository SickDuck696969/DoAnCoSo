using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player")]
public class Player : ScriptableObject
{
    public PlayerData data;
    public string pColor = "Black";
    public Button butt;

    public IEnumerator GetUserFromServer(string email, string password)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://localhost/testdating/getuser.php?email={email}&passsword={password}");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to contact server: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            data = JsonUtility.FromJson<PlayerData>(json);
        }
    }
}
