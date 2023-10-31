using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkBonusScript : BonusBase
{
    public GameDataScript gameData;

    public override void BonusActivate()
    {
        var sizeChanges = gameData.playerSizeChanges;

        // Если привысили лимит на уменьшение то 
        // ничего не делаем
        if (sizeChanges <= -2)
        {
            return;
        }

        var playerObj = GameObject.FindGameObjectWithTag("Player");

        var currentScale = playerObj.transform.localScale;

        playerObj.transform.localScale = new Vector3(currentScale.x / 1.5f, currentScale.y, currentScale.z);
        gameData.playerSizeChanges--;
    }
}
