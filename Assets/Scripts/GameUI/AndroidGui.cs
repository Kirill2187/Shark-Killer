using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AndroidGui : MonoBehaviour
{
    private GameManager _manager;
    public GameObject button;

    [HideInInspector] public bool isSpeedUpButtonPressed;

    public GameObject pausePanel;

    public TextMeshProUGUI shiftTip;
    public GameObject tip;

    private void Awake()
    {
        _manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (!Application.isMobilePlatform)
            button.SetActive(false);

        if (Application.isMobilePlatform)
            Destroy(shiftTip.gameObject);
        else
            Destroy(shiftTip.gameObject, 1.9f);
    }

    public void Press()
    {
        isSpeedUpButtonPressed = true;
    }

    public void Release()
    {
        isSpeedUpButtonPressed = false;
    }

    public void PauseButtonClicked()
    {
        AudioManager.instance.Play("Click");
        if (_manager.isGameFinished) return;
        if (_manager.isGamePaused)
        {
            pausePanel.SetActive(false);
            _manager.ContinueGame();
        }
        else
        {
            pausePanel.SetActive(true);
            _manager.PauseGame();
            tip.GetComponent<TextMeshProUGUI>().text = "Tip: \n" + Tips.GetRandomTip();
        }
    }

    public void MenuButtonClicked()
    {
        AudioManager.instance.Play("Click");
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        AudioManager.instance.Play("Click");
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
}