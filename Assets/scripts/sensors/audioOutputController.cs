using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioOutputController : MonoBehaviour
{
    // audio output
    public AudioSource speaker;
    // audio clip to be played
    public AudioClip sound;
    // collider for audio detection
    public SphereCollider detectionRange;

    // always set to zero
    private float minimumListeningDistance = 0.0f;
    // how far away the sound can be heard and detected
    public float maximumListeningDistance;

    // how long it will take take the collider to turn off after the sound plays
    public float clipFade;
    
    private void Start()
    {
        // bind values 
        speaker.minDistance = minimumListeningDistance;
        speaker.maxDistance = maximumListeningDistance;

        detectionRange.radius = maximumListeningDistance;
        speaker.clip = sound;
    }

    public void triggerSound()
    {
        // call and execute simmulateSound()
        StartCoroutine(simulateSound());
    }

    private IEnumerator simulateSound()
    {
        // enable the detection range and play the clip
        detectionRange.enabled = true;
        speaker.Play();
        
        while (speaker.isPlaying)
        {
            // wait for the clip to finish playing
        }
        
        // wait an amount of time to simmulate the traveling of sound
        yield return new WaitForSeconds(clipFade);
        
        // disable the detection range
        detectionRange.enabled = false;
        yield return null;
    }



   
}
