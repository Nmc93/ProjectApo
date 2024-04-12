using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    /// <summary> ���� ���� [unit.AI�ϸ� this�� ȣ��Ǵ� ��ȣ������ ����� �����ؼ� ���] </summary>
    protected Unit unit;
    /// <summary> ���� ���� </summary>
    protected UnitData unitData;

    #region ��� �̺�Ʈ

    /// <summary> �̺�Ʈ ���� </summary>
    public bool isReservation;

    /// <summary> ���� �ð� </summary>
    protected float curWaitTime;

    /// <summary> ���� �������� ���� �켱���� </summary>
    public eUnitEventPriority curStatePriority;

    /// <summary> ���� ������� �̺�Ʈ </summary>
    public UnitEventData waitUnitEvent;

    /// <summary> ��� �̺�Ʈ ���� </summary>
    protected virtual void WaitEvent()
    {
        //������ �̺�Ʈ ������ ���� �� ��ȯ
        if(null != waitUnitEvent)
        {
            UnitMgr.UnitEventReturn(waitUnitEvent);
        }

        //���� �̺�Ʈ ����
        waitUnitEvent = null;
    }

    /// <summary> ��� �̺�Ʈ ���� </summary>
    /// <param name="priority"> �̺�Ʈ �켱���� </param>
    /// <param name="waitType"> ������ ��� �̺�Ʈ </param>
    /// <param name="timing"> �̺�Ʈ ���� Ÿ�̹� </param>
    /// <param name="waitTime"> �̺�Ʈ �ð� </param>
    public virtual void SettingWaitEvent(
        eUnitEventPriority priority,
        eUnitSituation evnetType,
        eUnitWaitEventStartTiming timing,
        float waitTime)
    {
        if(evnetType != eUnitSituation.None)
        {
            waitUnitEvent = UnitMgr.GetUnitEvent();

            waitUnitEvent.SetData(priority, evnetType, timing, waitTime);
        }
    }

    #endregion ��� �̺�Ʈ

    #region ����

    /// <summary> AI ���� ���� </summary>
    /// <param name="unit"> ���� ������ </param>
    public virtual void Setting(Unit unit)
    {
        this.unit = unit;
        unitData = unit.data;
        curStatePriority = eUnitEventPriority.None;

        //���� ������ ���� ��� ��������
        if (unit == null || unitData == null)
        {
            Debug.LogError("AI�� �ൿ������ �� ������ �����Ͱ� �������� �ʽ��ϴ�.");
            return;
        }
    }

    #endregion ����

    #region ������Ʈ

    /// <summary> ���� ��Ȳ�� �°� ���� ���� </summary>
    /// <param name="eventType"> ������ ����� �� ��ȣ�ۿ� Ÿ�� </param>
    /// <returns> ��... </returns>
    public abstract void Refresh();

    /// <summary> ������ ������ ������Ʈ </summary>
    public virtual void Update()
    {
        WaitEventUpdate();
    }

    /// <summary> ��� �̺�Ʈ ��Ȳ ������Ʈ </summary>
    public virtual void WaitEventUpdate()
    {
        //��� Ʈ���Ű� �ɷ��ִ� ���
        if (null != waitUnitEvent)
        {
            // ������� ���
            if (curWaitTime >= waitUnitEvent.waitTime)
            {
                WaitEvent();
            }
            else
            {
                curWaitTime += Time.deltaTime;
            }
        }
    }

    #endregion ������Ʈ
}

/// <summary> �ΰ��� ���� ��Ƽ��? </summary>
public class NormalHumanAI : UnitAI
{
    public override void Setting(Unit unit)
    {
        //���� ������ ����
        base.Setting(unit);
    }

