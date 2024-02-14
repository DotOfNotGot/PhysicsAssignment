using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLayerHandler : MonoBehaviour
{
    
    [SerializeField]
    private int layerOnTriggerEnter;
    
    private int _layerOnTriggerExit = 0;

    // Changes layer of the other object so that it stops colliding with the ground
    // allowing it to fall down into the hole. Also changes the layerOnTriggerExit
    // integer so that it sets the object back to the correct layer after it exits.
    private void OnTriggerEnter(Collider other)
    {
        var otherGO = other.gameObject;
        
        _layerOnTriggerExit = otherGO.layer;
        otherGO.layer = layerOnTriggerEnter;
    }

    // Changes layer back to the layer the other object entered with so that it
    // can collide with the ground again.
    private void OnTriggerExit(Collider other)
    {
        other.gameObject.layer = _layerOnTriggerExit;
    }
}
