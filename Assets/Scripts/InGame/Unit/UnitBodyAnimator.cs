using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitBodyAnimator : Animator
{
    [Header("[�̺�Ʈ]"),Tooltip("���� �̺�Ʈ")]
    [SerializeField] private Action attackEvent;

    /// <summary> �ִϸ����� ���� </summary>
    /// <param name="animID"> �ִϸ��̼� ID <br/> UnitAnimatorTable ���� </param>
    public void SetAnimatior(int animID, Unit unit)
    {
        runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(animID);
    }

    /// <summary> �� ���� �̺�Ʈ </summary>
    public void EnemyAttackEvent()
    {
        //���� ����... ��... ����Ʈ? ���߿�

        // ���� Ž�� �� ����
        attackEvent();
    }

    /// <summary> ���� �ִϸ��̼� ���� �̺�Ʈ </summary>
    public void EndAttackAnimEvent()
    {

    }
}
