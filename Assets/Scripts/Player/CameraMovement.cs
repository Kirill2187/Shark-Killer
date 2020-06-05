using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 _envPos = Vector3.zero;
    private Transform _myT;

    private Vector3 _pos = Vector3.zero;
    public Vector2 bounds;
    public Transform environment;
    public Transform player;
    public float speed = 2f;
    public Transform walls;

    private void Awake()
    {
        _myT = transform;
        _pos.z = -10;

        var camera = Camera.main;
        var wall = GameObject.Find("Wall");
        var wallLeft = GameObject.Find("Wall Left");
        var left = camera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        var right = camera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        wallLeft.transform.position.Set(left, 0, 0);
        wall.transform.position.Set(right, 0, 0);
    }

    private void LateUpdate()
    {
        _pos.x = _myT.position.x + speed * Time.deltaTime;
        if (player.position.y <= bounds.y && player.position.y >= bounds.x) _pos.y = player.position.y;
        transform.position = _pos;

        _envPos.x = _myT.position.x;
        environment.position = _envPos;
    }

    private void Update()
    {
        walls.position = _envPos;
    }
}