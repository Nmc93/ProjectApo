using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHeadAnimator : MonoBehaviour
{
    [Header("[애니메이터]")]
    [SerializeField] private Animator anim;

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

    /// <summary> 애니메이션 실행 중지 </summary>
    public void SetPlay(bool isPlay)
    {
        anim.speed = isPlay ? 1f : 0f;
    }
}
