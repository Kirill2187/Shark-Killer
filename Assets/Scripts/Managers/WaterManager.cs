using UnityEngine;

public class WaterManager : MonoBehaviour
{
    private Water _nextWater;

    private Water _water;
    private float nextBlockX;
    public float oneBlockWidth = 25f;
    public Transform player;
    public GameObject waterPool;

    private void Start()
    {
        _water = Instantiate(waterPool, Vector3.zero, Quaternion.identity).GetComponent<Water>();
        _water.SpawnWater(-15f, oneBlockWidth, 2f, -3f);

        _nextWater = Instantiate(waterPool, Vector3.zero, Quaternion.identity).GetComponent<Water>();
        _nextWater.SpawnWater(-15f + oneBlockWidth, oneBlockWidth, 2f, -3f);
        nextBlockX = -15f + oneBlockWidth;

        _nextWater.prevBlock = _water;
        _water.nextBlock = _nextWater;
    }

    private void Update()
    {
        if (player.position.x > nextBlockX + oneBlockWidth / 2)
        {
            Destroy(_water.transform.root.gameObject);
            _water = _nextWater;
            _water.prevBlock = null;
            _nextWater = Instantiate(waterPool, Vector3.zero, Quaternion.identity).GetComponent<Water>();
            _nextWater.SpawnWater(nextBlockX + oneBlockWidth, oneBlockWidth, 2f, -3f);

            _nextWater.prevBlock = _water;
            _water.nextBlock = _nextWater;

            nextBlockX += oneBlockWidth;
        }
    }
}