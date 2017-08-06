using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTile : Tile
{

    Quest quest = BoardController.generateQuest();

    protected override void Start()
    {
        base.Start();
    }

    public override void use(PlayerController player)
    {
		base.use(player);
        player.setQuest(quest);
    }

}