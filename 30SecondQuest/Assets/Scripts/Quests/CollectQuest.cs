using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectQuest : Quest
{

    public enum Collectable
    {
        HEART = 0, SHIELD = 1, SWORD = 2
    }
    private Collectable _toCollect;

    private int _numToCollect = new System.Random().Next(4, 8);
	private int _numCollected = 0;

    public CollectQuest()
    {
        int collectableVal = new System.Random().Next(0, 3);
        switch (collectableVal)
        {
            case (int)Collectable.HEART:
                _toCollect = Collectable.HEART;
                break;
            case (int)Collectable.SHIELD:
                _toCollect = Collectable.SHIELD;
                break;
            case (int)Collectable.SWORD:
                _toCollect = Collectable.SWORD;
                break;
        }
    }

    public override string getQuestText()
    {
        string val = "Collect "+ _numToCollect +" ";
        switch (_toCollect)
        {
            case Collectable.HEART:
                val += "hearts. ";
                break;
            case Collectable.SHIELD:
                val += "shields. ";
                break;
            case Collectable.SWORD:
                val += "swords. ";
                break;
        }
        return val + progress*100.0f + "%";
    }

    public override void processTile(Tile tile)
    {switch (_toCollect)
        {
            case Collectable.HEART:
				if(tile is HeartTile)
					_numCollected++;
                break;
            case Collectable.SHIELD:
				if(tile is ShieldTile)
					_numCollected++;
                break;
            case Collectable.SWORD:
				if(tile is SwordTile)
					_numCollected++;
                break;
        }
		progress = (float)_numCollected / (float)_numToCollect;
    }


}
