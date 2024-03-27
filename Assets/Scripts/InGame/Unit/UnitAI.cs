using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    /// <summary> �ൿ �׼� �̺�Ʈ </summary>
    public Action<string[], Action>[] events;

    /// <summary> ���� ���� [unit.AI�ϸ� this�� ȣ��Ǵ� ��ȣ������ ����� �����ؼ� ���] </summary>
    protected Unit unit;
    /// <summary> ���� ���� </summary>
    protected UnitData unitData;

    #region ��� �̺�Ʈ

    /// <summary> �̺�Ʈ ���� </summary>
    public bool isReservation;

    /// <summary> ���� �ð� </summary>
    protected float curWaitTime;
    /// <summary> ��� �ð� </summary>
    protected float waitTime;
    /// <summary> ��� �̺�Ʈ Ÿ�� </summary>
    [NonSerialized] public eUnitWaitEvent waitType;
    /// <summary> ��� �̺�Ʈ ���� Ÿ�̹� </summary>
    [NonSerialized] public eUnitWaitEventStartTiming timing;

    /// <summary> ������ ��� �̺�Ʈ </summary>
    protected virtual void WaitEvent() 
    {
        isReservation = false;
        waitTime = 0f;
        waitType = eUnitWaitEvent.None;
        timing = eUnitWaitEventStartTiming.StartAnim;
    }

    /// <summary> ��� �̺�Ʈ ���� </summary>
    /// <param name="waitTime"> ��� �ð� </param>
    /// <param name="waitType"> ��� �ð� �� ������ �̺�Ʈ Ÿ�� </param>
    /// <param name="timing"> �̺�Ʈ ���� Ÿ�̹� </param>
    public virtual void SettingWaitEvent(
        float waitTime,
        eUnitWaitEvent waitType,
        eUnitWaitEventStartTiming timing)
    {
        if(waitType != eUnitWaitEvent.None)
        {
            isReservation = false;
            this.waitTime = waitTime;
            this.waitType = waitType;
            this.timing = timing;
        }
    }

    /// <summary> �̺�Ʈ ���� </summary>
    public virtual void StartWaitEvent()
    {
        isReservation = true;
    }

    #endregion ��� �̺�Ʈ

    #region ����

    /// <summary> �ൿ ���� �Լ� ���� </summary>
    public virtual void SetStateAction(Action<string[], Action>[] events)
    {
        this.events = events;
    }

    /// <summary> AI ���� ���� </summary>
    /// <param name="unit"> ���� ������ </param>
    public virtual void Setting(Unit unit)
    {
        this.unit = unit;
        unitData = unit.data;

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
    public abstract bool Refresh(eUnitSituation eventType);

    /// <summary> ������ ������ ������Ʈ </summary>
    public virtual void Update()
    {
        WaitEventUpdate();
    }

    /// <summary> ��� �̺�Ʈ ��Ȳ ������Ʈ </summary>
    public virtual void WaitEventUpdate()
    {
        //��� Ʈ���Ű� �ɷ��ִ� ���
        if (isReservation)
        {
            // ������� ���
            if (curWaitTime >= waitTime)
            {
                WaitEvent();

                isReservation = false;
                waitTime = 0f;
                waitType = eUnitWaitEvent.None;
                timing = eUnitWaitEventStartTiming.StartAnim;
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
    /// <param name="EventType"> ��ȣ�ۿ� �̺�Ʈ </param>
    public override bool Refresh(eUnitSituation EventType)
    {
        //������� ��� �ƹ��͵� ���� ����
        if (unit.uState == eUnitActionEvent.Die)
        {
            return false;
        }

        // ���ÿ� Idle
        // �̵��� Move - �Ϸ�� Idle
        // Ÿ������ ������ ��� Attack
        // �� �̽� BattleReady - ���� �ð��� ���� �� ������� Idle

        // �̺�Ʈ Ÿ��
        eUnitActionEvent actionType = eUnitActionEvent.Idle;
        // �ܺ� �̺�Ʈ�� �´� ��Ȳ Ÿ������ ��ȯ
        switch (EventType)
        {
            //��Ȳ ����
            case eUnitSituation.SituationClear:
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
            case eUnitSituation.StandbyCommand:
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
            case eUnitSituation.MoveCommand:
                {
                    actionType = eUnitActionEvent.Move;
                }
                break;

            //��Ȯ�� ��ü ����
            case eUnitSituation.CreatureEncounter:
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
            case eUnitSituation.StrikeCommand:
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

        //��� �̺�Ʈ ����
        float waitTime = 0f;
        eUnitWaitEvent waitEventType = eUnitWaitEvent.None;
        eUnitWaitEventStartTiming timing = eUnitWaitEventStartTiming.StartAnim;

        // events -> [0 : ���],[1 : �̵�],[2 : Ž��],[3 : ����],[4 : ���]

        //1�� �з�
        switch (actionType)
        {
            // ��� ����
            case eUnitActionEvent.Idle:
                {
                    stateAction = events[0];
                    actionKey = "Idle";
                }
                break;

            // �̵�
            case eUnitActionEvent.Move:
                {
                    stateAction = events[1];
                    actionKey = "Run";
                }
                break;

            // �� Ž��
            case eUnitActionEvent.BattleReady:
                {
                    stateAction = events[2];
                    actionKey = "BattleReady";
                    waitTime = unit.data.tbl_RSpeed;
                    waitEventType = eUnitWaitEvent.EndObjectEmotion;
                }
                break;

            // �� ����
            case eUnitActionEvent.Attack:
                {
                    stateAction = events[3];
                    actionKey = "Attack";
                    waitTime = unit.data.f_ASpeed;
                    waitEventType = eUnitWaitEvent.ActionAfterAttack;
                }
                break;

            // ���
            case eUnitActionEvent.Die:
                {
                    stateAction = events[4];
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
            stateAction(new string[]
            {
                $"{actionKey}_Head",
                $"{actionKey}_Face",
                $"{actionKey}_Body",
                $"{actionKey}_Arm{subAnimKey}"
            },
            StartWaitEvent);

            return true;
        }

        return false;
    }

    /// <summary> ��� �̺�Ʈ </summary>
    protected override void WaitEvent()
    {
        switch (waitType)
        {
            //��Ȯ�� ��ü ���� �Ϸ�
            case eUnitWaitEvent.EndObjectEmotion:
                {
                    //��Ȯ�� ��ü is ��
                    Refresh(eUnitSituation.StrikeCommand);
                }
                break;
            // ���� ��� ���� �ൿ
            case eUnitWaitEvent.ActionAfterAttack:
                {
                    //TODO: ���� ��� ���� �ൿ �۾�
                }
                break;
        }

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
    /// <param name="EventType"> ��ȣ�ۿ� �̺�Ʈ </param>
    public override bool Refresh(eUnitSituation EventType)
    {
        //������� ��� �ƹ��͵� ���� ����
        if (unit.uState == eUnitActionEvent.Die)
        {
            return false;
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

        switch (EventType)
        {
            //��Ȳ ����
            case eUnitSituation.SituationClear:
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
            case eUnitSituation.StandbyCommand:
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
            case eUnitSituation.MoveCommand:
                {
                    actionType = eUnitActionEvent.Move;
                }
                break;

            //��Ȯ�� ��ü ����
            case eUnitSituation.CreatureEncounter:
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
            case eUnitSituation.StrikeCommand:
                {
                    actionType = eUnitActionEvent.Attack;
                }
                break;
        }

        //[0 : �ָ�],[1 : ����],[2 : ���ڵ�],[3 : �ڵ�]
        string actionKey = string.Empty;
        System.Action<string[], System.Action> stateAction = null;

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

            return true;
        }

        return false;
    }

    /// <summary> ��� �̺�Ʈ(StartWaitEvent ����� ����) </summary>
    protected override void WaitEvent()
    {
        switch (waitType)
        {
            case eUnitWaitEvent.EndObjectEmotion:

                //��Ȯ�� ��ü is ��
                Refresh(eUnitSituation.StrikeCommand);
                break;
        }

        base.WaitEvent();
    }
}