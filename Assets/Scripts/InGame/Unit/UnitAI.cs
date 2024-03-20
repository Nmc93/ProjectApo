using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    #region �ൿ �׼�
    /// <summary> ��� </summary>
    public System.Action<string[], System.Action> idle;
    /// <summary> �̵� </summary>
    public System.Action<string[], System.Action> move;
    /// <summary> ���� ��� </summary>
    public System.Action<string[], System.Action> battleReady;
    /// <summary> ���� </summary>
    public System.Action<string[], System.Action> attack;
    /// <summary> ��� </summary>
    public System.Action<string[], System.Action> die;
    #endregion �ൿ �׼�

    #region ��� �̺�Ʈ
    /// <summary> ��� �̺�Ʈ On,Off ���� </summary>
    protected bool isOnWaitEvent = false;
    /// <summary> ��� �ð� </summary>
    protected float waitTime;
    /// <summary> ���� �ð� </summary>
    protected float curWaitTime;

    /// <summary> ���� ��� �̺�Ʈ Ÿ�� </summary>
    protected eUnitWaitEvent waitEvent;

    /// <summary> ���� ���� [unit.AI�ϸ� this�� ȣ��Ǵ� ��ȣ������ ����� �����ؼ� ���] </summary>
    protected Unit unit;
    /// <summary> ���� ���� </summary>
    protected UnitData unitData;

    /// <summary> ��� �̺�Ʈ ���� </summary>
    /// <param name="waitTime"> ��� �ð� </param>
    /// <param name="waitEvent"> ��� �ð� �� ������ �̺�Ʈ Ÿ�� </param>
    public virtual void StartWaitEvent(float waitTime , eUnitWaitEvent waitEvent)
    {
        if(waitEvent != eUnitWaitEvent.None)
        {
            this.waitEvent = waitEvent;
            this.waitTime = waitTime;
            isOnWaitEvent = true;
        }
    }

    /// <summary> ��� �̺�Ʈ </summary>
    protected virtual void WaitEvent() {}
    #endregion ��� �̺�Ʈ

    /// <summary> �ൿ ���� �Լ� ���� </summary>
    public virtual void SetStateAction(
        System.Action<string[], System.Action> idle, 
        System.Action<string[], System.Action> move,
        System.Action<string[], System.Action> battleReady,
        System.Action<string[], System.Action> attack, 
        System.Action<string[], System.Action> die)
    {
        this.idle = idle;
        this.move = move;
        this.battleReady = battleReady;
        this.attack = attack;
        this.die = die;
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

    /// <summary> ���� ��Ȳ�� �°� ���� ���� </summary>
    /// <param name="eventType"> ������ ����� �� ��ȣ�ۿ� Ÿ�� </param>
    /// <returns> ��... </returns>
    public abstract bool Refresh(eUnitSituation eventType);

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

                //��� �̺�Ʈ ����
                WaitEvent();
            }
            else
            {
                curWaitTime += Time.deltaTime;
            }
        }
    }
}

/// <summary> �ΰ���die; ���� ��Ƽ��? </summary>
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
        System.Action<string[], System.Action> stateAction = null;

        float waitTime = 0f;
        eUnitWaitEvent waitEventType = eUnitWaitEvent.None;

        //1�� �з�
        switch (actionType)
        {
            case eUnitActionEvent.Idle:         // ��� ����
                {
                    actionKey = "Idle";
                    stateAction = idle;
                }
                break;
            case eUnitActionEvent.Move:         // �̵�
                {
                    stateAction = move;
                    actionKey = "Run";
                }
                break;
            case eUnitActionEvent.BattleReady:  // �� Ž��
                {
                    stateAction = battleReady;
                    actionKey = "BattleReady";
                    waitTime = unit.data.reactionSpeed;
                    waitEventType = eUnitWaitEvent.EndObjectEmotion;
                }
                break;
            case eUnitActionEvent.Attack:       // �� ����
                {
                    stateAction = attack;
                    actionKey = "Attack";
                }
                break;
            case eUnitActionEvent.Die:          // ���
                {
                    actionKey = "Die";
                    stateAction = die;
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
            () => { StartWaitEvent(waitTime, waitEventType); });

            return true;
        }

        return false;
    }

    /// <summary> ��� �̺�Ʈ(StartWaitEvent ����� ����) </summary>
    protected override void WaitEvent()
    {
        base.WaitEvent();

        switch (waitEvent)
        {
            case eUnitWaitEvent.EndObjectEmotion: //��Ȯ�� ��ü ���� �Ϸ�
                {
                    //��Ȯ�� ��ü is ��
                    Refresh(eUnitSituation.StrikeCommand);
                }
                break;
        }
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
                    stateAction = idle;
                }
                break;
            case eUnitActionEvent.Move:         // �̵�
                {
                    actionKey = "Run";
                    stateAction = move;
                }
                break;
            case eUnitActionEvent.BattleReady:  // �� �߰�
                {
                    actionKey = "Angry";
                    stateAction = battleReady;
                }
                break;
            case eUnitActionEvent.Attack:       // �� ����
                {
                    actionKey = "Attack";
                    stateAction = attack;
                }
                break;
            case eUnitActionEvent.Die:          // ���
                {
                    actionKey = "Die";
                    stateAction = die;
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
        base.WaitEvent();

        switch (waitEvent)
        {
            case eUnitWaitEvent.EndObjectEmotion:

                //��Ȯ�� ��ü is ��
                Refresh(eUnitSituation.StrikeCommand);
                break;
        }
    }
}