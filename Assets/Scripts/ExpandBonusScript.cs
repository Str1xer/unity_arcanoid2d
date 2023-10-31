using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandBonusScript : BonusBase
{
    public GameDataScript gameData;

    public override void BonusActivate()
    {
        var sizeChanges = gameData.playerSizeChanges;

        // Если привысили лимит на увеличение то 
        // ничего не делаем
        if (sizeChanges >= 2) {
            return;
        }

        var playerObj = GameObject.FindGameObjectWithTag("Player");

        var currentScale = playerObj.transform.localScale;

        playerObj.transform.localScale = new Vector3(1.5f * currentScale.x, currentScale.y, currentScale.z);
        gameData.playerSizeChanges++;
    }
}
