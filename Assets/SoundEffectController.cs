using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectController : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip positiveSound;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.soundEffectController = this;
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayPositiveSound(){
        audioSource.clip = positiveSound;
        audioSource.Play();
    }
}
