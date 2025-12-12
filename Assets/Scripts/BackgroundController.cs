using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject[] backgroundParts;
    [SerializeField] private float partWidth;
    private GameController gameController;
    [SerializeField] private float offset = 0f;

    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Update()
    {
        if (gameController.GameStarted && !gameController.GameOver)
        {
            var cameraPos = Camera.main.transform.position;
            offset += speed * Time.deltaTime;

            if (offset >= partWidth)
                offset = 0;

            transform.position = new Vector2(cameraPos.x - offset, transform.position.y);
        }
    }
}
