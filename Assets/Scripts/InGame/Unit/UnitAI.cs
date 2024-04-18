using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    /// <summary> ���� ���� [unit.AI�ϸ� this�� ȣ��Ǵ� ��ȣ������ ����� �����ؼ� ���] </summary>
    protected Unit unit;

    /// <summary> �̺�Ʈ ���� </summary>
    protected bool isReservation;
    /// <summary> ���� �ð� </summary>
    protected float curWaitTime;
    /// <summary> ���� �������� ���� �켱���� </summary>
    protected eUnitEventPriority curStatePriority;
    /// <summary> ���� ������� �̺�Ʈ </summary>
    protected UnitEventData waitUnitEvent;

    /// <summary> AI ���� </summary>
    /// <param name="unit"> ���� ������ </param>
    public UnitAI(Unit unit)
    {
        this.unit = unit;
        curStatePriority = eUnitEventPriority.WaitState;

        //���� ������ ���� ��� ��������
        if (unit == null)
        {
            Debug.LogError("AI�� �ൿ������ �� ������ �����Ͱ� �������� �ʽ��ϴ�.");
            return;
        }
    }

    /// <summary> AI ���� </summary>
    /// <param name="unit"> ���� ������ </param>
    public virtual void Init(Unit unit)
    {
        this.unit = unit;
        curStatePriority = eUnitEventPriority.WaitState;

        //���� ������ ���� ��� ��������
        if (unit == null)
        {
            Debug.LogError("AI�� �ൿ������ �� ������ �����Ͱ� �������� �ʽ��ϴ�.");
            return;
        }
    }

    /// <summary> AI ĳ�� ���� </summary>
    public void Release()
    {
        unit = null;

        if(waitUnitEvent != null)
        {
            UnitMgr.UnitEventReturn(waitUnitEvent);
            waitUnitEvent = null;
        }
    }

    /// <summary> ��� �̺�Ʈ ���� </summary>
    /// <param name="priority"> �̺�Ʈ �켱���� </param>
    /// <param name="evnetType"> ������ �̺�Ʈ Ÿ�� </param>
    /// <param name="waitTime"> �̺�Ʈ �ð� </param>
    public virtual void SettingWaitEvent(eUnitEventPriority priority, eUnitSituation evnetType, float waitTime)
    {
        if(evnetType != eUnitSituation.None)
        {
            waitUnitEvent = UnitMgr.GetUnitEvent();
            waitUnitEvent.SetData(priority, evnetType, waitTime);
            isReservation = true;
        }
    }

    /// <summary> ���� ��Ȳ�� �°� ���� ���� </summary>
    public abstract void Refresh();

    /// <summary> ������ ������ ������Ʈ [������ UnitUpdate()���� ó��] </summary>
    public virtual void Update()
    {
        //������� �̺�Ʈ�� ���� ���
        if (isReservation && waitUnitEvent != null)
        {
            // ������� ���
            if (curWaitTime >= waitUnitEvent.waitTime)
            {
                isReservation = false;
                curWaitTime = 0;
                Refresh();
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
    public NormalHumanAI(Unit unit) : base(unit) { }

    public override void Init(Unit unit)
    {
        //���� ������ ����
        base.Init(unit);
    }

    /// <summary> �̺�Ʈ ���� </summary>
    public override void Refresh()
    {
        // ������� ��� �ƹ��͵� ���� ����
        if (unit.uState == eUnitActionEvent.Die)
        {
            return;
        }
        // ������� ����� ���� ���
        else if(null == waitUnitEvent)
        {
            Debug.LogError($"������� ����� �����ϴ�.");
            return;
        }
        // ���� WaitState�� �ƴϰ� �켱�������� �и��� ����� ���
        else if (curStatePriority != eUnitEventPriority.WaitState && waitUnitEvent.priority > curStatePriority)
        {
            Debug.LogError($"�켱������ ���� ��� :[Cur:{curStatePriority}] [New:{waitUnitEvent.priority}]");
            return;
        }
        
        // �ܺ� �̺�Ʈ�� �´� ��Ȳ Ÿ������ ��ȯ
        eUnitActionEvent actionType = eUnitActionEvent.Idle;
        switch (waitUnitEvent.eventType)
        {
            //��Ȳ ����
            case eUnitSituation.Situation_Clear:
                {
                    switch (unit.uState)
                    {
                        // ������ ��Ȳ�� ����� ��� ��Ⱑ �ƴ϶� ���� �غ�� ���� ����
                        case eUnitActionEvent.Attack:
                            actionType = eUnitActionEvent.BattleReady;
                            break;

                        // ������ Ÿ���� ���� ���� ����
                        default:
                            actionType = eUnitActionEvent.Idle;
                            break;
                    }
                }
                break;

            //��� ���
            case eUnitSituation.Standby_Command:
                {
                    switch (unit.uState)
                    {
                        // �� �� ������ ��� ���·� ��ȯ
                        case eUnitActionEvent.Idle:
                        case eUnitActionEvent.Move:
                        case eUnitActionEvent.BattleReady:
                            actionType = eUnitActionEvent.Idle;
                            break;

                        // �������� ��� �����غ� ���·� ��ȯ
                        case eUnitActionEvent.Attack:
                            actionType = eUnitActionEvent.BattleReady;
                            break;
                    }
                }
                break;

            //�̵� ���
            case eUnitSituation.Move_Command:
                {
                    actionType = eUnitActionEvent.Move;
                }
                break;

            //��Ȯ�� ��ü ����
            case eUnitSituation.Creature_Encounter:
                {
                    //�����°ų� �̵����� ��� ���� �غ�
                    switch (unit.uState)
                    {
                        case eUnitActionEvent.Idle:
                        case eUnitActionEvent.Move:
                            actionType = eUnitActionEvent.BattleReady;
                            break;
                    }
                }
                break;

            // ����, ��� ����
            case eUnitSituation.Strike_Command:
                {
                    actionType = eUnitActionEvent.Attack;
                }
                break;
        }

        //[0 : �ָ�],[1 : ����],[2 : ���ڵ�],[3 : �ڵ�]
        string actionKey = string.Empty;
        string subAnimKey = string.Empty;
        bool isDetailCheck = true;
        Action<string[], Action> stateAction = null;

        // events -> [0 : ���],[1 : �̵�],[2 : Ž��],[3 : ����],[4 : ���]

        //1�� �з�
        switch (actionType)
        {
            // ��� ����
            case eUnitActionEvent.Idle:
                {
                    actionKey = "Idle";
                }
                break;

            // �̵�
            case eUnitActionEvent.Move:
                {
                    actionKey = "Run";
                }
                break;

            // �� Ž��
            case eUnitActionEvent.BattleReady:
                {
                    actionKey = "BattleReady";
                }
                break;

            // �� ����
            case eUnitActionEvent.Attack:
                {
                    actionKey = "Attack";
                }
                break;

            // ���
            case eUnitActionEvent.Die:
                {
                    actionKey = "Die";
                    isDetailCheck = false;
                }
                break;
        }

        //3�� �߰� �з�
        if (isDetailCheck)
        {
            //�������� ���� Ÿ�Կ� ���� ����
            switch (unit.data.weaponTbl.WeaponType)
            {
                case 0: // �Ǽ�
                    subAnimKey = "_NoWeapon";
                    break;
                case 1: // ����
                    subAnimKey = "_Pistol";
                    break;
                case 2: // ���ڵ�
                case 3: // ������
                    subAnimKey = "_LongGun";
                    break;
            }
        }

        //2�� �з� �� Ű ����
        if (stateAction != null && !string.IsNullOrEmpty(actionKey))
        {
            //���� �۾��� �켱���� ���� �� ���� �̺�Ʈ ��ȯ
            curStatePriority = waitUnitEvent.priority;
            UnitMgr.UnitEventReturn(waitUnitEvent);
            waitUnitEvent = null;

            //������ ���� �� �ִϸ��̼� ����
            unit.ChangeState(
                actionType,
                new int[]
                {
                    Animator.StringToHash($"{actionKey}_Head"),
                    Animator.StringToHash($"{actionKey}_Face"),
                    Animator.StringToHash($"{actionKey}_Body"),
                    Animator.StringToHash($"{actionKey}_Arm{subAnimKey}")
                });
        }
    }
}

public class NomalZombieAI : UnitAI
{
    public NomalZombieAI(Unit unit) : base(unit) {}

    public override void Init(Unit unit)
    {
        //���� ������ ����
        base.Init(unit);
    }

    /// <summary> �̺�Ʈ ���� </summary>
    /// <param name="eventData"> ��ȣ�ۿ� �̺�Ʈ </param>
    public override void Refresh()
    {
        //������� ��� �ƹ��͵� ���� ����
        if (unit.uState == eUnitActionEvent.Die)
        {
            return;
        }

        //�̺�Ʈ Ÿ��
        eUnitActionEvent actionType = eUnitActionEvent.Idle;

        switch (waitUnitEvent.eventType)
        {
            //��Ȳ ����
            case eUnitSituation.Situation_Clear:
                {
                    switch (unit.uState)
                    {
                        // ������ ��Ȳ�� ����� ��� ��Ⱑ �ƴ϶� ���� �غ�� ���� ����
                        case eUnitActionEvent.Attack:
                            actionType = eUnitActionEvent.BattleReady;
                            break;

                        // ������ Ÿ���� ���� ���� ����
                        default:
                            actionType = eUnitActionEvent.Idle;
                            break;
                    }
                }
                break;

            //��� ���
            case eUnitSituation.Standby_Command:
                {
                    switch (unit.uState)
                    {
                        // �� �� ������ ��� ���·� ��ȯ
                        case eUnitActionEvent.Idle:
                        case eUnitActionEvent.Move:
                        case eUnitActionEvent.BattleReady:
                            actionType = eUnitActionEvent.Idle;
                            break;

                        // �������� ��� �����غ� ���·� ��ȯ
                        case eUnitActionEvent.Attack:
                            actionType = eUnitActionEvent.BattleReady;
                            break;
                    }
                }
                break;

            //�̵� ���
            case eUnitSituation.Move_Command:
                {
                    actionType = eUnitActionEvent.Move;
                }
                break;

            //��Ȯ�� ��ü ����
            case eUnitSituation.Creature_Encounter:
                {
                    //�����°ų� �̵����� ��� ���� �غ�
                    switch (unit.uState)
                    {
                        case eUnitActionEvent.Idle:
                        case eUnitActionEvent.Move:
                            actionType = eUnitActionEvent.BattleReady;
                            break;
                    }
                }
                break;

            // ����, ��� ����
            case eUnitSituation.Strike_Command:
                {
                    actionType = eUnitActionEvent.Attack;
                }
                break;
        }

        //[0 : �ָ�],[1 : ����],[2 : ���ڵ�],[3 : �ڵ�]
        string actionKey = string.Empty;

        //1�� �з�
        switch (actionType)
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
            case eUnitActionEvent.BattleReady:  // �� �߰�
                {
                    actionKey = "Angry";
                }
                break;
            case eUnitActionEvent.Attack:       // �� ����
                {
                    actionKey = "Attack";
                }
                break;
            case eUnitActionEvent.Die:          // ���
                {
                    actionKey = "Die";
                }
                break;
        }

        //2�� �з� �� Ű ����
        if (false == string.IsNullOrEmpty(actionKey))
        {
            // �ִϸ��̼�
            unit.ChangeState(
                actionType,
                new int[]
                {
                    Animator.StringToHash($"{actionKey}_Head"),
                    Animator.StringToHash($"{actionKey}_Face{unit.data.headAnimID}"),
                    Animator.StringToHash($"{actionKey}_Body"),
                    Animator.StringToHash($"{actionKey}_Arm")
                });

            //
            waitUnitEvent = null;
        }
    }
}