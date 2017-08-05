using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private BoardController _board;
    public int boardX = 0, boardY = 0;

    private bool canMove = true;
    private float _moveSpeed = 0.25f;

    private const float maxHp = 100.0f;
    public float hp = maxHp;

    public float damage = 10.0f;

    public bool onQuest = false;

    public Quest currentQuest;
    public int numBombs = 0;

    public const int MAX_BOMBS = 5;

    void Start()
    {
        _board = FindObjectOfType<BoardController>();
    }

    public void moveTo(Vector3 newPos)
    {
        StopAllCoroutines();
        StartCoroutine(lerpMoveTo(newPos));
    }

    private IEnumerator lerpMoveTo(Vector3 newPos)
    {
        while (Vector3.Distance(transform.position, newPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, newPos, _moveSpeed);
            yield return null;
        }
    }

    public void useTile(Tile tile)
    {
        // Use and destroy tile
        tile.use(this);
        if(tile != null)
            Destroy(tile.gameObject);
    }

    public void setQuest(Quest quest)
    {
        onQuest = (quest != null);
        this.currentQuest = quest;
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (canMove)
        {
            canMove = false;
            if (x > 0.0f)
            {
                _board.movePlayer(Direction.RIGHT);
            }
            else if (x < 0.0f)
            {
                _board.movePlayer(Direction.LEFT);

            }
            else if (y > 0.0f)
            {
                _board.movePlayer(Direction.UP);

            }
            else if (y < 0.0f)
            {
                _board.movePlayer(Direction.DOWN);

            }
            else
            {
                canMove = true;
            }
        }
        else if (x == 0.0f && y == 0.0f)
        {
            canMove = true;
        }

    }
}
