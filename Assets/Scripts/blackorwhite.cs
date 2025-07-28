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

public class blackorwhite : NetworkBehaviour
{
    public Player ng;
    public NetworkVariable<int> init = new NetworkVariable<int>();

    private void Start()
    {
        init.Value = 99;
    }
    [ServerRpc(RequireOwnership = false)]
    public void flipcoinServerRpc(int temp)
    {
        Debug.Log(init.Value);
        int face = 0;
        Debug.Log("face: " + face);
        while(init.Value == temp)
        {
            Debug.Log(init.Value + " == " + temp);
            for (int i = 0; i < 3; i++)
            {
                face = UnityEngine.Random.Range(0, 2);
                Debug.Log(i + " " + temp + " " + face);
            }
            init.Value = face;
            Debug.Log("init = face = " +  init.Value);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void changesceneServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    public void PlayerAdd(string url)
    {
        int Temp = init.Value;
        flipcoinServerRpc(Temp);
        StartCoroutine(DelayedInit(Temp, url));
        
    }

    private IEnumerator DelayedInit(int Temp, string url)
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("init " + init.Value + " Temp: " + Temp);
        if (init.Value == 1)
        {
            ng.pColor = "White";
        }
        else { ng.pColor = "Black"; }
        Debug.Log(ng.data.username + " " + ng.pColor);
        if (Temp != 99)
        {
            changesceneServerRpc();
        }
    }
}
