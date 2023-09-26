using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    public UnitData unitData;
    public Animator animator;

    #region �ൿ �׼�
    /// <summary> ��� </summary>
    public System.Action idle;
    /// <summary> �̵� </summary>
    public System.Action move;
    /// <summary> ���� </summary>
    public System.Action attack;
    /// <summary> ��� </summary>
    public System.Action die;
    #endregion �ൿ �׼�

    #region ��� �̺�Ʈ
    /// <summary> ��� �̺�Ʈ On,Off ���� </summary>
    protected bool isOnWaitEvent = false;
    /// <summary> ��� �ð� </summary>
    protected float waitTime;
    /// <summary> ���� �ð� </summary>
    protected float curWaitTime;

    public virtual void StartWaitEvent(float waitTime)
    {
        this.waitTime = waitTime;
        isOnWaitEvent = true;
    }
    /// <summary> ��� �̺�Ʈ </summary>
    protected virtual void WaitEvent() {}
    #endregion ��� �̺�Ʈ

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
        else if(animator == null)
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

    /// <summary> ������ ������ ������Ʈ </summary>
    public virtual void Update()
    {
        //��� Ʈ���Ű� �ɷ��ִ� ���
        if(isOnWaitEvent)
        {
            // ��� �Ϸ�
            if(curWaitTime >= waitTime)
            {
                isOnWaitEvent = false;
                curWaitTime = waitTime = 0;
            }
            else
            {
                curWaitTime += Time.deltaTime;
            }
        }
    }
}

/// <summary> �ΰ��� ���� ��Ƽ��? </summary>
public class NormalHumanAI : UnitAI
{
    public override void Setting(UnitData unitData, Animator animator)
    {
        //���� ������ ����
        base.Setting(unitData, animator);
    }

    /// <summary> �̺�Ʈ ���� </summary>
    /// <param name="EventType"> ��ȣ�ۿ� �̺�Ʈ </param>
    public override bool Refresh(eUnitActionEvent EventType)
    {
        //. ���ÿ� Idle
        //. �̵��� Move - �Ϸ�� Idle
        //. Ÿ������ ������ ��� Attack
        //. �� �̽� BattleReady - ���� �ð��� ���� �� ������� Idle

        //[0 : �ָ�],[1 : ����],[2 : ���ڵ�],[3 : �ڵ�]
        string actionKey = string.Empty;
        bool isDetailCheck = true;
        switch(EventType)
        {
            case eUnitActionEvent.Idle:         // ��� ����
                {
                    actionKey = "Idle";
                    idle();
                }
                break;
            case eUnitActionEvent.Move:         // �̵�
                {
                    actionKey = "Run";
                    move();
                }
                break;
            case eUnitActionEvent.EnemySearch:  // �� Ž��
                {
                    actionKey = "BattleReady";
                    idle();
                }
                break;
            case eUnitActionEvent.EnemyAttack:  // �� ����
                {
                    actionKey = "Attack";
                    attack();
                }
                break;
            case eUnitActionEvent.Die:          // ���
                {
                    actionKey = "Die";
                    isDetailCheck = false;
                    die();
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

    protected override void WaitEvent()
    {
        base.WaitEvent();
    }

}