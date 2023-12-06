using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    #region 행동 액션
    /// <summary> 대기 </summary>
    public System.Action<string> idle;
    /// <summary> 이동 </summary>
    public System.Action<string> move;
    /// <summary> 공격 대기 </summary>
    public System.Action<string> battleReady;
    /// <summary> 공격 </summary>
    public System.Action<string> attack;
    /// <summary> 사망 </summary>
    public System.Action<string> die;
    #endregion 행동 액션

    #region 대기 이벤트
    /// <summary> 대기 이벤트 On,Off 여부 </summary>
    protected bool isOnWaitEvent = false;
    /// <summary> 대기 시간 </summary>
    protected float waitTime;
    /// <summary> 지난 시간 </summary>
    protected float curWaitTime;

    /// <summary> 현재 대기 이벤트 타입 </summary>
    protected eUnitWaitEvent waitEvent;

    /// <summary> 현재 유닛 [unit.AI하면 this가 호출되는 상호참조의 관계라 조심해서 사용] </summary>
    protected Unit unit;
    /// <summary> 유닛 정보 </summary>
    protected UnitData unitData;

    /// <summary> 대기 이벤트 시작 </summary>
    /// <param name="waitTime"> 대기 시간 </param>
    /// <param name="waitEvent"> 대기 시간 후 실행할 이벤트 타입 </param>
    public virtual void StartWaitEvent(float waitTime , eUnitWaitEvent waitEvent)
    {
        this.waitEvent = waitEvent;
        this.waitTime = waitTime;
        isOnWaitEvent = true;
    }

    /// <summary> 대기 이벤트 </summary>
    protected virtual void WaitEvent() {}
    #endregion 대기 이벤트

    /// <summary> 행동 관련 함수 세팅 </summary>
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

    /// <summary> AI 정보 세팅 </summary>
    /// <param name="unit"> 유닛 데이터 </param>
    public virtual void Setting(Unit unit)
    {
        //유닛 정보가 없을 경우 강제종료
        if (unitData == null)
        {
            Debug.LogError("AI의 행동기준이 될 유닛의 데이터가 존재하지 않습니다.");
            return;
        }
        this.unit = unit;
        unitData = unit.data;
    }

    /// <summary> 현재 상황에 맞게 상태 갱신 </summary>
    /// <param name="eventType"> 유닛의 월드와 한 상호작용 타입 </param>
    /// <returns> 흠... </returns>
    public abstract bool Refresh(eUnitSituation eventType);

    /// <summary> 유닛의 정보를 업데이트 </summary>
    public virtual void Update()
    {
        //대기 트리거가 걸려있는 경우
        if(isOnWaitEvent)
        {
            // 대기 완료
            if(curWaitTime >= waitTime)
            {
                isOnWaitEvent = false;
                curWaitTime = waitTime = 0;

                //대기 이벤트 실행
                WaitEvent();
            }
            else
            {
                curWaitTime += Time.deltaTime;
            }
        }
    }
}

/// <summary> 인간형die; 보스 고티죠? </summary>
public class NormalHumanAI : UnitAI
{
    public override void Setting(Unit unit)
    {
        //유닛 데이터 세팅
        base.Setting(unit);
    }

    /// <summary> 이벤트 갱신 </summary>
    /// <param name="EventType"> 상호작용 이벤트 </param>
    public override bool Refresh(eUnitSituation EventType)
    {
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

        // 평상시엔 Idle
        // 이동시 Move - 완료시 Idle
        // 타겟으로 결정된 경우 Attack
        // 적 미싱 BattleReady - 일정 시간이 지난 후 경계종료 Idle

        //이벤트 타입
        eUnitActionEvent actionType = eUnitActionEvent.Idle;

        switch (EventType)
        {
            case eUnitSituation.SituationClear:     //상황 종료
                {
                    switch (unit.uState)
                    {
                        case eUnitActionEvent.Attack:   // 공격중 상황이 종료될 경우 대기가 아니라 전투 준비로 상태 변경
                            actionType = eUnitActionEvent.BattleReady;
                            break;
                        default:                        // 나머지 타입은 대기로 상태 변경
                            actionType = eUnitActionEvent.Idle;
                            break;
                    }
                }
                break;
            case eUnitSituation.StandbyCommand:     //대기 명령
                {
                    switch(unit.uState)
                    {
                        case eUnitActionEvent.Idle:
                            break;
                        case eUnitActionEvent.Move:
                            break;
                        case eUnitActionEvent.BattleReady:
                            break;
                        case eUnitActionEvent.Attack:
                            break;
                        case eUnitActionEvent.Die:
                            break;
                    }
                }
                break;
            case eUnitSituation.MoveCommand:        //이동 명령
                {
                }
                break;
            case eUnitSituation.CreatureEncounter:  //미확인 물체 조우
                {
                }
                break;
            case eUnitSituation.StrikeCommand:       // 지점, 대상 공격
                {
                }
                break;
        }

        //[0 : 주먹],[1 : 권총],[2 : 반자동],[3 : 자동]
        string actionKey = string.Empty;
        string subAnimKey = string.Empty;
        bool isDetailCheck = true;
        System.Action<string> stateAction = null;

        //1차 분류
        switch (actionType)
        {
            case eUnitActionEvent.Idle:         // 대기 상태
                {
                    actionKey = "Idle";
                    stateAction = idle;
                }
                break;
            case eUnitActionEvent.Move:         // 이동
                {
                    actionKey = "Run";
                    stateAction = move;
                }
                break;
            case eUnitActionEvent.BattleReady:  // 적 탐색
                {
                    actionKey = "BattleReady";
                    stateAction = battleReady;
                }
                break;
            case eUnitActionEvent.Attack:       // 적 공격
                {
                    actionKey = "Attack";
                    stateAction = attack;
                }
                break;
            case eUnitActionEvent.Die:          // 사망
                {
                    actionKey = "Die";
                    stateAction = die;
                    isDetailCheck = false;
                }
                break;
        }

        //3차 추가 분류
        if (isDetailCheck)
        {
            //착용중인 무기 타입에 따라 세팅
            switch (unitData.weaponTbl.WeaponType)
            {
                case 0: // 맨손
                    subAnimKey = "_NoWeapon";
                    break;
                case 1: // 권총
                    subAnimKey = "_Pistol";
                    break;
                case 2: // 반자동
                case 3: // 연사총
                    subAnimKey = "_NoWeapon";
                    break;
            }
        }

        //2차 분류 및 키 조합
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

    /// <summary> 대기 이벤트(StartWaitEvent 종료시 실행) </summary>
    protected override void WaitEvent()
    {
        base.WaitEvent();

        switch (waitEvent)
        {
            case eUnitWaitEvent.EndObjectEmotion:

                //미확인 물체 is 적
                Refresh(eUnitSituation.StrikeCommand);
                break;
        }
    }
}