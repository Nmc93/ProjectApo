using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    public UnitData unitData;
    public Animator animator;

    public System.Action idle;
    public System.Action move;
    public System.Action attack;
    public System.Action die;

    /// <summary> �ൿ ���� �Լ� ���� </summary>
    public virtual void SetStateAction(
        System.Action idle, 
        System.Action move,
        System.Action attack, 
        System.Action die)
    {
        this.idle = idle;
        this.move = move;
        this.attack = attack;
        this.die = die;
    }

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
    public abstract bool Refresh(eUnitActionEvent eventType);
}

/// <summary> �ΰ��� ���� ��Ƽ��? </summary>
public class NormalHumanAI : UnitAI
{
    public override void Setting(UnitData unitData, Animator animator)
    {
        //���� ������ ����
        base.Setting(unitData, animator);
    }

    public override bool Refresh(eUnitActionEvent EventType)
    {
        //[0 : �ָ�],[1 : ����],[2 : ���ڵ�],[3 : �ڵ�]
        string actionKey = string.Empty;
        bool isDetailCheck = true;
        switch(EventType)
        {
            case eUnitActionEvent.Idle:         // ��� ����
                {
                    actionKey = "Idle";
                }
                break;
            case eUnitActionEvent.Move:         // �̵�
                {
                    actionKey = "Run";
                }
                break;
            case eUnitActionEvent.EnemySearch:  // �� �߰�
                {
                    actionKey = "BattleReady";
                }
                break;
            case eUnitActionEvent.EnemyAttack:  // �� ����
                {
                    actionKey = "Attack";
                }
                break;
            case eUnitActionEvent.Die:          // ���
                {
                    actionKey = "Die";
                    isDetailCheck = false;
                }
                break;
        }

        //�� ������ �ʿ��� ���
        if (isDetailCheck)
        {
            //�������� ���� Ÿ�Կ� ���� ����
            switch (unitData.weaponTbl.WeaponType)
            {
                case 0: // �Ǽ�
                    actionKey = string.Format("{0}_NoWeapon", actionKey);
                    break;
                case 1: // ����
                    actionKey = string.Format("{0}_Pistal", actionKey);
                    break;
                case 2: // ���ڵ�
                case 3: // ������
                    actionKey = string.Format("{0}_NoWeapon", actionKey);
                    break;
            }
        }

        //���ǹ��� Ű�� ���� ��쿡 �ִϸ��̼� ����
        if (!string.IsNullOrEmpty(actionKey))
        {
            animator.SetTrigger(actionKey);
            return true;
        }

        return false;
    }
}