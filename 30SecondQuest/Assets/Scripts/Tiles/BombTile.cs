﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTile : LootTile {

	public override void use(PlayerController player)
	{
		base.use(player);
		player.gainBomb(1);
	}

}
