using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBonusScript : BonusBase
{
    public GameDataScript gameData;

    public override void BonusActivate()
    {

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var spriteRenderer = playerObj.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;

        gameData.isPlayerSticky = true;
    }
}