    /// <summary> �̺�Ʈ ���� </summary>
    public override void Refresh()
    {
        // ������� ��� �ƹ��͵� ���� ����
        if (unit.uState == eUnitActionEvent.Die)
        {
            return;
        }
        // �߸��� ����� ���
        else if(waitUnitEvent.priority == eUnitEventPriority.None)
        {
            Debug.LogError($"�߸��� ��� : {waitUnitEvent.priority}");
            return;
        }
        // �켱�������� �и��� ����� ���
        else if(waitUnitEvent != null && curStatePriority < waitUnitEvent.priority)
        {
            Debug.LogError($"�켱������ ���� ��� :[Cur:{curStatePriority}] [New:{waitUnitEvent.priority}]");
            return;
        }
        
        // �̺�Ʈ Ÿ��
        eUnitActionEvent actionType = eUnitActionEvent.Idle;
        // �ܺ� �̺�Ʈ�� �´� ��Ȳ Ÿ������ ��ȯ
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
            switch (unitData.weaponTbl.WeaponType)
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
            //�̺�Ʈ ����
            curStatePriority = waitUnitEvent.priority;
            UnitMgr.UnitEventReturn(waitUnitEvent);
            waitUnitEvent = null;

            //������ ���� �� �ִϸ��̼� ����
            unit.ChangeState(
                actionType,
                new string[]
                {
                    $"{actionKey}_Head",
                    $"{actionKey}_Face",
                    $"{actionKey}_Body",
                    $"{actionKey}_Arm{subAnimKey}"
                });
        }
    }

    /// <summary> ��� �̺�Ʈ </summary>
    protected override void WaitEvent()
    {
        base.WaitEvent();
    }
}

public class NomalZombieAI : UnitAI
{
    public override void Setting(Unit unit)
    {
        //���� ������ ����
        base.Setting(unit);
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

        #region
        //switch (curActionType)
        //{
        //    case eUnitActionEvent.Idle:
        //        break;
        //    case eUnitActionEvent.Move:
        //        break;
        //    case eUnitActionEvent.BattleReady:
        //        break;
        //    case eUnitActionEvent.Attack:
        //        break;
        //    case eUnitActionEvent.Die:
        //        break;
        //}
        #endregion

        // ���ÿ� Idle
        // �̵��� Move - �Ϸ�� Idle
        // Ÿ������ ������ ��� Attack
        // �� �̽� BattleReady - ���� �ð��� ���� �� ������� Idle

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
        Action<string[], System.Action> stateAction = null;

        //1�� �з�
        switch (actionType)
        {
            case eUnitActionEvent.Idle:         // ��� ����
                {
                    actionKey = "Idle";
                    //stateAction = idle;
                }
                break;
            case eUnitActionEvent.Move:         // �̵�
                {
                    actionKey = "Run";
                    //stateAction = move;
                }
                break;
            case eUnitActionEvent.BattleReady:  // �� �߰�
                {
                    actionKey = "Angry";
                    //stateAction = battleReady;
                }
                break;
            case eUnitActionEvent.Attack:       // �� ����
                {
                    actionKey = "Attack";
                    //stateAction = attack;
                }
                break;
            case eUnitActionEvent.Die:          // ���
                {
                    actionKey = "Die";
                    //stateAction = die;
                }
                break;
        }

        //2�� �з� �� Ű ����
        if (stateAction != null && !string.IsNullOrEmpty(actionKey))
        {
            stateAction(new string[]
            {
                $"{actionKey}_Head",
                $"{actionKey}_Face{unit.data.headAnimID}",
                $"{actionKey}_Body",
                $"{actionKey}_Arm"
            }, null);
        }
    }

    /// <summary> ��� �̺�Ʈ(StartWaitEvent ����� ����) </summary>
    protected override void WaitEvent()
    {
        //switch (curUnitEvent.waitEventType)
        //{
        //    case eUnitWaitEvent.EndObjectEmotion:
        //
        //        //��Ȯ�� ��ü is ��
        //        Refresh(eUnitSituation.StrikeCommand);
        //        break;
        //}

        base.WaitEvent();
    }
}