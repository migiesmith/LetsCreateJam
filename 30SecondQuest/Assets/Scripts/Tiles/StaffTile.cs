using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffTile : Tile
{

    [SerializeField] private AutoDestroy _lightingEffect;

    private BoardController _board;
    private const float RANDOM_DESTROY_CHANCE = 0.15f;
    private const int MAX_DESTROY = 8;


    protected override void Start()
    {
        base.Start();
        _board = FindObjectOfType<BoardController>();
    }

    public override void use(PlayerController player)
    {
        base.use(player);

        Tile[,] tiles = _board.getTiles();

        int destroyed = 0;

        for (int x = 0; x < tiles.GetLength(0); ++x)
        {
            for (int y = 0; y < tiles.GetLength(1); ++y)
            {
                if (tiles[x, y] != this && tiles[x,y] != null && (!tiles[x, y].isTraversable || Random.Range(0.0f, 1.0f) <= RANDOM_DESTROY_CHANCE))
                {
                    if (_lightingEffect != null)
                    {
                        AutoDestroy effect = Instantiate(_lightingEffect);
                        effect.transform.position = tiles[x, y].transform.position;
                    }
					player.useTile(tiles[x,y]);
					// Prevents usage of bombs when triggering extra tiles
					if(!tiles[x,y].isTraversable)
						player.gainBomb(1);

                    DestroyImmediate(tiles[x, y].gameObject);
                    destroyed++;
                }
                if (destroyed > MAX_DESTROY)
                    break;
            }
        }
		
        _board.fillGaps();
    }

}
