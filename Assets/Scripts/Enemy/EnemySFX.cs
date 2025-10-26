using System;
using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip patrolSound;
    public AudioClip chaseSound;
    public AudioClip hitSound;
    public AudioClip dieSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAttack()
    {
        if (attackSound == null) return;
        audioSource.PlayOneShot(attackSound);
    }

    public void PlayHit()
    {
        if (hitSound == null) return;
        audioSource.PlayOneShot(hitSound);
    }

    public void PlayDie()
    {
        if (dieSound == null) return;
        audioSource.PlayOneShot(dieSound);
    }

    public void PlayPatrol()
    {
        if (patrolSound == null) return;
        if (audioSource.clip == patrolSound && audioSource.isPlaying) return;

        audioSource.clip = patrolSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayChase()
    {
        if (chaseSound == null) return;
        if (audioSource.clip == chaseSound && audioSource.isPlaying) return;

        audioSource.clip = chaseSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopLoop()
    {
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.clip = null;
    }
}