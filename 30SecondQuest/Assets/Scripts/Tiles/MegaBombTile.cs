using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaBombTile : Tile
{

    private BoardController _board;

    protected override void Start()
    {
        base.Start();
        _board = FindObjectOfType<BoardController>();
    }

    public override void use(PlayerController player)
    {
        base.use(player);

        Tile[,] tiles = _board.getTiles();
        int tileX = 0, tileY = 0;
        for (int x = 0; x < tiles.GetLength(0); ++x)
        {
            for (int y = 0; y < tiles.GetLength(1); ++y)
            {
                if (tiles[x, y] == this)
                {
                    tileX = x;
                    tileY = y;
                    break;
                }
            }
        }

        // Destroy left and shift right tile
        if (tileX > 0)
        {
            player.useTile(tiles[tileX - 1, tileY]);
            // Prevents usage of bombs when triggering extra tiles
            if (!tiles[tileX - 1, tileY].isTraversable)
                player.gainBomb(1);

            DestroyImmediate(tiles[tileX - 1, tileY].gameObject);
            _board.updateBoard(Direction.RIGHT);
        }
        // Destroy right and shift left tile
        if (tileX + 1 < tiles.GetLength(0))
        {
            player.useTile(tiles[tileX + 1, tileY]);
            // Prevents usage of bombs when triggering extra tiles
            if (!tiles[tileX + 1, tileY].isTraversable)
                player.gainBomb(1);

            DestroyImmediate(tiles[tileX + 1, tileY].gameObject);
            _board.updateBoard(Direction.LEFT);
        }

        // Destroy up and shift down tile
        if (tileY > 0)
        {
            player.useTile(tiles[tileX, tileY - 1]);
            // Prevents usage of bombs when triggering extra tiles
            if (!tiles[tileX, tileY - 1].isTraversable)
                player.gainBomb(1);

            DestroyImmediate(tiles[tileX, tileY - 1].gameObject);
            _board.updateBoard(Direction.UP);
        }
        // Destroy down and shift up tile
        if (tileY + 1 < tiles.GetLength(0))
        {
            player.useTile(tiles[tileX, tileY + 1]);
            // Prevents usage of bombs when triggering extra tiles
            if (!tiles[tileX, tileY + 1].isTraversable)
                player.gainBomb(1);

            DestroyImmediate(tiles[tileX, tileY + 1].gameObject);
            _board.updateBoard(Direction.DOWN);
        }

    }
}
