using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    private Camera _camera;
    public GameObject feather;

    public bool isDead;
    public float speed;

    private void Start()
    {
        _camera = Camera.main;
        GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0);
    }

    private void Update()
    {
        if (transform.position.x < _camera.transform.position.x - 15) Destroy(gameObject);
    }

    public void Destroy()
    {
        if (!isDead) Instantiate(feather, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void Dead()
    {
        if (isDead) return;

        isDead = true;
        GetComponent<Rigidbody2D>().gravityScale = 0.2f;
        GetComponent<Rigidbody2D>().drag = 1f;
        GetComponent<Rigidbody2D>().angularDrag = 1f;
        GetComponent<Rigidbody2D>().velocity /= 3;
        GetComponent<Animator>().speed = 0;
    }
}