using UnityEngine;

public class GravityController : MonoBehaviour
{
    private BirdMovement _bird;
    private Rigidbody2D _body;
    private Fish _fish;

    private PlayerMovement _playerMovement;
    public float airGravityScale = 0.8f;
    public float waterLevel;

    private void Awake()
    {
        if (gameObject.tag.Equals("player"))
            _playerMovement = GetComponent<PlayerMovement>();
        else if (gameObject.tag.Equals("fish"))
            _fish = GetComponent<Fish>();
        else if (gameObject.tag.Equals("bird")) _bird = GetComponent<BirdMovement>();

        _body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_bird && transform.position.y < waterLevel)
        {
            _bird.Dead();
            return;
        }

        if (_fish && _fish.isShocked)
        {
            if (transform.position.y > waterLevel)
            {
                _body.gravityScale = airGravityScale;
                _body.angularDrag = 0f;
                _body.drag = 0f;
            }
            else if (transform.position.y < waterLevel - 0.05f)
            {
                _body.gravityScale = -1f;
                _body.angularDrag = 1f;
                _body.drag = 1f;
            }

            return;
        }

        if (transform.position.y > waterLevel)
        {
            if (_playerMovement)
                _playerMovement.canControl = false;
            if (_fish)
                _fish.canMove = false;

            _body.gravityScale = airGravityScale;
        }
        else if (transform.position.y < waterLevel - 0.05f)
        {
            if (_playerMovement)
                _playerMovement.canControl = true;
            if (_fish)
                _fish.canMove = true;
            _body.gravityScale = 0;
        }
    }
}