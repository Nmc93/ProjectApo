using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBody : MonoBehaviour
{
    [Header("[���� �ִϸ�����]"), Tooltip("���� �ִϸ�����")]
    [SerializeField] Animator animator;

    /// <summary> �ִϸ����� ���� </summary>
    /// <param name="animID"> �ִϸ��̼� ID <br/> UnitAnimatorTable ���� </param>
    public void SetAnimatior(int animID)
    {
        animator.runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(animID);
    }

    /// <summary> �ִϸ��̼� ���� </summary>
    /// <param name="key"> �ִϸ��̼� ���� Ű 
    /// <br/> [0 : ���� + �ٸ� �ִϸ��̼�]
    /// <br/> [1 : �� �ִϸ��̼�]</param>
    public void ChangeAnim(string[] key)
    {
        //�� + �ٸ� �ִϸ��̼�
        animator.SetTrigger(key[0]);
        //�� �ִϸ��̼�
        animator.SetTrigger(key[1]);
    }

    /// <summary> ���� �ִϸ��̼� ���� �̺�Ʈ </summary>
    public void EndAttackAnimEvent()
    {

    }
}
