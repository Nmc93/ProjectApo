using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    public UnitData unitData;

    #region �ൿ �׼�
    /// <summary> ��� </summary>
    public System.Action<string> idle;
    /// <summary> �̵� </summary>
    public System.Action<string> move;
    /// <summary> ���� ��� </summary>
    public System.Action<string> battleReady;
    /// <summary> ���� </summary>
    public System.Action<string> attack;
    /// <summary> ��� </summary>
    public System.Action<string> die;
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

    /// <summary> ��� �̺�Ʈ ���� </summary>
    /// <param name="waitTime"> ��� �ð� </param>
    /// <param name="waitEvent"> ��� �ð� �� ������ �̺�Ʈ Ÿ�� </param>
    public virtual void StartWaitEvent(float waitTime , eUnitWaitEvent waitEvent)
    {
        this.waitEvent = waitEvent;
        this.waitTime = waitTime;
        isOnWaitEvent = true;
    }

    /// <summary> ��� �̺�Ʈ </summary>
    protected virtual void WaitEvent() {}
    #endregion ��� �̺�Ʈ

    /// <summary> �ൿ ���� �Լ� ���� </summary>
    public virtual void SetStateAction(
        System.Action<string> idle, 
        System.Action<string> move,
        System.Action<string> battleReady,
        System.Action<string> attack, 
        System.Action<string> die)
    {
        this.idle = idle;
        this.move = move;
        this.battleReady = battleReady;
        this.attack = attack;
        this.die = die;
    }

    /// <summary> AI ���� ���� </summary>
    /// <param name="unitData"> ���� ������ </param>
    public virtual void Setting(UnitData unitData)
    {
        //���� ������ ���� ��� ��������
        if (unitData == null)
        {
            Debug.LogError("AI�� �ൿ������ �� ������ �����Ͱ� �������� �ʽ��ϴ�.");
            return;
        }

        this.unitData = unitData;
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
    public override void Setting(UnitData unitData)
    {
        //���� ������ ����
        base.Setting(unitData);
    }

    /// <summary> �̺�Ʈ ���� </summary>
    /// <param name="EventType"> ��ȣ�ۿ� �̺�Ʈ </param>
    public override bool Refresh(eUnitSituation EventType)
    {
        // ���ÿ� Idle
        // �̵��� Move - �Ϸ�� Idle
        // Ÿ������ ������ ��� Attack
        // �� �̽� BattleReady - ���� �ð��� ���� �� ������� Idle

        //�̺�Ʈ Ÿ��
        eUnitActionEvent actionType = eUnitActionEvent.Idle;

        switch (EventType)
        {
            case eUnitSituation.SituationClear:     //��Ȳ ����
                break;
            case eUnitSituation.StandbyCommand:     //��� ���
                break;
            case eUnitSituation.MoveCommand:        //�̵� ���
                break;
            case eUnitSituation.CreatureEncounter:  //��Ȯ�� ��ü ����
                break;
            case eUnitSituation.StrikeCommand:       // ����, ��� ����
                break;
        }

        //[0 : �ָ�],[1 : ����],[2 : ���ڵ�],[3 : �ڵ�]
        string actionKey = string.Empty;
        string subAnimKey = string.Empty;
        bool isDetailCheck = true;
        System.Action<string> stateAction = null;

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
            case eUnitActionEvent.BattleReady:  // �� Ž��
                {
                    actionKey = "BattleReady";
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
                    subAnimKey = "_NoWeapon";
                    break;
            }
        }

        //2�� �з� �� Ű ����
        if (stateAction != null && !string.IsNullOrEmpty(actionKey))
        {
            stateAction($"{actionKey}_Head");
            stateAction($"{actionKey}_Face");
            stateAction($"{actionKey}_Body");
            stateAction($"{actionKey}_Arm{subAnimKey}");
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