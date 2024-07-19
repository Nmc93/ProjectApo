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

    /// <summary> ���� ��� �� ID [���� ���� ��� : -1]</summary>
    public int tagetEnemyID = -1;
    /// <summary> ��ġ �����ȿ� �ִ� �� ID ��� </summary>
    public List<int> searchEnemyList = new List<int>();

    /// <summary> ���� �켱���� </summary>
    public eUnitEventPriority CurStatePriority => curStatePriority;

    /// <summary> AI ���� </summary>
    /// <param name="unit"> ���� ������ </param>
    public UnitAI(Unit unit)
    {
        this.unit = unit;
        curStatePriority = eUnitEventPriority.WaitState;
        tagetEnemyID = -1;
        searchEnemyList.Clear();

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
        tagetEnemyID = -1;
        searchEnemyList.Clear();

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
    public virtual void SettingWaitEvent(eUnitEventPriority priority, eUnitSituation evnetType, float waitTime = 0)
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

    #region Ÿ�� ����

    /// <summary> Ÿ���� �˻��ؼ� �Է��ؾ� �� ��� </summary>
    /// <param name="uID"> Ÿ���� UID </param>
    /// <returns> Ÿ�� ��Ͽ� ������ ��� true ��ȯ </returns>
    public virtual bool AddTarget(int uID)
    {
        // ������ �̸��� UID�� ��ȿ�ϰ� �ش� UID�� ���� ������ �������� ���
        if (UnitMgr.GetUnitType(uID) != unit.data.unitType)
        {
            //�߰ߵ� ����� 
            if (false == searchEnemyList.Contains(uID))
            {
                searchEnemyList.Add(uID);

                //���� Ÿ���� ���� ���
                if (searchEnemyList.Count == 1 && tagetEnemyID == -1)
                {
                    //Ÿ�� ���� �� �̺�Ʈ ����
                    tagetEnemyID = searchEnemyList[0];

                    SettingWaitEvent(
                        eUnitEventPriority.Situation_Response,
                        eUnitSituation.Creature_Encounter);
                }

                return true;
            }
        }

        return false;
    }

    /// <summary> �ش� UID�� ���� Ÿ���� ��Ͽ��� ���� </summary>
    /// <param name="uID"> Ÿ���� UID </param>
    public virtual void RemoveTarget(int uID)
    {
        // ����� ��Ͽ��� ����
        searchEnemyList.Remove(uID);

        // ���� Ÿ���� ��� Ÿ�� ����
        if(tagetEnemyID == uID)
        {
            tagetEnemyID = -1;
        }
    }

    /// <summary> ���� ���õ� ���� Ȯ���ϰ� ���� ���� ���� ���ɼ� üũ </summary>
    /// <returns> ���� ã�� �� ���ٸ� false ��ȯ </returns>
    public bool IsFindEnemy()
    {
        //TODO : ���� ��� ���� �� �׿� ���� ���� �۾�

        // �������� ������ �ְ� �ش� ������ ����ִ� ���
        if (tagetEnemyID != -1 && UnitMgr.IsUnitAlive(tagetEnemyID))
        {
            return true;
        }

        // ���ų� �׾��� ��� ���� Ÿ�� üũ
        while (searchEnemyList.Count > 0)
        {
            //
            tagetEnemyID = searchEnemyList[0];
            if (UnitMgr.IsUnitAlive(tagetEnemyID))
            {
                return true;
            }
            else
            {
                RemoveTarget(tagetEnemyID);
            }
        }

        return false;
    }

    #endregion Ÿ�� ����
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
        #region ���ܻ���

        // ������� ��� �ƹ��͵� ���� ����
        if (unit.uState == eUnitActionEvent.Die)
        {
            // ������ Ǯ�� �ǵ����� �۾��� ���
            if (waitUnitEvent.eventType == eUnitSituation.Return_Unit)
            {
                //�̺�Ʈ ���� ���� �� ��ȯ
                curStatePriority = eUnitEventPriority.WaitState;
                UnitMgr.UnitEventReturn(waitUnitEvent);
                waitUnitEvent = null;

                //������ Ǯ�� ��ȯ
                UnitMgr.ReturnUnitToPool(unit);
            }

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
        // ������ Ǯ�� �ǵ����� �۾��� ���
        else if (waitUnitEvent.eventType == eUnitSituation.Return_Unit)
        {
            //�̺�Ʈ ���� ���� �� ��ȯ
            curStatePriority = eUnitEventPriority.WaitState;
            UnitMgr.UnitEventReturn(waitUnitEvent);
            waitUnitEvent = null;

            //������ Ǯ�� ��ȯ
            UnitMgr.ReturnUnitToPool(unit);
            return;
        }

        #endregion ���ܻ���

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
                            {
                                actionType = IsFindEnemy() ? eUnitActionEvent.BattleReady : eUnitActionEvent.Idle;
                            }
                            break;

                        // ������ Ÿ���� ���� ���� ����
                        default:
                            {
                                actionType = eUnitActionEvent.Idle;
                            }
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
                            {
                                actionType = eUnitActionEvent.Idle;
                            }
                            break;

                        // �������� ��� �����غ� ���·� ��ȯ
                        case eUnitActionEvent.Move:
                        case eUnitActionEvent.BattleReady:
                        case eUnitActionEvent.Attack:
                            {
                                actionType = IsFindEnemy() ? eUnitActionEvent.BattleReady : eUnitActionEvent.Idle;
                            }
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
                            {
                                if(IsFindEnemy() == false)
                                {
                                    return;
                                }

                                actionType = eUnitActionEvent.BattleReady;
                            }
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

            // ü�� ����
            case eUnitSituation.HP_Zero:
                {
                    actionType = eUnitActionEvent.Die;
                }
                break;
        }

        //[0 : �ָ�],[1 : ����],[2 : ���ڵ�],[3 : �ڵ�]
        string actionKey = string.Empty;
        string subAnimKey = string.Empty;
        bool isDetailCheck = true;

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
        if (false == string.IsNullOrEmpty(actionKey))
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

            //���� �̺�Ʈ
            SetInternalEvnet(actionType);
        }
    }

    /// <summary> ���� ���� �� ����Ǵ� ���� �̺�Ʈ </summary>
    private void SetInternalEvnet(eUnitActionEvent type)
    {
        eUnitSituation nextSituation = eUnitSituation.None;
        float waitTime = 0;
        switch (type)
        {
            case eUnitActionEvent.BattleReady:
                {
                    nextSituation = eUnitSituation.Strike_Command;
                    waitTime = unit.data.f_ASpeed;
                }
                break;
        }

        //���°� ������ ��쿡�� ����
        if (nextSituation != eUnitSituation.None)
        {
            // ��� ���� �̺�Ʈ ����
            SettingWaitEvent(
                curStatePriority,   // ���� �켱������ ���
                nextSituation,      // �ùķ��̼� Ÿ��
                waitTime);          // ��� �ð�
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
    public override void Refresh()
    {
        #region ���� ó��

        // ������� ��� �ƹ��͵� ���� ����
        if (unit.uState == eUnitActionEvent.Die)
        {
            // ������ Ǯ�� �ǵ����� �۾��� ���
            if (waitUnitEvent.eventType == eUnitSituation.Return_Unit)
            {
                //�̺�Ʈ ���� ���� �� ��ȯ
                curStatePriority = eUnitEventPriority.WaitState;
                UnitMgr.UnitEventReturn(waitUnitEvent);
                waitUnitEvent = null;

                //������ Ǯ�� ��ȯ
                UnitMgr.ReturnUnitToPool(unit);
            }

            return;
        }
        // ������� ����� ���� ���
        else if (null == waitUnitEvent)
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

        #endregion ���� ó��

        //�̺�Ʈ Ÿ��
        eUnitActionEvent actionType = eUnitActionEvent.Idle;

        // ���� ��Ȳ�� �´� �ൿ Ÿ�� ����
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

            // ü�� ����
            case eUnitSituation.HP_Zero:
                {
                    actionType = eUnitActionEvent.Die;
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

            waitUnitEvent = null;
        }
    }
}