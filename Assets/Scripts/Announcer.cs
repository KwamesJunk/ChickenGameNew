using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Announcer : MonoBehaviour
{
    [SerializeField] AudioClip[] announcement;
    AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Announce(int clipNum)
    {
        audioSource.PlayOneShot(announcement[clipNum], 0.4f);
    }

    public void Announce()
    {
        int clipNum = Random.Range(0, announcement.Length);
        Announce(clipNum);
    }
}
