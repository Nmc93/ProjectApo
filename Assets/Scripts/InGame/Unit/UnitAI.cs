using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public abstract class UnitAI
{
    public UnitData unitData;
    public Animator animator;

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
        else if(animator = null)
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
    public abstract string Refresh(eUnitActionEvent eventType);
}

/// <summary> 인간형 보스 고티죠? </summary>
public class NormalHumanAI : UnitAI
{
    public override void Setting(UnitData unitData, Animator animator)
    {
        //유닛 데이터 세팅
        base.Setting(unitData, animator);
    }

    public override string Refresh(eUnitActionEvent EventType)
    {
        string actionKey = string.Empty;
        switch(EventType)
        {
            case eUnitActionEvent.NoEvent:
                {
                    actionKey = "";
                }
                break;
            case eUnitActionEvent.EnemySearch:
                {
                    actionKey = "";
                }
                break;
        }

        //[0 : 주먹],[1 : 권총],[2 : 반자동],[3 : 자동]
        if (!string.IsNullOrEmpty(actionKey))
        {
            animator.SetInteger(actionKey,unitData.weaponTbl.WeaponType);
        }

        return actionKey;
    }
}