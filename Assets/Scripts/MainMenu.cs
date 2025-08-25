using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject LogoutConfirmPanel;
    public GameObject LoginConfirmPanel;
    public GameObject RegisConfirmPanel;
    public GameObject RoomPanel;
    public GameObject ChooseModePanel;
    public GameObject LoginPanel;
    public GameObject RegisterPanel;
    public GameObject FriendPanel;
    public GameObject ProfilePanel;
    public GameObject SettingPanel;
    public GameObject PausePanel;
    public GameObject ChatPanel;
    public Slider volumeSlider;
    public Slider sfxSlider;
    public Player player;
    public TMP_Text namebox;
    public TMP_Text ipbox;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("hoverlayer") != null)
        {
            ipbox = GameObject.FindGameObjectWithTag("hoverlayer").GetComponent<TMP_Text>();
        }
        if (GameObject.FindGameObjectWithTag("NameBox"))
        {
            namebox = GameObject.FindGameObjectWithTag("NameBox").GetComponent<TMP_Text>();
        }
        if (AudioManager.Instance != null)
        {
            volumeSlider.value = AudioManager.Instance.GetBGMVolume();
            volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);

            sfxSlider.value = AudioManager.Instance.GetSFXVolume();
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        }
    }
    void Update()
    {
        if (ipbox != null)
        {
            ipbox.text = player.ip;
        }
        if (namebox != null)
        {
            if (player.data.username != null)
            {
                if(namebox != null)
                {
                    namebox.text = player.data.username;
                }
            }
        }
    }
    public void Play()
    {
        SceneManager.LoadSceneAsync("Game");
    }
    public void Back()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
    public void Quit()
    {
        Debug.Log("Quit Game triggered");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void Setting()
    {
        SettingPanel.SetActive(true);
    }
    public void OpenProfile()
    {
        ProfilePanel.SetActive(true);
    }
    public void OpenFriendList()
    {
        FriendPanel.SetActive(true);
    }
    public void OpenRegister()
    {
        ProfilePanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }
    public void OpenLogin()
    {
        ProfilePanel.SetActive(false);
        LoginPanel.SetActive(true);
    }
    public void OpenChooseMode()
    {
        ChooseModePanel.SetActive(true);
    }
    public void OpenRoom()
    {
        RoomPanel.SetActive(true);
        ChooseModePanel.SetActive(false);
    }
    public void CloseSettings()
    {
        SettingPanel.SetActive(false);
        ProfilePanel.SetActive(false);
        FriendPanel.SetActive(false);
        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(false);
        ChooseModePanel.SetActive(false);
        RoomPanel.SetActive(false);
        ChatPanel.SetActive(false);
        RegisConfirmPanel.SetActive(false);
        LoginConfirmPanel.SetActive(false);
        LogoutConfirmPanel.SetActive(false);
    }
    public void Restart()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "GameOffline")
        {
            SceneManager.LoadSceneAsync("GameOffline");
        }
        else if (sceneName == "GameAI")
        {
            SceneManager.LoadSceneAsync("GameAI");
        }
    }
    public void PauseGame()
    {
        PausePanel.SetActive(true);
    }
    public void ResumeGame()
    {
        PausePanel.SetActive(false);
    }
    public void OpenChat()
    {
        ChatPanel.SetActive(true);
    }
    public void RegisConfirm()
    { 
        CloseSettings();
        RegisConfirmPanel.SetActive(true);
    }
    public void LoginConfirm()
    {
        CloseSettings();
        LoginConfirmPanel.SetActive(true);
    }
    public void LogoutConfirm()
    { 
        CloseSettings();
        LogoutConfirmPanel.SetActive(true);
    }
}