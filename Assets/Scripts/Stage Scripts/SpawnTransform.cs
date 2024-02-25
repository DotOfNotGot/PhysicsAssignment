using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTransform : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
