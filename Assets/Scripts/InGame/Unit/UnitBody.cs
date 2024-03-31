using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBody : MonoBehaviour
{
    [Header("[몸통 애니메이터]"), Tooltip("몸통 애니메이터")]
    [SerializeField] Animator animator;

    /// <summary> 애니메이터 세팅 </summary>
    /// <param name="animID"> 애니메이션 ID <br/> UnitAnimatorTable 참조 </param>
    public void SetAnimatior(int animID)
    {
        animator.runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(animID);
    }

    /// <summary> 애니메이션 변경 </summary>
    /// <param name="key"> 애니메이션 변경 키 
    /// <br/> [0 : 몸통 + 다리 애니메이션]
    /// <br/> [1 : 팔 애니메이션]</param>
    public void ChangeAnim(string[] key)
    {
        //몸 + 다리 애니메이션
        animator.SetTrigger(key[0]);
        //팔 애니메이션
        animator.SetTrigger(key[1]);
    }

    /// <summary> 공격 애니메이션 종료 이벤트 </summary>
    public void EndAttackAnimEvent()
    {

    }
}
