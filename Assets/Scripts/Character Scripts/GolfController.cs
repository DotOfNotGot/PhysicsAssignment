using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GolfController : MonoBehaviour
{
    private Rigidbody _ballRb;
    private GolfUIHandler _uiHandler;
    
    [SerializeField] private float maxForce = 35.0f;
    [SerializeField] private float maxLobForce = 20.0f;

    private float _currentForce = 0.0f;
    
    [SerializeField, Range(0f, 90f)] private float lobAngle = 45.0f;
    
    [SerializeField] private float rotationSpeed = 1.0f;

    private Vector3 _direction;

    private bool _shouldLob = false;

    private bool _shootInput = false;
    
    private bool _isShooting = false;

    private Vector3 _lastStillPosition;

    private Coroutine _forceCoroutine;

    private void Awake()
    {
        _ballRb = GetComponent<Rigidbody>();
        _uiHandler = GetComponent<GolfUIHandler>();
    }

    private void FixedUpdate()
    {
        if (_isShooting && _shootInput)
        {
            // TODO: Launch the ball.
            StopCoroutine(_forceCoroutine);
            _isShooting = false;
            ShootBall();
        }
        else if (_shootInput)
        {
            // TODO: Coroutine for interpolating force value.
            _isShooting = true;
            _forceCoroutine = StartCoroutine(ForceCoroutine());
        }

        _shootInput = false;
    }

    private void ShootBall()
    {
        _direction = transform.forward;
        _ballRb.velocity = _direction * _currentForce;
    }

    private IEnumerator ForceCoroutine()
    {
        bool isAdditive = true;
        float t = 0.001f;
        float currentMaxForce = _shouldLob ? maxLobForce : maxForce;

        while (t > 0)
        {
            t += isAdditive ? Time.deltaTime : -Time.deltaTime;
            
            if (t >= 1.0f)
            {
                isAdditive = false;
            }
            _currentForce = Mathf.Lerp(0.0f, currentMaxForce, t);
            _uiHandler.SetFillAmount(t, currentMaxForce);
            yield return new WaitForFixedUpdate();
        }

        _currentForce = 0.5f;
        ShootBall();
    }

    public void SetShootInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        _shootInput = true;
    }
    
    
}
