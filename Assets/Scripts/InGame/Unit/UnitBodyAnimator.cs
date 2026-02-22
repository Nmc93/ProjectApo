using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GEnum;

public class UnitBodyAnimator : UnitAnimator
{
    /// <summary> 공격 이벤트 </summary>
    public Action attackEvent;

    /// <summary> 애니메이션 종료 이벤트 </summary>
    public Action<eUnitActionEvent> endAnimEvent;

    /// <summary> 적 공격 이벤트 </summary>
    public void OnEnemyAttackEvent()
    {
        // 유닛 탐색 및 공격
        attackEvent();
    }

    /// <summary> 애니메이션 종료 이벤트 </summary>
    public void OnEndAnimEvent(eUnitActionEvent type)
    {
        endAnimEvent(type);
    }
}
