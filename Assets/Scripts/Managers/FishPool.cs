using System.Collections.Generic;
using UnityEngine;

public class FishPool : MonoBehaviour
{
    public GameObject fish;

    private readonly Queue<GameObject> fishes = new Queue<GameObject>();
    public int poolSize = 100;

    private void Awake()
    {
        for (var i = 0; i < poolSize; i++)
        {
            var newFish = Instantiate(fish, Vector3.zero, Quaternion.identity);
            newFish.GetComponent<Fish>().Init();
            newFish.SetActive(false);

            fishes.Enqueue(newFish);
        }
    }

    public Fish Get(Vector3 pos, Quaternion rot)
    {
        if (fishes.Count == 0) return NewFish(pos, rot);

        var newFish = fishes.Dequeue();
        newFish.transform.position = pos;
        newFish.transform.rotation = rot;
        return newFish.GetComponent<Fish>();
    }

    private Fish NewFish(Vector3 pos, Quaternion rot)
    {
        var newFish = Instantiate(fish, pos, rot);
        newFish.GetComponent<Fish>().Init();

        return newFish.GetComponent<Fish>();
    }

    public void ReturnFish(GameObject fish)
    {
        fishes.Enqueue(fish);
    }

    public int GetFishCount()
    {
        return poolSize - fishes.Count;
    }
}