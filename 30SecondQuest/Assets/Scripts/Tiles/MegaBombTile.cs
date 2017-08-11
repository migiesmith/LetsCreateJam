using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaBombTile : Tile
{

    [SerializeField] private AudioClip _megaBombAudio;
    private BoardController _board;

    protected override void Start()
    {
        base.Start();
        _board = getBoard();
    }

    protected BoardController getBoard()
    {
        return FindObjectOfType<BoardController>();
    }

    public override void use(PlayerController player)
    {
        base.use(player);

        Tile[,] tiles = (_board != null) ? _board.getTiles() : (_board = getBoard()).getTiles();
        int tileX = 0, tileY = 0;
        for (int x = 0; x < tiles.GetLength(0); ++x)
        {
            for (int y = 0; y < tiles.GetLength(1); ++y)
            {
                if (tiles[x, y] == this)
                {
                    tileX = x;
                    tileY = y;
                    // Ensure any other MegaBombTiles don't try to trigger this one
                    tiles[tileX, tileY] = null;
                    break;
                }
            }
        }

        // Destroy left and shift right tile
        if (tileX > 0 && tiles[tileX - 1, tileY] != null)
        {
            bool tileIsTraversable = tiles[tileX - 1, tileY].isTraversable;
            player.useTile(tiles[tileX - 1, tileY]);
            // Prevents usage of bombs when triggering extra tiles
            if (!tileIsTraversable)
                player.gainBomb(1);

            if (tiles[tileX - 1, tileY] != null)
                DestroyImmediate(tiles[tileX - 1, tileY].gameObject);

        }
        // Destroy right and shift left tile
        if (tileX + 1 < tiles.GetLength(0) && tiles[tileX + 1, tileY] != null)
        {
            bool tileIsTraversable = tiles[tileX + 1, tileY].isTraversable;

            player.useTile(tiles[tileX + 1, tileY]);
            // Prevents usage of bombs when triggering extra tiles
            if (!tileIsTraversable)
                player.gainBomb(1);

            if (tiles[tileX + 1, tileY] != null)
                DestroyImmediate(tiles[tileX + 1, tileY].gameObject);
        }

        // Destroy up and shift down tile
        if (tileY > 0 && tiles[tileX, tileY - 1] != null)
        {
            bool tileIsTraversable = tiles[tileX, tileY - 1].isTraversable;

            player.useTile(tiles[tileX, tileY - 1]);
            // Prevents usage of bombs when triggering extra tiles
            if (!tileIsTraversable)
                player.gainBomb(1);

            if (tiles[tileX, tileY - 1] != null)
                DestroyImmediate(tiles[tileX, tileY - 1].gameObject);

        }
        // Destroy down and shift up tile
        if (tileY + 1 < tiles.GetLength(0) && tiles[tileX, tileY + 1] != null)
        {
            bool tileIsTraversable = tiles[tileX, tileY + 1].isTraversable;
            player.useTile(tiles[tileX, tileY + 1]);
            // Prevents usage of bombs when triggering extra tiles
            if (!tileIsTraversable)
                player.gainBomb(1);

            if (tiles[tileX, tileY + 1] != null)
                DestroyImmediate(tiles[tileX, tileY + 1].gameObject);
        }

        // If this is the tile the player is on
        if (player.boardX == tileX && player.boardY == tileY)
        {
            // Fill gaps
            _board.fillGaps();
            // Play Audio
            if (_megaBombAudio != null)
            {
                // Create GameObject for audio
                GameObject go = new GameObject();
                // Add audio
                AudioSource audio = go.AddComponent<AudioSource>();
                audio.clip = _megaBombAudio;
                audio.Play();
                // Add AutoDestroy Script
                AutoDestroy aD = go.AddComponent<AutoDestroy>();
                aD.timeTillDestroyed = audio.clip.length;
            }
        }

    }
}
