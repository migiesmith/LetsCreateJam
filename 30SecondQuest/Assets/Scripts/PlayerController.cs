using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private BoardController _board;
    public int boardX = 0, boardY = 0;

    private bool canMove = true;
    private float _moveSpeed = 0.25f;


    /* Health */
    public const int MAX_HP = 4;
    private int _hp = MAX_HP;

    /* Shield */
    private const int MAX_SHIELDS = 3;
    private int _numShields = MAX_SHIELDS;


    /* Bombs */
    public const int MAX_BOMBS = 3;
    private int _numBombs = 0;

    /* Swords */
    public const int MAX_SWORDS = 4;
    private int _numSwords = 0;


    /* Quest */
    public Quest currentQuest;
    public bool onQuest = false;

    private int _score = 0;

    [SerializeField] Animator _animator;

    [SerializeField] private ResourceBar _hpBar;
    [SerializeField] private ResourceBar _shieldBar;
    [SerializeField] private ResourceBar _swordBar;
    [SerializeField] private ResourceBar _bombBar;
    [SerializeField] private Text _questText;
    [SerializeField] private Text _questDuration;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _topScoreText;

    [Header("Audio")]

    [SerializeField]
    private AudioSource _questCompleteAudio;

    [SerializeField] private AudioSource _questReceivedAudio;

    [SerializeField] private AudioSource[] _lootTileAudio;


    void Start()
    {
        _board = FindObjectOfType<BoardController>();
        if (_animator != null)
            _animator.Play("Idle");

        if (_hpBar != null)
            _hpBar.setValue(_hp);
        if (_shieldBar != null)
            _shieldBar.setValue(_numShields);
        if (_swordBar != null)
            _swordBar.setValue(_numSwords);
        if (_bombBar != null)
            _bombBar.setValue(_numBombs);
        if (_scoreText != null)
            _scoreText.text = "Score: 0";
        if (_topScoreText != null)
            _topScoreText.text = "Top Score: " + PlayerPrefs.GetInt("TopScore", 0);
    }

    public void moveTo(Vector3 newPos)
    {
        StopAllCoroutines();
        StartCoroutine(lerpMoveTo(newPos));
    }

    private IEnumerator lerpMoveTo(Vector3 newPos)
    {
        while (Vector3.Distance(transform.position, newPos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, newPos, _moveSpeed);
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.Normalize(transform.position - newPos), Vector3.up));
            yield return null;
        }
        transform.position = newPos;
    }

    public void shift(Direction dir)
    {
        StopAllCoroutines();
        if (_animator != null)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, ((int)dir + 2) * 90.0f, 0.0f));
            _animator.Play("Shift");
        }
    }

    public void useTile(Tile tile)
    {
        if (tile != null)
        {
            // Use and destroy tile
            if (currentQuest != null)
                currentQuest.processTile(tile);

            if (tile is LootTile)
            {
                if (_lootTileAudio.Length > 0)
                {
                    int idx = Random.Range(0, _lootTileAudio.Length);
                    if (_lootTileAudio[idx] != null)
                        _lootTileAudio[idx].Play();
                }
            }

            tile.use(this);
            if (tile != null)
                Destroy(tile.gameObject);
        }
    }

    public void setQuest(Quest quest)
    {
        onQuest = (quest != null);
        this.currentQuest = quest;
        this.currentQuest.questResult += onQuestComplete;
        if (_questReceivedAudio != null)
            _questReceivedAudio.Play();
        _board.questStarted();
    }

    private void onQuestComplete(Quest.QuestResults result)
    {
        switch (result)
        {
            case Quest.QuestResults.FAIL:
                {
                    _board.newGame();
                    break;
                }
            case Quest.QuestResults.COMPELTE:
                {
                    currentQuest = null;
                    onQuest = false;
                    _score++;
                    if (_scoreText != null)
                        _scoreText.text = "Score: " + _score;
                    if (_questCompleteAudio != null)
                        _questCompleteAudio.Play();
                    break;
                }
        }
        if (_questDuration != null)
            _questDuration.text = "";
        if (_questText != null)
            _questText.text = "";
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

        if (currentQuest != null)
        {
            currentQuest.update();

            // Check incase the quest didn't trigger a completion callback
            if (currentQuest != null)
            {
                if (_questDuration != null)
                    _questDuration.text = currentQuest.getQuestDuration().ToString();
                if (_questText != null)
                    _questText.text = currentQuest.getQuestText();
            }
        }
    }


    public void heal(int amount)
    {
        _hp = Mathf.Min(_hp + amount, MAX_HP);
        if (_hpBar != null)
            _hpBar.setValue(_hp);
    }

    public void takeDamage(int amount, DamageType type)
    {
        if (type == DamageType.NORMAL && _numShields > 0)
        {
            _numShields = Mathf.Max(_numShields - amount, 0);
            if (_shieldBar != null)
                _shieldBar.setValue(_numShields);
        }
        else
        {
            _hp = Mathf.Max(_hp - amount, 0);
            if (_hpBar != null)
                _hpBar.setValue(_hp);
        }
    }

    public int getHP()
    {
        return _hp;
    }

    public void gainShield(int amount)
    {
        _numShields = Mathf.Min(_numShields + amount, MAX_SHIELDS);
        if (_shieldBar != null)
            _shieldBar.setValue(_numShields);
    }

    public int getNumShield()
    {
        return _numShields;
    }

    public void gainSwords(int amount)
    {
        _numSwords = Mathf.Min(_numSwords + amount, MAX_SWORDS);
        if (_swordBar != null)
            _swordBar.setValue(_numSwords);
    }

    public void useSwords(int amount)
    {
        _numSwords = Mathf.Max(_numSwords - amount, 0);
        if (_swordBar != null)
            _swordBar.setValue(_numSwords);
    }

    public int getNumSwords()
    {
        return _numSwords;
    }

    public void gainBomb(int amount)
    {
        _numBombs = Mathf.Min(_numBombs + amount, MAX_BOMBS);
        if (_bombBar != null)
            _bombBar.setValue(_numBombs);
    }

    public void useBombs(int amount)
    {
        _numBombs = Mathf.Max(_numBombs - amount, 0);
        if (_bombBar != null)
            _bombBar.setValue(_numBombs);
    }

    public int getNumBombs()
    {
        return _numBombs;
    }


    public int getScore()
    {
        return _score;
    }

}

public enum DamageType
{
    NORMAL, ARMOUR_IGNORE
}