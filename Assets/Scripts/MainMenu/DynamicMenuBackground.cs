using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicMenuBackground : MonoBehaviour
{
    private List<FishImg> currentFishes;

    public Sprite[] fishes;

    public GameObject fishesPanel;
    public GameObject fishRight, fishLeft;
    public int maxFishes = 10;
    public float spawnChance = 0.01f;

    private void Awake()
    {
        currentFishes = new List<FishImg>();
    }

    private void Update()
    {
        CheckAllFishes();
        if (currentFishes.Count < maxFishes && Random.Range(0f, 1f) < spawnChance) Spawn();
        MoveAll();
    }

    private void MoveAll()
    {
        foreach (var fish in currentFishes) fish.fish.transform.Translate(fish.speed * Time.deltaTime, 0, 0);
    }

    private void CheckAllFishes()
    {
        foreach (var fish in currentFishes)
            if (fish.fish.transform.position.x < -300 || fish.fish.transform.position.x > Screen.width + 400)
            {
                Destroy(fish.fish);
                currentFishes.Remove(fish);
                return;
            }
    }

    private void Spawn()
    {
        var isForward = Random.Range(0, 2) == 0;

        var type = Random.Range(0, fishes.Length);
        var speed = Random.Range(Screen.width / 10f, Screen.width / 5f);
        var first = isForward ? fishRight : fishLeft;
        var center = new Vector2(
            first.transform.position.x + (isForward ? Random.Range(-100f, 0f) : Random.Range(0f, 100f)),
            Random.Range(0f, 600f));
        var rot = first.transform.rotation * Quaternion.Euler(0, 0, Random.Range(-30f, 30f));

        for (var i = 0; i < Random.Range(3, 15); i++)
        {
            var fish = Instantiate(first, Random.insideUnitCircle * 50 + center, rot, transform);

            var img = fish.GetComponent<Image>();
            img.sprite = fishes[type];

            currentFishes.Add(new FishImg(fish, speed));
        }
    }

    private class FishImg
    {
        public readonly GameObject fish;
        public readonly float speed;

        public FishImg(GameObject newFish, float newSpeed)
        {
            fish = newFish;
            speed = newSpeed;
        }
    }
}