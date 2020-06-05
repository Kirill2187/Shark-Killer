using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image audioImage;
    public Sprite audioOff;

    public Sprite audioOn;
    public Slider audioSlider;
    public GameObject buttonsPanel;

    public GameObject loadingPanel;
    public Slider loadingSlider;
    public AudioMixer mixer;

    public GameObject infoMenu;

    public TextMeshProUGUI bestScore;

    private void Awake()
    {
        var volume = PlayerPrefs.GetFloat("audio", 0.5f);
        audioImage.sprite = volume == 0.0001f ? audioOn : audioOff;
        audioSlider.value = volume;

        mixer.SetFloat("volume", Mathf.Log10(volume) * 50);
    }

    private void Start()
    {
        infoMenu.SetActive(true);
        infoMenu.SetActive(false);
        loadingPanel.SetActive(true);
        loadingPanel.SetActive(false);
        
        var volume = PlayerPrefs.GetFloat("audio", 0.5f);
        mixer.SetFloat("volume", Mathf.Log10(volume) * 50);
        
        int best = PlayerPrefs.GetInt("bestScore", 0);
        bestScore.text = "Your best score\n<color=#FFFF66>" + best + "</color>";
    }

    public void Play()
    {
        AudioManager.instance.Play("Click");
        loadingPanel.SetActive(true);
        buttonsPanel.SetActive(false);
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        var operation = SceneManager.LoadSceneAsync(1);
        loadingSlider.value = 0;
        while (!operation.isDone)
        {
            loadingSlider.value = operation.progress;
            yield return null;
        }
    }

    public void InfoButtonPressed()
    {
        AudioManager.instance.Play("Click");
        ShowInfoMenu();
    }

    public void QuitButtonPressed()
    {
        AudioManager.instance.Play("Click");
        Application.Quit();
    }

    public void AudioButtonPressed()
    {
        var isAudioOn = PlayerPrefs.GetFloat("audio", 0.5f) != 0.0001f;
        isAudioOn = !isAudioOn;
        PlayerPrefs.SetFloat("audio", isAudioOn ? 0.5f : 0.0001f);

        audioImage.sprite = isAudioOn ? audioOn : audioOff;
        audioSlider.value = isAudioOn ? 0.5f : 0.0001f;
        mixer.SetFloat("volume", isAudioOn ? Mathf.Log10(0.5f) * 50f : Mathf.Log10(0.0001f) * 50f);

        AudioManager.instance.Play("Click");
    }

    public void SetVolume(float volume)
    {
        audioSlider.value = volume;
        audioImage.sprite = volume != 0.0001f ? audioOn : audioOff;
        mixer.SetFloat("volume", Mathf.Log10(volume) * 50);

        PlayerPrefs.SetFloat("audio", volume);
    }

    public void ShowInfoMenu()
    {
        buttonsPanel.SetActive(false);
        infoMenu.SetActive(true);
    }

    public void HideInfoMenu()
    {
        AudioManager.instance.Play("Click");
        
        infoMenu.SetActive(false);
        buttonsPanel.SetActive(true);
    }
}