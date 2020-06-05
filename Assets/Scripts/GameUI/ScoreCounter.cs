using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    private int currentScore;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateScore(int score)
    {
        currentScore = score;
        StartCoroutine(Pulse());
    }

    private IEnumerator Pulse()
    {
        for (var i = 1f; i <= 1.2f; i += 0.05f)
        {
            text.rectTransform.localScale = new Vector3(i, i, i);
            yield return new WaitForEndOfFrame();
        }

        text.rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

        text.text = currentScore.ToString();

        for (var i = 1.2f; i >= 1f; i -= 0.05f)
        {
            text.rectTransform.localScale = new Vector3(i, i, i);
            yield return new WaitForEndOfFrame();
        }

        text.rectTransform.localScale = Vector3.one;
    }
}