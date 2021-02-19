using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    public GameObject playerCamera;

    [SerializeField]
    private float walkSpeed, runSpeed, jumpForce, mouseSensitivity, playerGravity, playerHeight;

    private Rigidbody _rigidbody;

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
       
    }

    void Update()
    {
        // Check if sprint is available
        if (canSprint)
        {
            // Check if shift is being held
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                isSprinting = true;
                Debug.Log("holding sprint");

                if(Input.GetKeyUp(KeyCode.LeftShift))
                {
                    isSprinting = false;
                    Debug.Log("let got of sprint");
                }
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
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isCrouching = true;
                Debug.Log("holding crouch");

                if (Input.GetKeyUp(KeyCode.LeftControl))
                {
                    Debug.Log("let got of crouch");
                    isCrouching = false;
                    
                }
            }
            else
            {
                isCrouching = false;
            }

        }
          

        // Get mouse Input from player
        float horizontalAxis = Input.GetAxis("Mouse X") * mouseSensitivity;
        float vertaicalAxis = Mathf.Clamp(Input.GetAxis("Mouse Y") * mouseSensitivity, -85, 85);

        // Get "wasd" Input from player
        float dirX = Input.GetAxis("Horizontal");
        float dirZ = Input.GetAxis("Vertical");

        // Turn movement into vector
        Vector3 plyrDir = new Vector3(dirX, -playerGravity, dirZ);

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

        // Move the player       
        _rigidbody.velocity = transform.TransformDirection(plyrDir);
        
    }
}
