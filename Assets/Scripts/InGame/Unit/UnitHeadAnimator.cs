using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHeadAnimator : Animator
{
    /// <summary> 애니메이터 세팅 </summary>
    /// <param name="animID"> 애니메이션 ID <br/> UnitAnimatorTable 참조 </param>
    public void SetAnimatior(int animID)
    {
        runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(animID);
    }
}
