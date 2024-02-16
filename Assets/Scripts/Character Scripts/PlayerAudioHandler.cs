using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{

    private AudioSource _audioSource;

    [SerializeField] private AudioClip hitSound;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayHitSound()
    {
        if (_audioSource.isPlaying) return;
        
        _audioSource.clip = hitSound;
        _audioSource.Play();
    }
    
    
}
