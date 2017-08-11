using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffTile : Tile
{

    [SerializeField] private AutoDestroy _lightingEffect;
    [SerializeField] private AudioClip _lightingAudio;

    private BoardController _board;
    private const float RANDOM_DESTROY_CHANCE = 0.15f;
    private const int MAX_DESTROY = 8;

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
                    // Ensure any other StaffTiles don't try to trigger this one
                    tiles[tileX, tileY] = null;
                    break;
                }
            }
        }

        // Keep track of number destroyed
        int destroyed = 0;

        for (int x = 0; x < tiles.GetLength(0); ++x)
        {
            for (int y = 0; y < tiles.GetLength(1); ++y)
            {
                if ((x != tileX || y != tileY) && tiles[x, y] != null && (!tiles[x, y].isTraversable || Random.Range(0.0f, 1.0f) <= RANDOM_DESTROY_CHANCE))
                {
                    if (_lightingEffect != null)
                    {
                        AutoDestroy effect = Instantiate(_lightingEffect);
                        effect.transform.position = tiles[x, y].transform.position;
                    }
                    bool tileIsTraversable = tiles[x, y].isTraversable;

                    player.useTile(tiles[x, y]);
                    // Prevents usage of bombs when triggering extra tiles
                    if (!tileIsTraversable)
                        player.gainBomb(1);

                    // Destroy immedietly if it isn't already
                    if (tiles[x, y] != null)
                        DestroyImmediate(tiles[x, y].gameObject);

                    destroyed++;
                }
                if (destroyed > MAX_DESTROY)
                    break;
            }
        }

        // If this is the tile the player is on
        if (player.boardX == tileX && player.boardY == tileY)
        {
            // Fill gaps
            _board.fillGaps();
            // Play Audio
            if (_lightingAudio != null)
            {
                // Create GameObject for audio
                GameObject go = new GameObject();
                // Add audio
                AudioSource audio = go.AddComponent<AudioSource>();
                audio.clip = _lightingAudio;
                audio.Play();
                // Add AutoDestroy Script
                AutoDestroy aD = go.AddComponent<AutoDestroy>();
                aD.timeTillDestroyed = audio.clip.length;
            }
        }
    }

}
