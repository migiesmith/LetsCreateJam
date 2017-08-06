using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardController : MonoBehaviour
{

    private Vector2 _boardSize = new Vector2(5, 5);
    private float _tileSize = 5.0f;
    private float _tileBorder = 0.5f;

    [SerializeField] private Tile _blankTilePrefab;
    [SerializeField] private Tile _questTilePrefab;
    [SerializeField] private Tile[] _enemyTilePrefabs;
    [SerializeField] private Tile[] _obstacleTilePrefabs;

    private const float enemySpawnRate = 0.25f;
    private const float obstacleSpawnRate = 0.1f;
    private const float lootSpawnRate = 0.55f;
    [SerializeField] private Tile[] _lootTilePrefabs;

    private Tile[,] _tiles;

    public PlayerController player;
    private bool questTileExists = false;

    [Header("Audio")]
    [SerializeField]
    private AudioSource _playerHitting;
    [SerializeField] private AudioSource _playerHit;


    void Start()
    {
        _tiles = new Tile[Mathf.FloorToInt(_boardSize.x), Mathf.FloorToInt(_boardSize.y)];

        // Init player with board
        player = FindObjectOfType<PlayerController>();
        setPlayerPos(Mathf.RoundToInt(_boardSize.x * 0.5f), Mathf.RoundToInt(_boardSize.x * 0.5f));

        for (int x = 0; x < Mathf.FloorToInt(_boardSize.x); ++x)
        {
            float xPos = getXTileOffset(x);
            for (int y = 0; y < Mathf.FloorToInt(_boardSize.y); ++y)
            {
                if (x != player.boardX || y != player.boardY)
                {
                    float yPos = getYTileOffset(y);
                    _tiles[x, y] = makeNewTile(x, y, xPos, yPos);
                }
            }
        }

        updateTileVisuals();
    }

    private Tile makeNewTile(int boardX, int boardY, float xPos, float yPos)
    {
        Tile newTile = Instantiate(pickTile(boardX, boardY));
        newTile.transform.position = new Vector3(xPos, this.transform.position.y, yPos);
        newTile.transform.parent = this.transform;
        return newTile;
    }

    private float getXTileOffset(int x)
    {
        return x * (_tileSize + _tileBorder) - ((_boardSize.x * 0.5f - 0.5f) * (_tileSize + _tileBorder));
    }

    private float getYTileOffset(int y)
    {
        return y * (_tileSize + _tileBorder) - ((_boardSize.y * 0.5f - 0.5f) * (_tileSize + _tileBorder));
    }

    private void setPlayerPos(int x, int y)
    {
        player.boardX = x;
        player.boardY = y;
        if (_tiles[x, y] != null)
            player.moveTo(_tiles[x, y].transform.position);
    }

    public void movePlayer(Direction dir)
    {
        int dx = dir == Direction.LEFT ? -1 : dir == Direction.RIGHT ? 1 : 0;
        int dy = dir == Direction.DOWN ? -1 : dir == Direction.UP ? 1 : 0;

        if (player.boardX + dx < 0 || player.boardX + dx >= _boardSize.x || player.boardY + dy < 0 || player.boardY + dy >= _boardSize.y)
        {
            player.shift(dir);
            return;
        }
        Tile movingTo = _tiles[player.boardX + dx, player.boardY + dy];
        if (movingTo is EnemyTile)
        {
            EnemyTile enemyTile = (EnemyTile)movingTo;
            if (player.getNumSwords() <= 0)
            {
                // Do damage to palyer
                player.takeDamage(1, enemyTile.damageType);
                // Play Audio
                if (_playerHit != null)
                    _playerHit.Play();
                // Reload level if the player dies
                if (player.getHP() <= 0)
                    SceneManager.LoadScene(0);
            }
            else
            {
                player.useSwords(1);
            }
            // Do damage to enemy
            enemyTile.attack();
            // Play Audio
            if (_playerHitting != null)
                _playerHitting.Play();

            if (enemyTile.getHP() <= 0)
            {
                setPlayerPos(player.boardX + dx, player.boardY + dy);
                updateBoard(dir);
            }
            else { player.shift(dir); }

        }
        else if (movingTo.isTraversable || player.getNumBombs() > 0)
        {
            setPlayerPos(player.boardX + dx, player.boardY + dy);
            updateBoard(dir);
        }
        else
        {
            player.shift(dir);
        }

        if (_tiles[player.boardX, player.boardY] != null)
        {
            player.useTile(_tiles[player.boardX, player.boardY]);
            if (_tiles[player.boardX, player.boardY] is QuestTile)
                questTileExists = false;
        }
    }

    private void updateBoard(Direction dir)
    {
        switch (dir)
        {
            case Direction.UP:
                {
                    // Shift Column up and create a new tile
                    int x = player.boardX;
                    float xPos = getXTileOffset(x);
                    for (int y = player.boardY - 1; y > 0; --y)
                    {
                        _tiles[x, y] = _tiles[x, y - 1];
                        _tiles[x, y].moveTo(new Vector3(xPos, this.transform.position.y, getYTileOffset(y)));
                    }
                    _tiles[x, 0] = makeNewTile(x, 0, xPos, getYTileOffset(0));
                    break;
                }
            case Direction.DOWN:
                {
                    // Shift Column down and create a new tile
                    int x = player.boardX;
                    float xPos = getXTileOffset(x);
                    for (int y = player.boardY + 1; y < _boardSize.y - 1; ++y)
                    {
                        _tiles[x, y] = _tiles[x, y + 1];
                        _tiles[x, y].moveTo(new Vector3(xPos, this.transform.position.y, getYTileOffset(y)));
                    }
                    int newPos = Mathf.RoundToInt(_boardSize.y - 1);
                    _tiles[x, newPos] = makeNewTile(x, newPos, xPos, getYTileOffset(newPos));
                    break;
                }
            case Direction.LEFT:
                {
                    // Shift Column up and create a new tile
                    int y = player.boardY;
                    float yPos = getXTileOffset(y);
                    for (int x = player.boardX + 1; x < _boardSize.x - 1; ++x)
                    {
                        _tiles[x, y] = _tiles[x + 1, y];
                        _tiles[x, y].moveTo(new Vector3(getXTileOffset(x), this.transform.position.y, yPos));
                    }
                    int newPos = Mathf.RoundToInt(_boardSize.x - 1);
                    _tiles[newPos, y] = makeNewTile(newPos, y, getXTileOffset(newPos), yPos);
                    break;
                }
            case Direction.RIGHT:
                {
                    // Shift Column up and create a new tile
                    int y = player.boardY;
                    float yPos = getXTileOffset(y);
                    for (int x = player.boardX - 1; x > 0; --x)
                    {
                        _tiles[x, y] = _tiles[x - 1, y];
                        _tiles[x, y].moveTo(new Vector3(getXTileOffset(x), this.transform.position.y, yPos));
                    }
                    _tiles[0, y] = makeNewTile(0, y, getXTileOffset(0), yPos);

                    break;
                }
        }
        updateTileVisuals();
    }

    private void updateTileVisuals()
    {
        for (int x = 0; x < _tiles.GetLength(0); ++x)
        {
            for (int y = 0; y < _tiles.GetLength(1); ++y)
            {
                // Skip the player tile
                if (x == player.boardX && y == player.boardY)
                    continue;

                bool[] dirs = new bool[4];
                dirs[(int)Direction.UP] = (y + 1 < _boardSize.y && ((x == player.boardX && y + 1 == player.boardY) || _tiles[x, y + 1].isTraversable));
                dirs[(int)Direction.DOWN] = (y - 1 >= 0 && ((x == player.boardX && y - 1 == player.boardY) || _tiles[x, y - 1].isTraversable));
                dirs[(int)Direction.LEFT] = (x - 1 >= 0 && ((x - 1 == player.boardX && y == player.boardY) || _tiles[x - 1, y].isTraversable));
                dirs[(int)Direction.RIGHT] = (x + 1 < _boardSize.x && ((x + 1 == player.boardX && y == player.boardY) || _tiles[x + 1, y].isTraversable));
                _tiles[x, y].updateTile(dirs);
            }
        }
    }

    void Update()
    {

    }




    /* --- Tile Generation --- */

    private Tile pickTile(int x, int y)
    {
        // Check for quests
        if (!player.onQuest && !questTileExists)
        {
            questTileExists = true;
            return _questTilePrefab;
        }

        // Check for overlap with player
        if (x != player.boardX || y != player.boardY)
        {
            float rnd = Random.Range(0.0f, 1.0f);
            if (rnd <= obstacleSpawnRate)
            {
                // Return an obstacle tile
                Tile obstacleTile = pickObstacleTile();
                obstacleTile.transform.rotation = Quaternion.Euler(new Vector3(0.0f, Random.Range(0, 4) * 90.0f, 0.0f));
                return obstacleTile;
            }
            else if (rnd <= enemySpawnRate)
            {
                // Return an enemy tile
                return pickEnemyTile();
            }
            else if (rnd <= enemySpawnRate + lootSpawnRate)
            {
                // Return a loot tile
                return pickLootTile();
            }
        }

        // Return a blank tile by default
        return _blankTilePrefab;
    }

    private Tile pickObstacleTile()
    {
        if (_obstacleTilePrefabs.Length == 0)
            return _blankTilePrefab;
        int idx = Random.Range(0, _obstacleTilePrefabs.Length);
        return _obstacleTilePrefabs[idx];
    }

    private Tile pickEnemyTile()
    {
        if (_enemyTilePrefabs.Length == 0)
            return _blankTilePrefab;
        int idx = Random.Range(0, _enemyTilePrefabs.Length);
        return _enemyTilePrefabs[idx];
    }

    private Tile pickLootTile()
    {
        if (_lootTilePrefabs.Length == 0)
            return _blankTilePrefab;
        int idx = Random.Range(0, _lootTilePrefabs.Length);
        return _lootTilePrefabs[idx];
    }

    public static Quest generateQuest()
    {
        if (new System.Random().Next(0, 100) < 50)
            return new CollectQuest();
        else
            return new KillQuest();
    }

}



public enum Direction
{
    RIGHT = 0, DOWN = 1, LEFT = 2, UP = 3
}