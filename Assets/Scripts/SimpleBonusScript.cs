using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBonusScript : BonusBase
{
    public GameDataScript gameData;

    public override void BonusActivate()
    {

        gameData.ResetStickyPlayer();
    }
}
