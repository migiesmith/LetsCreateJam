using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTile : LootTile {

	public override void use(PlayerController player)
	{
		base.use(player);
		player.gainSwords(1);
	}

}
