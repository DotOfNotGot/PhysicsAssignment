using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTransform : MonoBehaviour
{
    [SerializeField]private Vector3 backUpSpawnPos;

    public Vector3 BackUpSpawnPos => backUpSpawnPos;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(backUpSpawnPos, Vector3.one);
    }
}
