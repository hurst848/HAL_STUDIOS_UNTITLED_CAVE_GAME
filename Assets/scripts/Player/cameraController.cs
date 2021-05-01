using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    public GameObject playerCamera;

    [SerializeField]
    private float walkSpeed, runSpeed, jumpForce, mouseSensitivity, playerGravity, playerHeight, crouchHeight, standHeight;

    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    // private movement varibles
    [HideInInspector] public bool canMove = true;
    private bool canJump = true;
    private bool canSprint = true;
    private bool canCrouch = true;
    public static bool isSprinting = false;
    private bool isCrouching = false;

    public audioOutputController footstepEmmision;
    public audioOutputController mouthEmmision;

    public List<AudioClip> painNoises;

    [HideInInspector] public bool won = false;

    private float halfSpeed;

    void Start()
    {
        halfSpeed = walkSpeed / 2.0f;
        // assign varibles
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        
        // start variouse sound coroutines
        StartCoroutine(footSteps());
        StartCoroutine(painNoiseController());
    }

    void Update()
    {
        if (health.currentHP <= 0)
        {
            canMove = false;
            StopAllCoroutines();
            _rigidbody.freezeRotation = false;
        }
        if (canMove)
        {
            if (canSprint)
            {
                // Check if shift is being held
                if (Input.GetKey(KeyCode.LeftShift)) { isSprinting = true; }
                else { isSprinting = false; }

            }
            // Check if crouch is available
            if (canCrouch)
            {
                // Check if control is being held
                if (Input.GetKey(KeyCode.LeftControl)) { isCrouching = true; }
                else { isCrouching = false; }
            }
            if (isCrouching)
            {
                //set height
                playerHeight = crouchHeight;
                _collider.height = playerHeight;
                walkSpeed = halfSpeed;
            }
            else
            {
                playerHeight = standHeight;
                _collider.height = playerHeight;
                walkSpeed = halfSpeed * 2.0f;
            }

            if (canJump)
            {
                // Check if control is being held
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _rigidbody.AddForce(Vector3.up * jumpForce * 2);
                }
            }

            float horizontalAxis = Input.GetAxis("Mouse X") * mouseSensitivity;
            float vertaicalAxis = Mathf.Clamp(Input.GetAxis("Mouse Y") * mouseSensitivity, -85, 85);


            float dirX = Input.GetAxis("Horizontal");
            float dirZ = Input.GetAxis("Vertical");

            transform.Rotate(0, horizontalAxis, 0);
            playerCamera.transform.Rotate(-vertaicalAxis, 0, 0);

            Vector3 ply = transform.TransformDirection(dirX, 0, dirZ);


            if (isSprinting) { _rigidbody.velocity = new Vector3(ply.x *runSpeed , _rigidbody.velocity.y, ply.z *runSpeed); }
            else { _rigidbody.velocity = new Vector3(ply.x * walkSpeed, _rigidbody.velocity.y, ply.z * walkSpeed); }

            //if (isSprinting && _rigidbody.velocity.magnitude < runSpeed) { _rigidbody.AddForce(ply.x * runSpeed, 0, ply.z * runSpeed); }
            //else if (_rigidbody.velocity.magnitude < walkSpeed) { _rigidbody.AddForce(ply.x * walkSpeed, 0, ply.z * walkSpeed); }

        }
    }



    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canJump = true;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            health.currentHP--;
            //Debug.Log("enemyHit");

        }
        if (collision.gameObject.tag == "Finish")
        {
            canMove = false;
            StopAllCoroutines();
            won = true;
            _rigidbody.velocity = Vector3.zero;
        }
    }

    void OnCollisionExit(Collision offFloor)
    {
        if (offFloor.gameObject.tag == "Ground")
        {
            canJump = false;
            //Debug.Log("off the ground");
        }
    }


  IEnumerator jumpDelay()
    {
        //Debug.Log("started");
        //playerGravity = 0;
        _rigidbody.useGravity = false;
        yield return new WaitForSeconds(0.1f);
        _rigidbody.useGravity = true;
        //playerGravity = 1;
        //Debug.Log("ended");

        yield return null;
  }
    IEnumerator footSteps()
    {
        yield return new WaitForSeconds(3.0f);
        while (health.currentHP != 0)
        {
            if (_rigidbody.velocity.magnitude  > 0.1f && canJump && !isCrouching)
            {
                if (isSprinting)
                {
                    footstepEmmision.gameObject.GetComponent<AudioSource>().pitch *= 1.5f;
                    footstepEmmision.monsterVolume *= 1.5f;
                    footstepEmmision.triggerSound();
                    yield return new WaitForSeconds(footstepEmmision.gameObject.GetComponent<AudioSource>().clip.length / 1.5f);
                    footstepEmmision.gameObject.GetComponent<AudioSource>().pitch /= 1.5f;
                    footstepEmmision.monsterVolume /= 1.5f;
                }
                else
                {
                    Debug.Log("WALK SOUNDS");
                    footstepEmmision.triggerSound();
                    yield return new WaitForSeconds(footstepEmmision.gameObject.GetComponent<AudioSource>().clip.length);
                }      
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator painNoiseController()
    {
        yield return new WaitForSeconds(3.0f);

        int previouseHP = health.currentHP;

        while (health.currentHP != 0)
        {
            if (health.currentHP != previouseHP)
            {
                int choise = Random.Range(0, 9) % 2;
                if (choise == 0) { mouthEmmision.sound = painNoises[0]; } 
                else { mouthEmmision.sound = painNoises[1]; }
                mouthEmmision.monsterVolume *= 1.5f;
                mouthEmmision.triggerSound();
                yield return new WaitForSeconds(mouthEmmision.gameObject.GetComponent<AudioSource>().clip.length);
                mouthEmmision.monsterVolume /= 1.5f;
                previouseHP = health.currentHP;
            }
         
            yield return null;
        }
        yield return null;
    }


    

}




/*// Check if sprint is available
       if (canSprint)
       {
           // Check if shift is being held
           if (Input.GetKey(KeyCode.LeftShift))
           {
               isSprinting = true;
               //Debug.Log("holding sprint");
           }
           else
           {
               isSprinting = false;
           }

       }       

       // Check if crouch is available
       if (canCrouch)
       {
           // Check if control is being held
           if (Input.GetKey(KeyCode.LeftControl))
           {
               isCrouching = true;
               //Debug.Log("holding crouch");
           }
           else
           {
               isCrouching = false;
           }

       }

       if(isCrouching)
       {
           //set height
           playerHeight = crouchHeight;
           _collider.height = playerHeight;
           //Debug.Log(playerHeight);
           //Debug.Log("standing");
       }
       else
       {
           playerHeight = standHeight;
           _collider.height = playerHeight;
           //Debug.Log(playerHeight);
           //Debug.Log("standing");
       }


       // Check if crouch is available
       if (canJump)
       {
           // Check if control is being held
           if (Input.GetKeyDown(KeyCode.Space))
           {
               //_rigidbody.AddForce(0, jumpForce , 0, ForceMode.Impulse);
               //_rigidbody.velocity += jumpForce * Vector3.up;
               //StartCoroutine(jump());
               //StartCoroutine(jumpDelay());
               //Debug.Log("jumping");
               _rigidbody.AddForce(Vector3.up * jumpForce*100);
           }

       }


       // Get mouse Input from player
       float horizontalAxis = Input.GetAxis("Mouse X") * mouseSensitivity;
       float vertaicalAxis = Mathf.Clamp(Input.GetAxis("Mouse Y") * mouseSensitivity, -85, 85);

       // Get "wasd" Input from player
       float dirX = Input.GetAxis("Horizontal");
       float dirZ = Input.GetAxis("Vertical");
       float dirY = 0;//playerGravity;

       // Turn movement into vector
       //-dirY
       Vector3 plyrDir = new Vector3(dirX, transform.position.y, dirZ);

       // Rotate the player
       transform.Rotate(0, horizontalAxis, 0);
       playerCamera.transform.Rotate(-vertaicalAxis, 0, 0);

       // Set velocity of the player
       if (isSprinting)
       {
           plyrDir *= runSpeed;
       }
       else
       {
           plyrDir *= walkSpeed;
       }

       plyrDir.y = _rigidbody.velocity.y;
       _rigidbody.velocity = plyrDir;

       if(isCrouching)
       {

       }

       // Move the player 
       if (canMove)
       {
           _rigidbody.velocity = transform.TransformDirection(plyrDir.x, 0, plyrDir.z);

       }*/