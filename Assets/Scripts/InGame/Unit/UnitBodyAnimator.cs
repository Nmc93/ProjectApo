using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GEnum;

public class UnitBodyAnimator : MonoBehaviour
{
    [Header("[�ִϸ�����]")]
    [SerializeField] private Animator anim;

    /// <summary> ���� �̺�Ʈ </summary>
    public Action attackEvent;

    /// <summary> �ִϸ��̼� ���� �̺�Ʈ </summary>
    public Action<eUnitActionEvent> endAnimEvent;

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

    /// <summary> �� ���� �̺�Ʈ </summary>
    public void OnEnemyAttackEvent()
    {
        // ���� Ž�� �� ����
        attackEvent();
    }

    /// <summary> �ִϸ��̼� ���� �̺�Ʈ </summary>
    public void OnEndAnimEvent(eUnitActionEvent type)
    {
        endAnimEvent(type);
    }

    /// <summary> �ִϸ��̼� ���� ���� </summary>
    public void SetPlay(bool isPlay)
    {
        anim.speed = isPlay ? 1f : 0f;
    }
}
