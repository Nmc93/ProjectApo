using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHeadAnimator : Animator
{
    /// <summary> �ִϸ����� ���� </summary>
    /// <param name="animID"> �ִϸ��̼� ID <br/> UnitAnimatorTable ���� </param>
    public void SetAnimatior(int animID)
    {
        runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(animID);
    }
}
