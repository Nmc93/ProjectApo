using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBodyAnimator : Animator
{
    /// <summary> 애니메이터 세팅 </summary>
    /// <param name="animID"> 애니메이션 ID <br/> UnitAnimatorTable 참조 </param>
    public void SetAnimatior(int animID)
    {
        runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(animID);
    }

    /// <summary> 공격 애니메이션 종료 이벤트 </summary>
    public void EndAttackAnimEvent()
    {

    }
}
