using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player")]
public class Player : ScriptableObject
{
    public PlayerData data;
    public string pColor = "Black";
    public Button butt;
    public string status = "";
    public int init;
    public string ip;

    public void OnEnable()
    {
        ip = GetLocalIPAddress();
    }
    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "No IPv4 address found";
    }
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
