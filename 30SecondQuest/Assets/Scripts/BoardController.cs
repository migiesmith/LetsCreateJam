using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{

    private Vector2 _boardSize = new Vector2(5, 5);
    private float _tileSize = 5.0f;
    private float _tileBorder = 0.5f;

    [SerializeField] private Tile _blankTilePrefab;
    [SerializeField] private Tile _questTilePrefab;
    [SerializeField] private Tile[] _enemyTilePrefabs;
    private const float enemySpawnRate = 0.20f;
    private const float lootSpawnRate = 0.70f;
    [SerializeField] private Tile[] _lootTilePrefabs;

    private Tile[,] _tiles;

    public PlayerController player;
    private bool questTileExists = false;

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
        bool madeMove = false;
        switch (dir)
        {
            case Direction.UP:
                if (player.boardY + 1 < _boardSize.y)
                {
                    setPlayerPos(player.boardX, player.boardY + 1);
                    madeMove = true;
                }
                break;
            case Direction.DOWN:
                if (player.boardY - 1 >= 0)
                {
                    setPlayerPos(player.boardX, player.boardY - 1);
                    madeMove = true;
                }
                break;
            case Direction.LEFT:
                if (player.boardX - 1 >= 0)
                {
                    setPlayerPos(player.boardX - 1, player.boardY);
                    madeMove = true;
                }
                break;
            case Direction.RIGHT:
                if (player.boardX + 1 < _boardSize.x)
                {
                    setPlayerPos(player.boardX + 1, player.boardY);
                    madeMove = true;
                }
                break;
        }

        player.useTile(_tiles[player.boardX, player.boardY]);

        if (madeMove)
            updateBoard(dir);
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
            if (rnd <= enemySpawnRate)
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

}



public enum Direction
{
    UP, DOWN, LEFT, RIGHT
}