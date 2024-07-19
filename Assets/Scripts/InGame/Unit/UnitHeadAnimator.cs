using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHeadAnimator : MonoBehaviour
{
    [Header("[�ִϸ�����]")]
    [SerializeField] private Animator anim;

    /// <summary> �ִϸ����� ���� </summary>
    /// <param name="animID"> �ִϸ��̼� ID <br/> UnitAnimatorTable ���� </param>
    public void SetAnimatior(int animID)
    {
        anim.runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(animID);
    }

    /// <summary> �ִϸ��̼� </summary>
    /// <param name="key"> �ִϸ��̼� Ű </param>
    public void ChangeAnimation(int key)
    {
        anim.SetTrigger(key);
    }

    /// <summary> �ִϸ��̼� ���� ���� </summary>
    public void SetPlay(bool isPlay)
    {
        anim.speed = isPlay ? 1f : 0f;
    }
}
