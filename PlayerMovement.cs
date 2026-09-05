using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public bool _movementDisabled;
    [SerializeField] private CameraShake CS;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private float _moveSpeedFast;
    [SerializeField] private float _moveSpeedSlow;
    [SerializeField] private float _bounceSpeed = 1.0f;

    private InputSystem_Actions _input; // THIS
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Vector2 _bounceUpArea;
    private Vector2 _bounceDownArea;
    private CharacterController _controller;
    private Renderer _capsuleRenderer;
    private float _moveSpeed;
    private float _yVelocity;
    private float _xRotation;
    private bool _headBounceUp;
    private bool _characterMoves;

    private void Awake()
    {
        _input = new InputSystem_Actions(); // THIS!!!
        _controller = GetComponent<CharacterController>();
        _capsuleRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _capsuleRenderer.enabled = false;
        _bounceUpArea = new Vector3(0f, 1.2f, 0f);
        _bounceDownArea = new Vector3(0f, 0.8f, 0f);
    }

    private void OnEnable() // ALL THIS (subscription)
    {
        _input.Enable();

        _input.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _input.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _input.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _input.Player.Look.canceled += ctx => _lookInput = Vector2.zero;
    }
    private void OnDisable()
    {
        _input.Disable();
    }

    private void Update() // check for every frame if button pressed
    {
        _moveInput = _input.Player.Move.ReadValue<Vector2>();
        Look();

        if (!_movementDisabled)
        {

            if (_moveInput != Vector2.zero)
            {
                if (!_characterMoves)
                {
                    _characterMoves = true;
                    CS.CharacterMoveEnable();
                }

                Move();

                if (Keyboard.current.shiftKey.wasPressedThisFrame || Keyboard.current.shiftKey.isPressed)
                {
                    _moveSpeed = _moveSpeedFast;
                    CS._isRunning = true;
                }

                else if (Keyboard.current.shiftKey.wasReleasedThisFrame)
                {
                    _moveSpeed = _moveSpeedSlow;
                    CS._isRunning = false;
                }
            }

            else
            {
                _characterMoves = false;
                CS.CharacterMoveDisable();
            }
        }
    }


    private void Look()
    {
        float mouseX = _lookInput.x * _mouseSensitivity;
        float mouseY = _lookInput.y * _mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void Move()
    {
        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        Vector3 velocity = move * _moveSpeed;
        velocity.y = _yVelocity;

        _controller.Move(velocity * Time.deltaTime);
    }

    private void HeadBounce()
    {

        transform.position = Vector3.Lerp(
            transform.position,
            _headBounceUp? _bounceUpArea : _bounceDownArea,
            _bounceSpeed * Time.deltaTime);

        if (transform.position.y > 1.1f || transform.position.y < 0.9f)
        {
            _headBounceUp = !_headBounceUp;
        }
    }
}
