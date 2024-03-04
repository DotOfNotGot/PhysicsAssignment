using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour
{
    [SerializeField] private float upwardsModifier = 0.0f;
    [SerializeField] private float speed = 10.0f;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var playerRb = other.attachedRigidbody;

        var speedDir = transform.right + new Vector3(0, upwardsModifier, 0);
        
        playerRb.velocity = speedDir * speed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        Gizmos.DrawRay(transform.position, transform.right + new Vector3(0, upwardsModifier, 0));
    }
}
