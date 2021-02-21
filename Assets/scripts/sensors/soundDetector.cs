using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundDetector : MonoBehaviour
{   
    // the range of listening
    public float listeningRange;
    // the multiplier for increasing the listening range when actively listening
    public float listeningMultiplier;
    // the threshold for the monster to immidetaly chase after a target
    public float chaseThreshold;
    // the threshold for the monster to investigate a target (not run)
    public float investigateThreshold;

    // the deturmined target of the listener
    public Vector3 target;
    // whether or not the monster should be persuing the origin of a sound 
    public bool chase = false;
    // whether or not the monster should be investigating the origin of a sound
    public bool investigating = false;
    // whether or not the monster is listening
    public bool listening = false;


    // current state of the audio listener:
    //      false = passive
    //      true  = active
    private bool state = false;




    private void Start()
    {
        GetComponent<SphereCollider>().radius = listeningRange;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // check to see if it has detected an object with the sound tag
        if (collision.gameObject.tag == "sound")
        {
            // if passivley listening
            if (!state)
            {
                float soundLevel = calulateSoundValue(collision);
                if (soundLevel >= chaseThreshold)
                {
                    target = collision.transform.position;
                    chase = true;
                }
                else
                {
                    switchState(true);
                }
            }
            // if activeley listening
            if (state)
            {
                float soundLevel = calulateSoundValue(collision);
                if (soundLevel >= chaseThreshold)
                {
                    target = collision.transform.position;
                    switchState(false);
                    chase = true;
                }
                else if (soundLevel >= investigateThreshold)
                {
                    target = collision.transform.position;
                    switchState(false);
                    investigating= true;
                }
          
            } 
        }
    }

    public void switchState(bool _x)
    {
        if (!_x)
        {
            GetComponent<SphereCollider>().radius = listeningRange;
            state = false;
            listening = false;
        }
        else
        {
            state = true;
            listening = true;
            GetComponent<SphereCollider>().radius *= listeningMultiplier;
        }
    }

    private float calulateSoundValue(Collision _c)
    {
        AudioSource source = _c.gameObject.GetComponent<AudioSource>();
        return source.volume / Mathf.Abs(Vector3.Distance(_c.gameObject.transform.position, transform.position));
    }
}
