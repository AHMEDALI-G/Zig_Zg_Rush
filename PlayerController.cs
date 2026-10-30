using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Forward Movement")]
    public float startSpeed = 8f;
    public float speedIncreasePerSecond = 0.15f;
    public float maxSpeed = 20f;

    [Header("Lanes")]
    public float laneDistance = 3f;
    public float laneChangeSpeed = 15f;

    [Header("Jump")]
    public float baseJumpForce = 8f;
    public float maxJumpForce = 14f;
    public float jumpChargeRate = 12f;
    public float gravity = -25f;

    [Header("Tilt")]
    public float tiltAmount = 20f;
    public float tiltSmooth = 8f;

    [Header("Audio")]
    public AudioClip swipeSound;
    public AudioClip jumpSound;
    public AudioClip crashSound;
    public AudioClip coinSound;
    private AudioSource audioSource;

    private CharacterController controller;
    private int currentLane = 1;
    private float verticalVelocity = 0f;
    private float currentSpeed;
    private float jumpHoldTime = 0f;
    private bool isJumping = false;
    private float targetTilt = 0f;

    private PathGenerator pathGenerator;
    private float lastPathX = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = startSpeed;
        pathGenerator = FindObjectOfType<PathGenerator>();
        lastPathX = 0f;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        currentSpeed = Mathf.Min(currentSpeed + speedIncreasePerSecond * Time.deltaTime, maxSpeed);

        HandleInput();
        Move();
        Tilt();
    }

    void HandleInput()
    {
        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            currentLane = Mathf.Max(0, currentLane - 1);
            targetTilt = tiltAmount;

            if (audioSource != null && swipeSound != null)
                audioSource.PlayOneShot(swipeSound, 0.5f);
        }

        // Move Right
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            currentLane = Mathf.Min(2, currentLane + 1);
            targetTilt = -tiltAmount;

            if (audioSource != null && swipeSound != null)
                audioSource.PlayOneShot(swipeSound, 0.5f);
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (controller.isGrounded)
            {
                isJumping = true;
                jumpHoldTime = 0f;
                verticalVelocity = baseJumpForce;

                if (audioSource != null && jumpSound != null)
                    audioSource.PlayOneShot(jumpSound, 0.7f);
            }
        }

        if (isJumping)
        {
            bool stillHolding = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
            if (stillHolding && jumpHoldTime < 0.3f)
            {
                verticalVelocity += jumpChargeRate * Time.deltaTime;
                verticalVelocity = Mathf.Min(verticalVelocity, maxJumpForce);
                jumpHoldTime += Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            isJumping = false;
        }
    }

    void Move()
    {
        float pathX = 0f;
        if (pathGenerator != null)
        {
            pathX = pathGenerator.GetPathXAtZ(transform.position.z);
        }

        float shift = pathX - lastPathX;

        if (Mathf.Abs(shift) >= (laneDistance - 0.5f))
        {
            if (shift > 0)
            {
                currentLane = Mathf.Max(0, currentLane - 1);
            }
            else
            {
                currentLane = Mathf.Min(2, currentLane + 1);
            }
        }
        lastPathX = pathX;

        float targetX = pathX + (currentLane - 1) * laneDistance;
        float xMove = (targetX - transform.position.x) * laneChangeSpeed;

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = new Vector3(xMove, verticalVelocity, currentSpeed);
        controller.Move(move * Time.deltaTime);

        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().SetFloat("Speed", currentSpeed);
        }
    }

    void Tilt()
    {
        float pathX = pathGenerator != null ? pathGenerator.GetPathXAtZ(transform.position.z) : 0f;
        float targetX = pathX + (currentLane - 1) * laneDistance;

        if (Mathf.Abs(transform.position.x - targetX) < 0.1f)
            targetTilt = 0f;

        Quaternion targetRot = Quaternion.Euler(0, 0, targetTilt);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, tiltSmooth * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacle"))
        {
            if (crashSound != null)
            {
                AudioSource.PlayClipAtPoint(crashSound, transform.position, 1.0f);
            }

            if (Camera.main.GetComponent<CameraFollow>() != null)
            {
                Camera.main.GetComponent<CameraFollow>().ShakeCamera(0.4f, 0.5f);
            }

            GameManager.Instance.GameOver();
        }
    }

   
}