using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator _animator;

    private Rigidbody2D _body;
    private Vector2 _direction;
    private Camera _mainCam;
    private Vector2 _mousePos;
    private Quaternion _targetRotation;

    public float acceleration = 15f;
    public float accelerationSpeed = 1f;

    public ParticleSystem bubbles;
    private ParticleSystem.EmissionModule bubblesEmitter;

    [HideInInspector] public bool canControl = true;
    [HideInInspector] public float defaultAcceleration = 15f;
    [HideInInspector] public float defaultMovementSpeed = 1f;
    [HideInInspector] public float defaultTurnSpeed = 450f;

    public AndroidGui gui;
    [HideInInspector] public bool isDead;
    public Joystick joystick;

    private GameManager manager;

    public float movementSpeed = 1f;

    public float turnSpeed = 450f;

    private void Awake()
    {
        defaultMovementSpeed = movementSpeed;
        defaultAcceleration = acceleration;
        defaultTurnSpeed = turnSpeed;
        _body = GetComponent<Rigidbody2D>();
        _mainCam = Camera.main;
        _animator = GetComponent<Animator>();
        bubblesEmitter = bubbles.emission;
        bubblesEmitter.enabled = false;

        if (!Application.isMobilePlatform)
            joystick.gameObject.SetActive(false);

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (IsBoosted())
        {
            movementSpeed = accelerationSpeed;
            bubblesEmitter.enabled = true;
        }
        else
        {
            movementSpeed = defaultMovementSpeed;
            bubblesEmitter.enabled = false;
        }

        UpdateDirection();

        if (canControl && !manager.isGameFinished)
            _animator.speed = movementSpeed / 8f;
        else
            _animator.speed = 0;
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        if (canControl)
            AccelerateTo(transform.right * movementSpeed, acceleration);
        var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        _body.MoveRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle),
            turnSpeed * Time.fixedDeltaTime));
    }

    private void AccelerateTo(Vector2 targetVelocity, float maxAccel)
    {
        var deltaV = targetVelocity - _body.velocity;
        var accel = deltaV / Time.deltaTime;

        if (accel.sqrMagnitude > maxAccel * maxAccel)
            accel = accel.normalized * maxAccel;

        _body.AddForce(accel, ForceMode2D.Force);
    }

    public bool IsBoosted()
    {
        if (Application.isMobilePlatform) return gui.isSpeedUpButtonPressed && canControl;
        return Input.GetKey(KeyCode.LeftShift) && canControl;
    }

    public void UpdateDirection()
    {
        if (!Application.isMobilePlatform)
        {
            _mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            _direction = _mousePos - _body.position;
        }
        else
        {
            _direction = joystick.Direction.normalized;
        }
    }
}