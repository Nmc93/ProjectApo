using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GEnum;

public class UnitBodyAnimator : MonoBehaviour
{
    [Header("[애니메이터]")]
    [SerializeField] private Animator anim;

    /// <summary> 공격 이벤트 </summary>
    public Action attackEvent;

    /// <summary> 애니메이션 종료 이벤트 </summary>
    public Action<eUnitActionEvent> endAnimEvent;

    /// <summary> 애니메이터 세팅 </summary>
    /// <param name="animID"> 애니메이션 ID <br/> UnitAnimatorTable 참조 </param>
    public void SetAnimatior(int animID)
    {
        anim.runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(animID);
    }

    /// <summary> 애니메이션 </summary>
    /// <param name="key"> 애니메이션 키 </param>
    public void ChangeAnimation(int key)
    {
        anim.SetTrigger(key);
    }

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

    /// <summary> 애니메이션 실행 중지 </summary>
    public void SetPlay(bool isPlay)
    {
        anim.speed = isPlay ? 1f : 0f;
    }
}
