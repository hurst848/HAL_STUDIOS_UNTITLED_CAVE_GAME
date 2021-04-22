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
    private bool canMove = true;
    private bool canJump = true;
    private bool canSprint = true;
    private bool canCrouch = true;
    private bool isSprinting = false;
    private bool isCrouching = false;



      


    void Start()
    {
        
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

    }

    void Update()
    {
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

    }

    private void FixedUpdate()
    {
        if (canSprint)
        {
            // Check if shift is being held
            if (Input.GetKey(KeyCode.LeftShift)){ isSprinting = true; }
            else{ isSprinting = false; }

        }
        // Check if crouch is available
        if (canCrouch)
        {
            // Check if control is being held
            if (Input.GetKey(KeyCode.LeftControl)){ isCrouching = true; }
            else{ isCrouching = false; }
        }
        if (isCrouching)
        {
            //set height
            playerHeight = crouchHeight;
            _collider.height = playerHeight;            
        }
        else
        {
            playerHeight = standHeight;
            _collider.height = playerHeight;
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


        if (isSprinting){ _rigidbody.AddForce(ply.x * runSpeed, 0, ply.z*runSpeed); }
        else { _rigidbody.AddForce(ply.x * walkSpeed, 0, ply.z * walkSpeed); }
        

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canJump = true;
            Debug.Log("on the ground");
        }

        if (collision.gameObject.tag == "Enemy")
        {
            health.currentHP--;
            //Debug.Log("enemyHit");

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
    

}
