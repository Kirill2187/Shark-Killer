using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    private FishPool _pool;
    public float chance;

    public int typesCount = 5;
    public float yMin, yMax;

    private void Start()
    {
        _pool = GameObject.Find("FishPool").GetComponent<FishPool>();

        for (var i = 0; i < 3; i++) SpawnBoids(Random.Range(8f, 20f));
    }

    private void FixedUpdate()
    {
        if (_pool.GetFishCount() < 175 && Random.Range(0, 1f) < chance) SpawnBoids(transform.position.x);
    }

    private void SpawnBoids(float x)
    {
        var y = Random.Range(yMin, yMax);
        var type = Random.Range(0, typesCount);
        for (var i = 0; i < Random.Range(4, 25); i++)
        {
            var pos = new Vector3(x + Random.Range(-2f, 2f), y + Random.Range(-2f, 2f), 0);
            var newFish = _pool.Get(pos, Quaternion.identity);
            newFish.AddToGame(type);
        }
    }
}