using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class HitSound : BaseClass {
    private AudioSource audioSource;
    public AudioClip audioClip;
    public float volume = 0.4f;
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider col)
    {
        audioSource.PlayOneShot(audioClip, volume);
    }
}
