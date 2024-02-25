using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{

    [SerializeField] private float bumpForce = 10.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetBumpDirection(Vector3 dir, Vector3 normal)
    {
        return (dir + normal).normalized * bumpForce ;
    }

    [Header("Debug")]
    [SerializeField]private Vector3 _debugBumperPos;
    [SerializeField]private Vector3 _debugBumperNormal;
    [SerializeField]private Vector3 _debugBumpDir;
    [SerializeField]private Vector3 _positionAtBump;
    
    private bool _hasBumped = false;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player") && !_hasBumped)
        {
            Rigidbody ballRb = other.rigidbody;
            
            Vector3 contactNormal = other.GetContact(0).normal;
            _debugBumperPos = other.GetContact(0).point;
            _debugBumperNormal = contactNormal;
            _positionAtBump = other.transform.position;
            var bumpDir = GetBumpDirection(ballRb.velocity.normalized, contactNormal);
            _debugBumpDir = bumpDir.normalized;
            
            _hasBumped = true;
            ballRb.velocity = bumpDir;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (_hasBumped)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_debugBumperPos, 1);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_positionAtBump, 1);
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_debugBumperPos, _debugBumperPos + _debugBumperNormal);
            
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_positionAtBump, _positionAtBump + _debugBumpDir);
        }
    }
}
