using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject bird;
    public float chance = 0.05f;

    public Vector2 minMaxBounds;

    private void FixedUpdate()
    {
        if (Random.Range(0f, 1f) <= chance)
        {
            var pos = new Vector3(transform.position.x, Random.Range(minMaxBounds.x, minMaxBounds.y), 0);
            AudioManager.instance.Play("Seagulls");
            Instantiate(bird, pos, Quaternion.identity);
        }
    }
}