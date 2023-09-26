using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    public UnitData unitData;
    public Animator animator;

    #region 행동 액션
    /// <summary> 대기 </summary>
    public System.Action idle;
    /// <summary> 이동 </summary>
    public System.Action move;
    /// <summary> 공격 </summary>
    public System.Action attack;
    /// <summary> 사망 </summary>
    public System.Action die;
    #endregion 행동 액션

    #region 대기 이벤트
    /// <summary> 대기 이벤트 On,Off 여부 </summary>
    protected bool isOnWaitEvent = false;
    /// <summary> 대기 시간 </summary>
    protected float waitTime;
    /// <summary> 지난 시간 </summary>
    protected float curWaitTime;

    public virtual void StartWaitEvent(float waitTime)
    {
        this.waitTime = waitTime;
        isOnWaitEvent = true;
    }
    /// <summary> 대기 이벤트 </summary>
    protected virtual void WaitEvent() {}
    #endregion 대기 이벤트

    /// <summary> 행동 관련 함수 세팅 </summary>
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

    /// <summary> AI 정보 세팅 </summary>
    /// <param name="unitData"> 유닛 데이터 </param>
    /// <param name="animator"> 유닛의 애니메이터 </param>
    public virtual void Setting(UnitData unitData, Animator animator)
    {
        //유닛 정보가 없을 경우 강제종료
        if (unitData == null)
        {
            Debug.LogError("AI의 행동기준이 될 유닛의 데이터가 존재하지 않습니다.");
            return;
        }
        //애니메이터가 없을 경우에도 강제 종료
        else if(animator == null)
        {
            Debug.LogError("유닛에 애니메이터가 없습니다.");
            return;
        }

        this.unitData = unitData;
        this.animator = animator;
    }

    /// <summary> 현재 상황에 맞게 상태 갱신 </summary>
    /// <param name="eventType"> 유닛의 월드와 한 상호작용 타입 </param>
    /// <returns> 흠... </returns>
    public abstract bool Refresh(eUnitActionEvent eventType);

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
    public override void Setting(UnitData unitData, Animator animator)
    {
        //유닛 데이터 세팅
        base.Setting(unitData, animator);
    }

    /// <summary> 이벤트 갱신 </summary>
    /// <param name="EventType"> 상호작용 이벤트 </param>
    public override bool Refresh(eUnitActionEvent EventType)
    {
        //. 평상시엔 Idle
        //. 이동시 Move - 완료시 Idle
        //. 타겟으로 결정된 경우 Attack
        //. 적 미싱 BattleReady - 일정 시간이 지난 후 경계종료 Idle

        //[0 : 주먹],[1 : 권총],[2 : 반자동],[3 : 자동]
        string actionKey = string.Empty;
        bool isDetailCheck = true;
        switch(EventType)
        {
            case eUnitActionEvent.Idle:         // 대기 상태
                {
                    actionKey = "Idle";
                    idle();
                }
                break;
            case eUnitActionEvent.Move:         // 이동
                {
                    actionKey = "Run";
                    move();
                }
                break;
            case eUnitActionEvent.EnemySearch:  // 적 탐색
                {
                    actionKey = "BattleReady";
                    idle();
                }
                break;
            case eUnitActionEvent.EnemyAttack:  // 적 공격
                {
                    actionKey = "Attack";
                    attack();
                }
                break;
            case eUnitActionEvent.Die:          // 사망
                {
                    actionKey = "Die";
                    isDetailCheck = false;
                    die();
                }
                break;
        }

        //상세 세팅이 필요할 경우
        if (isDetailCheck)
        {
            //착용중인 무기 타입에 따라 세팅
            switch (unitData.weaponTbl.WeaponType)
            {
                case 0: // 맨손
                    actionKey = string.Format("{0}_NoWeapon", actionKey);
                    break;
                case 1: // 권총
                    actionKey = string.Format("{0}_Pistal", actionKey);
                    break;
                case 2: // 반자동
                case 3: // 연사총
                    actionKey = string.Format("{0}_NoWeapon", actionKey);
                    break;
            }
        }

        //유의미한 키가 있을 경우에 애니메이션 변경
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