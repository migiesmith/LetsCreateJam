using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTile : Tile
{

    Quest quest;

    void Start()
    {
        // Set quest
        // quest = ???;
    }

    public override void use(PlayerController player)
    {
		base.use(player);
        player.setQuest(quest);
    }

}