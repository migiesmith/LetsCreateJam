using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTile : LootTile {

	public override void use(PlayerController player)
	{
		base.use(player);
		player.gainShield(1);
	}

}
