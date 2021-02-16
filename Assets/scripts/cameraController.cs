using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    public GameObject playerCamera;

    [SerializeField]
    private float walkSpeed, runSpeed, jumpForce, mouseSensitivity, playerGravity;

    private Rigidbody _rigidbody;

    // private movement varibles
    private bool canMove = true;
    private bool canJump = true;
    private bool isSprinting = false;
    private bool canSprint = true;


    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if shift is being held
        if (canSprint)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
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
        _rigidbody.velocity = transform.TransformDirection(plyrDir);
    }
}
