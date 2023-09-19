using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    public UnitData unitData;
    public Animator animator;

    /// <summary> AI ���� ���� </summary>
    /// <param name="unitData"> ���� ������ </param>
    /// <param name="animator"> ������ �ִϸ����� </param>
    public virtual void Setting(UnitData unitData, Animator animator)
    {
        //���� ������ ���� ��� ��������
        if (unitData == null)
        {
            Debug.LogError("AI�� �ൿ������ �� ������ �����Ͱ� �������� �ʽ��ϴ�.");
            return;
        }
        //�ִϸ����Ͱ� ���� ��쿡�� ���� ����
        else if(animator = null)
        {
            Debug.LogError("���ֿ� �ִϸ����Ͱ� �����ϴ�.");
            return;
        }

        this.unitData = unitData;
        this.animator = animator;
    }

    /// <summary> ���� ��Ȳ�� �°� ���� ���� </summary>
    /// <param name="eventType"> ������ ����� �� ��ȣ�ۿ� Ÿ�� </param>
    /// <returns> ��... </returns>
    public abstract string Refresh(eUnitActionEvent eventType);
}

/// <summary> �ΰ��� ���� ��Ƽ��? </summary>
public class NormalHumanAI : UnitAI
{
    public override void Setting(UnitData unitData, Animator animator)
    {
        //���� ������ ����
        base.Setting(unitData, animator);
    }

    public override string Refresh(eUnitActionEvent EventType)
    {
        string actionKey = string.Empty;
        switch(EventType)
        {
            case eUnitActionEvent.NoEvent:
                {
                    actionKey = "";
                }
                break;
            case eUnitActionEvent.EnemySearch:
                {
                    actionKey = "";
                }
                break;
        }

        //[0 : �ָ�],[1 : ����],[2 : ���ڵ�],[3 : �ڵ�]
        if (!string.IsNullOrEmpty(actionKey))
        {
            animator.SetInteger(actionKey,unitData.weaponTbl.WeaponType);
        }

        return actionKey;
    }
}