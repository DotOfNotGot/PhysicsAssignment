using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public event Action<Enemy> OnDeath;
    
    private MeshRenderer _meshRenderer;
    
    // Start is called before the first frame update
    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleDeath()
    {
        _meshRenderer.enabled = false;
        OnDeath?.Invoke(this);
    }
    
}
