using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Bumper : MonoBehaviour
{

    [SerializeField] private float bumpForce = 10.0f;
    
    private Vector3 _debugBumperPos;
    private Vector3 _debugBumperNormal;
    private Vector3 _debugBumpDir;
    private Vector3 _playerPositionAtBump;
    private Vector3 _playerBumpDir;
    
    private bool _hasBumped = false;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            Rigidbody ballRb = other.rigidbody;

            _playerBumpDir = ballRb.velocity.normalized;
            
            Vector3 contactNormal = -other.GetContact(0).normal;
            _debugBumperPos = other.GetContact(0).point;
            _debugBumperNormal = contactNormal.normalized;
            _playerPositionAtBump = other.transform.position;

            Vector3 newVel = new Vector3(_playerBumpDir.x * bumpForce, 0,
                _playerBumpDir.z * bumpForce);
            _debugBumpDir = newVel.normalized;
            
            _hasBumped = true;
            ballRb.velocity = newVel;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!_hasBumped) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_debugBumperPos, 0.1f);
            
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_playerPositionAtBump, 0.25f);
            
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_debugBumperPos, _debugBumperPos + _debugBumperNormal);
            
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_playerPositionAtBump, _playerPositionAtBump + _debugBumpDir);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(_playerPositionAtBump, _playerPositionAtBump + _playerBumpDir);
    }
}
