using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitBodyAnimator : Animator
{
    [Header("[이벤트]"),Tooltip("공격 이벤트")]
    [SerializeField] private Action attackEvent;

    /// <summary> 애니메이터 세팅 </summary>
    /// <param name="animID"> 애니메이션 ID <br/> UnitAnimatorTable 참조 </param>
    public void SetAnimatior(int animID, Unit unit)
    {
        runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(animID);
    }

    /// <summary> 적 공격 이벤트 </summary>
    public void EnemyAttackEvent()
    {
        //유닛 공격... 흠... 이펙트? 나중에

        // 유닛 탐색 및 공격
        attackEvent();
    }

    /// <summary> 공격 애니메이션 종료 이벤트 </summary>
    public void EndAttackAnimEvent()
    {

    }
}
