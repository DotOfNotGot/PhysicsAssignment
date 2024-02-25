using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public event Action<Enemy> OnDeath;
    
    private MeshRenderer _meshRenderer;

    public GolfController OwnerPlayer { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleDeath(GolfController newOwner)
    {
        _meshRenderer.enabled = false;
        OwnerPlayer = newOwner;
        OnDeath?.Invoke(this);
    }
    
}
