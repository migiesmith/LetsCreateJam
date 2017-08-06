using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTile : Tile
{

    [SerializeField] private int hp = 1;
    [SerializeField] private ResourceBar _hpBar;
    [SerializeField] public DamageType damageType;

    public string enemyName = "";


    protected override void Start()
    {
        base.Start();
        if(_hpBar != null)
            _hpBar.setValue(hp);
    }

    public void attack()
    {
        hp--;
        if(_hpBar != null)
            _hpBar.setValue(hp);
    }

    void Update()
    {

    }

    public int getHP()
    {
        return hp;
    }

}