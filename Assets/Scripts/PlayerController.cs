using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float acceleration;
    [SerializeField] private AudioSource jumpAudio;

    private Rigidbody2D rb;
    private GameController gameController;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        var cameraPos = Camera.main.transform.position;

        if (transform.position.x >= cameraPos.x)
            Camera.main.transform.position = new Vector3(transform.position.x, cameraPos.y, cameraPos.z);
    }

    private void FixedUpdate()
    {
        if (gameController.GameStarted && !gameController.GameOver)
        {
            Time.timeScale += acceleration;
            rb.linearVelocityX = speed;
            anim.SetBool("IsRunning", true);
        }
    }

    public void Jump()
    {
        if (gameController.GameStarted && !gameController.GameOver && IsGrounded())
        {
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
            jumpAudio.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Spikes"))
        {
            anim.SetTrigger("Death");
            rb.bodyType = RigidbodyType2D.Static;
            gameController.EndGame();
        }
    }

    private bool IsGrounded()
    {
        var colliders = Physics2D.OverlapBoxAll
        (
            new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2),
            new Vector2(0.5f, 0.05f), 0f
        );

        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("Block"))
            {
                return true;
            }
        }

        return false;
    }
}
