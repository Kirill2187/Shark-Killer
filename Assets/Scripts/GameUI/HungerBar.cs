using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    private Image[] _fishes;
    public int fishesCount = 20;
    public GameObject fishImage;
    public Color off;
    public float space = 30f;
    public Color zeroHungerColor;

    private void Awake()
    {
        _fishes = new Image[fishesCount];
        _fishes[0] = fishImage.GetComponent<Image>();
        for (var i = 0; i < fishesCount - 1; i++)
        {
            var fish = Instantiate(fishImage, fishImage.transform.position, fishImage.transform.rotation, transform)
                .GetComponent<Image>();
            fish.transform.position = new Vector3(fish.transform.position.x + (i + 1) * space,
                fish.transform.position.y, fish.transform.position.z);
            _fishes[i + 1] = fish;
        }
    }

    public void SetHunger(int amount, int max)
    {
        var count = (int) Mathf.Ceil((float) amount / max * fishesCount);
        if (count > fishesCount) return;

        if (count <= 0)
        {
            foreach (var fish in _fishes) fish.color = zeroHungerColor;
        }
        else
        {
            for (var i = 0; i < count; i++) _fishes[i].color = Color.white;

            for (var i = count; i < fishesCount; i++) _fishes[i].color = off;
        }
    }
}