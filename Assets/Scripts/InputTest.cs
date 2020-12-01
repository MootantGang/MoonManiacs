using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InputTest : NetworkBehaviour
{
    public CharacterController charController;
    public float speed = 10f;
    public Camera cam;
    public float mouseSensitivity = 500f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    public Box playerBox;

    public bool PickedBox = false;

    public Material playerMaterial;
    public Material defaultMaterial;

    public float cameraSpeed = 1;
    public float lookUpMax = 15f;
    public float lookDownMax = -15f;
    
    public float lookRightMax = 60;
    public float lookLeftMax = -60f;
    private Quaternion camRotation;

    public Animator playerAnimator;
    public float gravity = 9.8f;

    [SyncVar]
    public bool inputEnabled = false;

    private PlayerBehaviors playerBehaviors;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerAnimator = GetComponent<Animator>();
        playerBehaviors = GetComponent<PlayerBehaviors>();
        if (GameManager.instance.gameHasStarted) {
            inputEnabled = true;
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            cam.enabled = false;
            cam.GetComponent<AudioListener>().enabled = false;
            return;
        }

        if (playerBehaviors.CanInteract()) {
            if (InputManager.SprintButton())
            {
                if (!PickedBox)
                {
                    playerBox.transform.parent = transform;
                    playerBox.transform.position = transform.position + new Vector3(0.0f, 2.0f, 0.0f);
                    PickedBox = true;
                }else if (PickedBox)
                {
                    playerBox.transform.parent = null;
                    playerBox.transform.position = transform.position + transform.forward * 2;
                    PickedBox = false;
                }
            }

            MovePlayer();
            RotateCamera();
        }       
    }

    public void MovePlayer()
    {
        //Movement
        float x = InputManager.HorizontalMovement();
        float z = InputManager.VerticalMovement();
        
        playerAnimator.SetFloat("Speed", Mathf.Abs(x) + Mathf.Abs(z));
        playerAnimator.SetFloat("SpeedX", x);
        playerAnimator.SetFloat("SpeedY", z);

        Vector3 move = transform.right * x + transform.forward * z;
        move.y -= gravity*Time.deltaTime;
        charController.Move(move * speed * Time.deltaTime);
    }

    public void RotateCamera()
    {
        //Camera      
        camRotation.x -= Input.GetAxis("Mouse Y")*cameraSpeed;
        camRotation.y += Input.GetAxis("Mouse X")*cameraSpeed;

        camRotation.x = Mathf.Clamp(camRotation.x, lookDownMax, lookUpMax);

        transform.localRotation = Quaternion.Euler(0f, camRotation.y, 0f);
        cam.transform.localRotation = Quaternion.Euler(camRotation.x, cam.transform.rotation.y, camRotation.z);
        transform.rotation = new Quaternion(transform.rotation.x, cam.transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }
   
    public override void OnStartClient()
    {
        if (!isLocalPlayer)
            return;
    }       
}
