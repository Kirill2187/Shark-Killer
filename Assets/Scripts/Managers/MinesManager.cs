using System.Collections.Generic;
using UnityEngine;

public class MinesManager : MonoBehaviour
{
    private float lastSpawnTime;
    public int maxChainsCount = 7;
    public GameObject mine;

    [HideInInspector] public List<Mine> mines = new List<Mine>();

    public float minTimeBetweenMines = 5f;

    public AnimationCurve spawnChance;

    private void FixedUpdate()
    {
        if (Time.time > lastSpawnTime + minTimeBetweenMines &&
            Random.Range(0f, 1f) < spawnChance.Evaluate(Time.timeSinceLevelLoad) / 2500f)
        {
            lastSpawnTime = Time.time;
            var newMine = Instantiate(mine, transform.position, Quaternion.identity).GetComponent<Mine>();
            newMine.Init(Random.Range(5, maxChainsCount + 1));

            mines.Add(newMine);
        }
    }
}