using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GolfController : MonoBehaviour
{
    private Rigidbody _ballRb;
    private GolfUIHandler _uiHandler;
    private LineProjection _lineProjection;
    private Collider _ballCollider;
    private PlayerAudioHandler _audioHandler;
    private PlayerInput _input;

    public event Action<Vector2> CameraRotation;
    public event Action OnTurnDone;
    public event Action<int> OnWin;
    
    [SerializeField] private float forceChargeSpeed = 5.0f;
    [SerializeField] private float maxForce = 35.0f;
    [SerializeField] private float maxLobForce = 20.0f;

    private float _currentMaxForce = 0.0f;    
    private float _currentForce = 0.0f;
    
    [SerializeField, Range(0f, 90f)] private float lobAngle = 45.0f;
    
    [SerializeField] private float rotationSpeed = 1.0f;

    public int PlayerIndex { get; set; } = 0;

    private int _playerScore = 0;
    

    private float _rotationDirection;
    private Vector3 _directionAngles;

    private Vector3 Direction => transform.forward;

    private bool _shouldLob = false;

    private bool _shootInput = false;
    
    private bool _isChargingShot = false;
    private bool _hasShot = false;

    private Coroutine _forceCoroutine;

    private Vector3 _oldTransformPosition;
    private Vector3 _lastStillPosition;

    private Coroutine _turnEndCoroutine;
    
    private void Awake()
    {
        _ballRb = GetComponent<Rigidbody>();
        _uiHandler = GetComponent<GolfUIHandler>();
        _lineProjection = GetComponent<LineProjection>();
        _ballCollider = GetComponent<Collider>();
        _audioHandler = GetComponent<PlayerAudioHandler>();
        _input = GetComponent<PlayerInput>();
        
        _directionAngles = transform.forward;
        _currentMaxForce = maxForce;
    }

    private void Update()
    {
        HandleRotationInput();

        if (_hasShot && _ballRb.velocity.magnitude < 0.5f && _turnEndCoroutine == null)
        {
            _turnEndCoroutine = StartCoroutine(TurnEndTimer());
        }

        if (transform.position.y < -2.0f)
        {
            transform.position = _lastStillPosition;
            TurnEnd();
        }
    }

    private IEnumerator TurnEndTimer()
    {
        yield return new WaitForSeconds(1.0f);

        if (_ballRb.velocity.magnitude < 0.5f)
        {
            TurnEnd();
        }

        _turnEndCoroutine = null;
    }

    private void TurnEnd()
    {
        _turnEndCoroutine = null;
        _hasShot = false;
        OnTurnDone?.Invoke();
    }
    
    private void FixedUpdate()
    {
        // Checks if the player has started charging a shot and if the input to shoot has been received.
        if (_isChargingShot && _shootInput)
        {
            StopCoroutine(_forceCoroutine);
            _isChargingShot = false;
            ShootBall();
        }
        // Otherwise it starts the charging of the shot
        else if (_shootInput)
        {
            _isChargingShot = true;
            _forceCoroutine = StartCoroutine(ForceCoroutine());
        }

        _shootInput = false;
        if (_oldTransformPosition != transform.position)
        {
            _oldTransformPosition = transform.position;
            _lineProjection.LineEnabled(false);
        }
        else if(_lastStillPosition != transform.position)
        {
            _lastStillPosition = transform.position;
            _lineProjection.CalculateTrajectory(_currentMaxForce, Direction, _ballCollider, _ballRb);
        }
    }

    private IEnumerator ForceCoroutine()
    {
        bool isAdditive = true;
        float t = 0.001f;

        while (t > 0)
        {
            t += ((isAdditive ? Time.deltaTime : -Time.deltaTime) / _currentMaxForce) * forceChargeSpeed;
            
            if (t >= 1.0f)
            {
                isAdditive = false;
            }
            _currentForce = Mathf.Lerp(0.0f, _currentMaxForce, t);
            _uiHandler.SetFillAmount(t);
            yield return new WaitForFixedUpdate();
        }

        _currentForce = 0.5f;
        ShootBall();
    }
    private void ShootBall()
    {
        _hasShot = true;
        _ballRb.velocity = Direction * _currentForce;
    }

    public void SetShootInput(InputAction.CallbackContext context)
    {
        if (!context.performed || _hasShot) return;
        
        _shootInput = true;
    }

    public void SetShouldLob(InputAction.CallbackContext context)
    {
        if (!context.performed || _isChargingShot || _hasShot) return;

        _shouldLob = !_shouldLob;
        
        _directionAngles.x = _shouldLob ? -lobAngle : 0;

        transform.rotation = Quaternion.Euler(_directionAngles);
        
        _currentMaxForce = _shouldLob ? maxLobForce : maxForce;
        _lineProjection.CalculateTrajectory(_currentMaxForce, Direction, _ballCollider, _ballRb);
    }
    
    public void GetCameraRotationInput(InputAction.CallbackContext context)
    {
        // Sends the rotation value to the camera if it is this controllers turn.
        CameraRotation?.Invoke(context.ReadValue<Vector2>());
    }

    public void GetRotationInput(InputAction.CallbackContext context)
    {
        _rotationDirection = context.ReadValue<float>();
    }

    private void HandleRotationInput()
    {
        if (_rotationDirection == 0 || _isChargingShot || _hasShot) return;
        
        _directionAngles.y += _rotationDirection * rotationSpeed * Time.deltaTime;
        
        transform.rotation = Quaternion.Euler(_directionAngles);

        if (transform.position != _oldTransformPosition)
        {
            transform.position = _oldTransformPosition;
        }
        
        _lineProjection.CalculateTrajectory(_currentMaxForce, Direction, _ballCollider, _ballRb);
    }

    public void StartPlayerTurn()
    {
        SetPlayerTurn(true);
    }
    public void EndPlayerTurn()
    {
        _ballRb.isKinematic = true;
        transform.rotation = Quaternion.Euler(_directionAngles);
        _ballRb.isKinematic = false;
        _hasShot = false;
        
        SetPlayerTurn(false);
    }

    private void SetPlayerTurn(bool value)
    {
        _input.enabled = value;
        _lineProjection.LineEnabled(value);
        _uiHandler.SetUIActive(value);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WinTrigger"))
        {
            OnWin?.Invoke(PlayerIndex);
            _ballRb.detectCollisions = false;
            _ballRb.isKinematic = true;
        }
        else if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().HandleDeath(this);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        _audioHandler.PlayHitSound();
    }
}
