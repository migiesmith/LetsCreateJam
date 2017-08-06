using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartTile : LootTile {

	public override void use(PlayerController player)
	{
		base.use(player);
		player.heal(1);
	}

}
