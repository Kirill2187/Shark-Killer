using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private Boids _boids;
    private Camera _camera;
    private FishPool _pool;

    public GameObject blood;

    [HideInInspector] 
    public Rigidbody2D body;
    [HideInInspector] 
    public bool canMove = true;

    private int _curFrame;

    [HideInInspector] 
    public bool isDead;

    private bool _isInitialized;
    
    [HideInInspector] 
    public bool isShocked;

    [HideInInspector] 
    public bool killedByPlayer;
    public float movementSpeed = 10f;

    [HideInInspector] 
    public HashSet<Fish> neighbours = new HashSet<Fish>();
    public Sprite[] sprites;

    [HideInInspector] 
    public int type;

    private void Start()
    {
        if (!_isInitialized) Init();
    }

    public void Init()
    {
        if (_isInitialized) return;

        _isInitialized = true;

        body = GetComponent<Rigidbody2D>();
        _boids = GameObject.Find("BoidsManager").GetComponent<Boids>();
        _camera = Camera.main;
        _pool = GameObject.Find("FishPool").GetComponent<FishPool>();

        _curFrame = Random.Range(0, 100);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (other.isTrigger || !other.transform.root.gameObject.CompareTag("fish")) return;

        if (other.transform.root.gameObject.GetComponent<Fish>().type == type)
            neighbours.Add(other.transform.root.gameObject.GetComponent<Fish>());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.isTrigger || !other.transform.root.gameObject.CompareTag("fish")) return;
        if (other.transform.root.gameObject.GetComponent<Fish>().type == type)
            neighbours.Remove(other.transform.root.gameObject.GetComponent<Fish>());
    }

    private void UpdateNeighbours()
    {
        foreach (var fish in _boids.boids[type])
        {
            if (fish == this) continue;

            if ((fish.body.position - body.position).sqrMagnitude <= _boids.viewDst * _boids.viewDst)
                neighbours.Add(fish);
            else if (neighbours.Contains(fish)) neighbours.Remove(fish);
        }
    }

    private void CheckAllNeighbours()
    {
        foreach (var fish in neighbours)
            if (fish.isDead)
            {
                neighbours.Remove(fish);
                return;
            }
    }

    public void AddToGame(int type)
    {
        isDead = false;
        isShocked = false;
        killedByPlayer = false;

        gameObject.SetActive(true);
        this.type = type;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[type];

        _boids.AddFish(this);
    }

    public void ReturnToPool()
    {
        isDead = true;
        _boids.boids[type].Remove(this);
        gameObject.SetActive(false);
        neighbours.Clear();

        _pool.ReturnFish(gameObject);
    }

    private void Update()
    {
        if (isDead) return;
        ++_curFrame;
        if (_curFrame == 100)
        {
            _curFrame = 0;
            UpdateNeighbours();
        }

        //if (Random.Range(0f, 1f) < 0.01f) CheckAllNeighbours();
        if (body.position.x < _camera.transform.position.x - 12f) Destroy();
    }

    public void UpdateVelocity(Vector2 velocity)
    {
        if (!canMove || isShocked) return;
        if (isDead) return;
        if (velocity.sqrMagnitude > movementSpeed * movementSpeed) velocity = velocity.normalized * movementSpeed;

        var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 180f;
        body.MoveRotation(angle);
        //body.velocity = velocity;
        AccelerateTo(velocity, 4f);
    }

    public void Destroy()
    {
        if (isDead) return;
        if (killedByPlayer) Instantiate(blood, transform.position, Quaternion.identity);

        ReturnToPool();
    }

    private void AccelerateTo(Vector2 targetVelocity, float maxAccel)
    {
        var deltaV = targetVelocity - body.velocity;
        var accel = deltaV / Time.fixedDeltaTime;

        if (accel.sqrMagnitude > maxAccel * maxAccel)
            accel = accel.normalized * maxAccel;

        body.AddForce(accel, ForceMode2D.Force);
    }
}