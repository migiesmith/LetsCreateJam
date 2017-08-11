using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KillQuest : Quest
{

    public enum Collectable
    {
        DEMON = 0, SPIDER = 1, ORC = 2
    }
    private Collectable _toCollect;

    private int _numToKill = new System.Random().Next(3, 5);
    private int _numKilled = 0;

    public KillQuest()
    {
        int collectableVal = new System.Random().Next(0, 3);
        switch (collectableVal)
        {
            case (int)Collectable.DEMON:
                _toCollect = Collectable.DEMON;
                break;
            case (int)Collectable.SPIDER:
                _toCollect = Collectable.SPIDER;
                break;
            case (int)Collectable.ORC:
                _toCollect = Collectable.ORC;
                break;
        }
    }

    public override string getQuestText()
    {
        string val = "Defeat " + _numToKill + " ";
        switch (_toCollect)
        {
            case Collectable.DEMON:
                val += "demons. ";
                break;
            case Collectable.SPIDER:
                val += "spiders. ";
                break;
            case Collectable.ORC:
                val += "orcs. ";
                break;
        }
        return val + Mathf.RoundToInt(progress * 100.0f) + "%";
    }

    public override void processTile(Tile tile)
    {
        switch (_toCollect)
        {
            case Collectable.DEMON:
                if (tile is EnemyTile)
                    if (((EnemyTile)tile).enemyName == "Demon")
                        _numKilled++;
                break;
            case Collectable.SPIDER:
                if (tile is EnemyTile)
                    if (((EnemyTile)tile).enemyName == "Spider")
                        _numKilled++;
                break;
            case Collectable.ORC:
                if (tile is EnemyTile)
                    if (((EnemyTile)tile).enemyName == "Orc")
                        _numKilled++;
                break;
        }
        progress = (float)_numKilled / (float)_numToKill;
    }


}
