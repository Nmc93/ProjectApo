using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    #region 행동 액션
    /// <summary> 대기 </summary>
    public System.Action<string[], System.Action> idle;
    /// <summary> 이동 </summary>
    public System.Action<string[], System.Action> move;
    /// <summary> 공격 대기 </summary>
    public System.Action<string[], System.Action> battleReady;
    /// <summary> 공격 </summary>
    public System.Action<string[], System.Action> attack;
    /// <summary> 사망 </summary>
    public System.Action<string[], System.Action> die;
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
        if(waitEvent != eUnitWaitEvent.None)
        {
            this.waitEvent = waitEvent;
            this.waitTime = waitTime;
            isOnWaitEvent = true;
        }
    }

    /// <summary> 대기 이벤트 </summary>
    protected virtual void WaitEvent() {}
    #endregion 대기 이벤트

    /// <summary> 행동 관련 함수 세팅 </summary>
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

    /// <summary> AI 정보 세팅 </summary>
    /// <param name="unit"> 유닛 데이터 </param>
    public virtual void Setting(Unit unit)
    {
        this.unit = unit;
        unitData = unit.data;

        //유닛 정보가 없을 경우 강제종료
        if (unit == null || unitData == null)
        {
            Debug.LogError("AI의 행동기준이 될 유닛의 데이터가 존재하지 않습니다.");
            return;
        }
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
        //사망했을 경우 아무것도 하지 않음
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

        // 평상시엔 Idle
        // 이동시 Move - 완료시 Idle
        // 타겟으로 결정된 경우 Attack
        // 적 미싱 BattleReady - 일정 시간이 지난 후 경계종료 Idle

        // 이벤트 타입
        eUnitActionEvent actionType = eUnitActionEvent.Idle;
        // 외부 이벤트에 맞는 상황 타입으로 변환
        switch (EventType)
        {
            //상황 종료
            case eUnitSituation.SituationClear:
                {
                    switch (unit.uState)
                    {
                        // 공격중 상황이 종료될 경우 대기가 아니라 전투 준비로 상태 변경
                        case eUnitActionEvent.Attack:
                            actionType = eUnitActionEvent.BattleReady;
                            break;

                        // 나머지 타입은 대기로 상태 변경
                        default:
                            actionType = eUnitActionEvent.Idle;
                            break;
                    }
                }
                break;

            //대기 명령
            case eUnitSituation.StandbyCommand:
                {
                    switch (unit.uState)
                    {
                        // 별 일 없으면 대기 상태로 전환
                        case eUnitActionEvent.Idle:
                        case eUnitActionEvent.Move:
                        case eUnitActionEvent.BattleReady:
                            actionType = eUnitActionEvent.Idle;
                            break;

                        // 공격중일 경우 전투준비 상태로 전환
                        case eUnitActionEvent.Attack:
                            actionType = eUnitActionEvent.BattleReady;
                            break;
                    }
                }
                break;

            //이동 명령
            case eUnitSituation.MoveCommand:
                {
                    actionType = eUnitActionEvent.Move;
                }
                break;

            //미확인 물체 조우
            case eUnitSituation.CreatureEncounter:
                {
                    //대기상태거나 이동중일 경우 전투 준비
                    switch (unit.uState)
                    {
                        case eUnitActionEvent.Idle:
                        case eUnitActionEvent.Move:
                            actionType = eUnitActionEvent.BattleReady;
                            break;
                    }
                }
                break;

            // 지점, 대상 공격
            case eUnitSituation.StrikeCommand:
                {
                    actionType = eUnitActionEvent.Attack;
                }
                break;
        }

        //[0 : 주먹],[1 : 권총],[2 : 반자동],[3 : 자동]
        string actionKey = string.Empty;
        string subAnimKey = string.Empty;
        bool isDetailCheck = true;
        System.Action<string[], System.Action> stateAction = null;

        float waitTime = 0f;
        eUnitWaitEvent waitEventType = eUnitWaitEvent.None;

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
                    stateAction = move;
                    actionKey = "Run";
                }
                break;
            case eUnitActionEvent.BattleReady:  // 적 탐색
                {
                    stateAction = battleReady;
                    actionKey = "BattleReady";
                    waitTime = unit.data.reactionSpeed;
                    waitEventType = eUnitWaitEvent.EndObjectEmotion;
                }
                break;
            case eUnitActionEvent.Attack:       // 적 공격
                {
                    stateAction = attack;
                    actionKey = "Attack";
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
                    subAnimKey = "_LongGun";
                    break;
            }
        }

        //2차 분류 및 키 조합
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

    /// <summary> 대기 이벤트(StartWaitEvent 종료시 실행) </summary>
    protected override void WaitEvent()
    {
        base.WaitEvent();

        switch (waitEvent)
        {
            case eUnitWaitEvent.EndObjectEmotion: //미확인 물체 감정 완료
                {
                    //미확인 물체 is 적
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
        //유닛 데이터 세팅
        base.Setting(unit);
    }

    /// <summary> 이벤트 갱신 </summary>
    /// <param name="EventType"> 상호작용 이벤트 </param>
    public override bool Refresh(eUnitSituation EventType)
    {
        //사망했을 경우 아무것도 하지 않음
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

        // 평상시엔 Idle
        // 이동시 Move - 완료시 Idle
        // 타겟으로 결정된 경우 Attack
        // 적 미싱 BattleReady - 일정 시간이 지난 후 경계종료 Idle

        //이벤트 타입
        eUnitActionEvent actionType = eUnitActionEvent.Idle;

        switch (EventType)
        {
            //상황 종료
            case eUnitSituation.SituationClear:
                {
                    switch (unit.uState)
                    {
                        // 공격중 상황이 종료될 경우 대기가 아니라 전투 준비로 상태 변경
                        case eUnitActionEvent.Attack:
                            actionType = eUnitActionEvent.BattleReady;
                            break;

                        // 나머지 타입은 대기로 상태 변경
                        default:
                            actionType = eUnitActionEvent.Idle;
                            break;
                    }
                }
                break;

            //대기 명령
            case eUnitSituation.StandbyCommand:
                {
                    switch (unit.uState)
                    {
                        // 별 일 없으면 대기 상태로 전환
                        case eUnitActionEvent.Idle:
                        case eUnitActionEvent.Move:
                        case eUnitActionEvent.BattleReady:
                            actionType = eUnitActionEvent.Idle;
                            break;

                        // 공격중일 경우 전투준비 상태로 전환
                        case eUnitActionEvent.Attack:
                            actionType = eUnitActionEvent.BattleReady;
                            break;
                    }
                }
                break;

            //이동 명령
            case eUnitSituation.MoveCommand:
                {
                    actionType = eUnitActionEvent.Move;
                }
                break;

            //미확인 물체 조우
            case eUnitSituation.CreatureEncounter:
                {
                    //대기상태거나 이동중일 경우 전투 준비
                    switch (unit.uState)
                    {
                        case eUnitActionEvent.Idle:
                        case eUnitActionEvent.Move:
                            actionType = eUnitActionEvent.BattleReady;
                            break;
                    }
                }
                break;

            // 지점, 대상 공격
            case eUnitSituation.StrikeCommand:
                {
                    actionType = eUnitActionEvent.Attack;
                }
                break;
        }

        //[0 : 주먹],[1 : 권총],[2 : 반자동],[3 : 자동]
        string actionKey = string.Empty;
        System.Action<string[], System.Action> stateAction = null;

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
            case eUnitActionEvent.BattleReady:  // 적 발견
                {
                    actionKey = "Angry";
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
                }
                break;
        }

        //2차 분류 및 키 조합
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