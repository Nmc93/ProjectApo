using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    /// <summary> 현재 유닛 [unit.AI하면 this가 호출되는 상호참조의 관계라 조심해서 사용] </summary>
    protected Unit unit;

    /// <summary> 이벤트 예약 </summary>
    protected bool isReservation;
    /// <summary> 지난 시간 </summary>
    protected float curWaitTime;
    /// <summary> 현재 진행중인 일의 우선순위 </summary>
    protected eUnitEventPriority curStatePriority;
    /// <summary> 현재 대기중인 이벤트 </summary>
    protected UnitEventData waitUnitEvent;

    /// <summary> AI 세팅 </summary>
    /// <param name="unit"> 유닛 데이터 </param>
    public UnitAI(Unit unit)
    {
        this.unit = unit;
        curStatePriority = eUnitEventPriority.WaitState;

        //유닛 정보가 없을 경우 강제종료
        if (unit == null)
        {
            Debug.LogError("AI의 행동기준이 될 유닛의 데이터가 존재하지 않습니다.");
            return;
        }
    }

    /// <summary> AI 세팅 </summary>
    /// <param name="unit"> 유닛 데이터 </param>
    public virtual void Init(Unit unit)
    {
        this.unit = unit;
        curStatePriority = eUnitEventPriority.WaitState;

        //유닛 정보가 없을 경우 강제종료
        if (unit == null)
        {
            Debug.LogError("AI의 행동기준이 될 유닛의 데이터가 존재하지 않습니다.");
            return;
        }
    }

    /// <summary> AI 캐싱 해제 </summary>
    public void Release()
    {
        unit = null;

        if(waitUnitEvent != null)
        {
            UnitMgr.UnitEventReturn(waitUnitEvent);
            waitUnitEvent = null;
        }
    }

    /// <summary> 대기 이벤트 세팅 </summary>
    /// <param name="priority"> 이벤트 우선순위 </param>
    /// <param name="evnetType"> 실행할 이벤트 타입 </param>
    /// <param name="waitTime"> 이벤트 시간 </param>
    public virtual void SettingWaitEvent(eUnitEventPriority priority, eUnitSituation evnetType, float waitTime)
    {
        if(evnetType != eUnitSituation.None)
        {
            waitUnitEvent = UnitMgr.GetUnitEvent();
            waitUnitEvent.SetData(priority, evnetType, waitTime);
            isReservation = true;
        }
    }

    /// <summary> 현재 상황에 맞게 상태 갱신 </summary>
    public abstract void Refresh();

    /// <summary> 유닛의 정보를 업데이트 [유닛의 UnitUpdate()에서 처리] </summary>
    public virtual void Update()
    {
        //대기중인 이벤트가 있을 경우
        if (isReservation && waitUnitEvent != null)
        {
            // 대기중일 경우
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

/// <summary> 인간형 보스 고티죠? </summary>
public class NormalHumanAI : UnitAI
{
    public NormalHumanAI(Unit unit) : base(unit) { }

    public override void Init(Unit unit)
    {
        //유닛 데이터 세팅
        base.Init(unit);
    }

    /// <summary> 이벤트 갱신 </summary>
    public override void Refresh()
    {
        // 사망했을 경우 아무것도 하지 않음
        if (unit.uState == eUnitActionEvent.Die)
        {
            return;
        }
        // 대기중인 명령이 없을 경우
        else if(null == waitUnitEvent)
        {
            Debug.LogError($"대기중인 명령이 없습니다.");
            return;
        }
        // 현재 WaitState가 아니고 우선순위에서 밀리는 명령일 경우
        else if (curStatePriority != eUnitEventPriority.WaitState && waitUnitEvent.priority > curStatePriority)
        {
            Debug.LogError($"우선순위가 낮은 명령 :[Cur:{curStatePriority}] [New:{waitUnitEvent.priority}]");
            return;
        }
        
        // 외부 이벤트에 맞는 상황 타입으로 변환
        eUnitActionEvent actionType = eUnitActionEvent.Idle;
        switch (waitUnitEvent.eventType)
        {
            //상황 종료
            case eUnitSituation.Situation_Clear:
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
            case eUnitSituation.Standby_Command:
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
            case eUnitSituation.Move_Command:
                {
                    actionType = eUnitActionEvent.Move;
                }
                break;

            //미확인 물체 조우
            case eUnitSituation.Creature_Encounter:
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
            case eUnitSituation.Strike_Command:
                {
                    actionType = eUnitActionEvent.Attack;
                }
                break;
        }

        //[0 : 주먹],[1 : 권총],[2 : 반자동],[3 : 자동]
        string actionKey = string.Empty;
        string subAnimKey = string.Empty;
        bool isDetailCheck = true;
        Action<string[], Action> stateAction = null;

        // events -> [0 : 대기],[1 : 이동],[2 : 탐색],[3 : 공격],[4 : 사망]

        //1차 분류
        switch (actionType)
        {
            // 대기 상태
            case eUnitActionEvent.Idle:
                {
                    actionKey = "Idle";
                }
                break;

            // 이동
            case eUnitActionEvent.Move:
                {
                    actionKey = "Run";
                }
                break;

            // 적 탐색
            case eUnitActionEvent.BattleReady:
                {
                    actionKey = "BattleReady";
                }
                break;

            // 적 공격
            case eUnitActionEvent.Attack:
                {
                    actionKey = "Attack";
                }
                break;

            // 사망
            case eUnitActionEvent.Die:
                {
                    actionKey = "Die";
                    isDetailCheck = false;
                }
                break;
        }

        //3차 추가 분류
        if (isDetailCheck)
        {
            //착용중인 무기 타입에 따라 세팅
            switch (unit.data.weaponTbl.WeaponType)
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
            //현재 작업의 우선순위 세팅 및 현재 이벤트 반환
            curStatePriority = waitUnitEvent.priority;
            UnitMgr.UnitEventReturn(waitUnitEvent);
            waitUnitEvent = null;

            //유닛의 상태 및 애니메이션 변경
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
        //유닛 데이터 세팅
        base.Init(unit);
    }

    /// <summary> 이벤트 갱신 </summary>
    /// <param name="eventData"> 상호작용 이벤트 </param>
    public override void Refresh()
    {
        //사망했을 경우 아무것도 하지 않음
        if (unit.uState == eUnitActionEvent.Die)
        {
            return;
        }

        //이벤트 타입
        eUnitActionEvent actionType = eUnitActionEvent.Idle;

        switch (waitUnitEvent.eventType)
        {
            //상황 종료
            case eUnitSituation.Situation_Clear:
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
            case eUnitSituation.Standby_Command:
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
            case eUnitSituation.Move_Command:
                {
                    actionType = eUnitActionEvent.Move;
                }
                break;

            //미확인 물체 조우
            case eUnitSituation.Creature_Encounter:
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
            case eUnitSituation.Strike_Command:
                {
                    actionType = eUnitActionEvent.Attack;
                }
                break;
        }

        //[0 : 주먹],[1 : 권총],[2 : 반자동],[3 : 자동]
        string actionKey = string.Empty;

        //1차 분류
        switch (actionType)
        {
            case eUnitActionEvent.Idle:         // 대기 상태
                {
                    actionKey = "Idle";
                }
                break;
            case eUnitActionEvent.Move:         // 이동
                {
                    actionKey = "Run";
                }
                break;
            case eUnitActionEvent.BattleReady:  // 적 발견
                {
                    actionKey = "Angry";
                }
                break;
            case eUnitActionEvent.Attack:       // 적 공격
                {
                    actionKey = "Attack";
                }
                break;
            case eUnitActionEvent.Die:          // 사망
                {
                    actionKey = "Die";
                }
                break;
        }

        //2차 분류 및 키 조합
        if (false == string.IsNullOrEmpty(actionKey))
        {
            // 애니메이션
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