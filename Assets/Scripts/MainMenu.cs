using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject RoomPanel;
    public GameObject ChooseModePanel;
    public GameObject LoginPanel;
    public GameObject RegisterPanel;
    public GameObject FriendPanel;
    public GameObject ProfilePanel;
    public GameObject SettingPanel;
    public Slider volumeSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            volumeSlider.value = AudioManager.Instance.GetBGMVolume();
            volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);

            sfxSlider.value = AudioManager.Instance.GetSFXVolume();
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
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
    }
}
