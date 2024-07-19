using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

[Serializable]
public class Unit : MonoBehaviour
{
    #region 인스펙터

    [Header("탐색 범위")]
    [SerializeField] BoxCollider2D searchArea;

    [Header("[유닛 스프라이트]")]
    [Tooltip("머리")]
    [SerializeField] SpriteRenderer head;
    [Tooltip("얼굴 데코")]
    [SerializeField] SpriteRenderer faceDeco;
    [Tooltip("머리카락")]
    [SerializeField] SpriteRenderer hair;
    [Tooltip("뒷머리")]
    [SerializeField] SpriteRenderer backHair;
    [Tooltip("모자")]
    [SerializeField] SpriteRenderer hat;

    [Tooltip("무기")]
    [SerializeField] SpriteRenderer weapon;

    [Header("[유닛 애니메이션]"),Tooltip("머리 애니메이션")]
    [SerializeField] UnitHeadAnimator uHeadAnimator;
    [Tooltip("몸통 애니메이션")]
    [SerializeField] UnitBodyAnimator uBodyAnimator;

    #endregion 인스펙터

    #region 데이터

    [Header("[유닛 데이터]")]
    /// <summary> 유닛의 UID </summary>
    public int UID;

    /// <summary> 해당 유닛의 정보 </summary>
    public UnitData data;
    /// <summary> 해당 유닛의 AI </summary>
    public UnitAI ai;

    /// <summary> 현재 유닛의 행동 </summary>
    public eUnitActionEvent uState;

    [Header("[목표 지점]")]
    /// <summary> 목적지 포인트 </summary>
    public Vector2 targetPoint;
    /// <summary> 해당 유닛의 아틀라스 타입 </summary>
    private eAtlasType atlasType;
    
    /// <summary> 현재 HP </summary>
    public int CurHP
    {
        set
        {
            //현재 HP 세팅
            data.f_CurHp = value;

            //현재 사망 체크
            if(value <= 0)
            {
                ai.SettingWaitEvent(
                        eUnitEventPriority.WaitState,
                        eUnitSituation.HP_Zero);
            }
        }
        get => data.f_CurHp;
    }

    private void Start()
    {
        //공격 이벤트 세팅
        uBodyAnimator.attackEvent = TargetAttackEvnet;
        uBodyAnimator.endAnimEvent = EndAnimEvent;
    }

    /// <summary> 데이터 및 기초 세팅 </summary>
    public void Init(UnitData data)
    {
        if(data == null)
        {
            return;
        }

        //유닛 데이터 세팅
        this.data = data;

        atlasType = data.unitType switch
        {
            eUnitType.Human => eAtlasType.Unit_Human,
            eUnitType.Zombie => eAtlasType.Unit_Zombie,
            _ => eAtlasType.Unit_Human,
        };

        // 머리 세팅
        ChangeSprite(head, data.headID);
        //얼굴 장식
        ChangeSprite(faceDeco, data.faceDecoID);
        //머리카락 세팅
        ChangeSprite(hair, data.hairID);
        //뒷머리 세팅
        ChangeSprite(backHair, data.backHairID);
        //모자 세팅
        ChangeSprite(hat, data.hatID);

        //무기 세팅(맨손일 경우 세팅하지 않음)
        if (data.weaponTbl.Path != "None" && atlasType == eAtlasType.Unit_Human)
        {
            weapon.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, data.weaponTbl.Path);
        }

        //머리 세팅 (애니메이션 컨트롤러)
        uHeadAnimator.SetAnimatior(data.headAnimID);
        //몸, 팔 세팅 (애니메이션 컨트롤러)
        uBodyAnimator.SetAnimatior(data.bodyAnimID);

        //애니메이션 Play
        uHeadAnimator.SetPlay(true);
        uBodyAnimator.SetPlay(true);

        //스탯 계산 및 적용
        RefreshStat();

        //AI 세팅
        SetAI();
    }

    /// <summary> 캐릭터 스탯 계산 및 적용 </summary>
    private void RefreshStat()
    {
        //스탯 계산
        data.RefreshStat();

        // 탐색 범위 적용
        searchArea.size = new Vector2(data.f_DetectionRange, 1);
        searchArea.offset = new Vector2(-((float)data.f_DetectionRange / 2), 0);
    }

    #endregion 데이터

    #region 유니티 오버라이드

    #region 이벤트 등록, 해제

    private void OnEnable()
    {
        //업데이트 등록
        UnitMgr.AddUpdateEvent(UnitUpdate);
    }

    private void OnDisable()
    {
        //업데이트 해제
        UnitMgr.RemoveUpdateEvent(UnitUpdate);
    }

    #endregion 이벤트 등록, 해제

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //레이어 타입 11(센서)은 감지하지 않음
        if(collision.gameObject.layer == 11)
        {
            return;
        }

        // 유닛의 이름은 UID로 유효하고 해당 UID를 가진 유닛이 적대적인 경우
        if (int.TryParse(collision.name, out int uID))
        {
            //발견된 타겟을 체크, 공격 대상일 경우 저장
            ai.AddTarget(uID);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (int.TryParse(collision.name, out int uID))
        {
            ai.RemoveTarget(uID);
        }
    }

    #endregion 유니티 오버라이드

    #region AI

    /// <summary> 타입에 맞는 AI를 생성 및 세팅 </summary>
    private void SetAI()
    {
        //기본 상태로 변경
        uState = eUnitActionEvent.Idle;

        Type t = typeof(NormalHumanAI);

        //타입에 맞는 AI 세팅
        switch (data.unitType)
        {
            case eUnitType.Human:   //인간 AI 생성
                {
                    if(ai != null)
                    {
                        if(ai is NormalHumanAI)
                        {
                            ai.Init(this);
                        }
                        else
                        {
                            ai.Release();
                            ai = new NormalHumanAI(this);
                        }
                    }
                    else
                    {
                        ai = new NormalHumanAI(this);
                    }
                }
                break;
            case eUnitType.Zombie:  //좀비 AI 생성
                {
                    if (ai != null)
                    {
                        if (ai is NomalZombieAI)
                        {
                            ai.Init(this);
                        }
                        else
                        {
                            ai.Release();
                            ai = new NomalZombieAI(this);
                        }
                    }
                    else
                    {
                        ai = new NomalZombieAI(this);
                    }
                }
                break;
        }

        // 대기 내부 이벤트 실행
        ai.SettingWaitEvent(
            eUnitEventPriority.WaitState,
            eUnitSituation.Standby_Command);
    }

    /// <summary> 유닛 업데이트 함수 </summary>
    private void UnitUpdate()
    {
        //AI의 업데이트
        if (ai != null)
        {
            ai.Update();
        }
    }

    /// <summary> 상태 변경 </summary>
    /// <param name="state"> 변경 상태 </param>
    /// <param name="animIDs"> 변경 애니메이션 키 </param>
    public void ChangeState(eUnitActionEvent state, int[] animIDs)
    {
        //상태 변경
        uState = state;

        //머리, 얼굴 애니메이션 변경
        uHeadAnimator.ChangeAnimation(animIDs[0]);
        uHeadAnimator.ChangeAnimation(animIDs[1]);
        //몸 + 다리, 팔 애니메이션 변경
        uBodyAnimator.ChangeAnimation(animIDs[2]);
        uBodyAnimator.ChangeAnimation(animIDs[3]);
    }

    #region 상태 이벤트

    /// <summary> 타겟 공격 실행 이벤트 </summary>
    void TargetAttackEvnet()
    {
        UnitMgr.instance.AttackUnit(ai.tagetEnemyID, data.f_Damage);
    }

    /// <summary> 애니메이션 종료 이벤트 </summary>
    void EndAnimEvent(eUnitActionEvent type)
    {
        eUnitSituation nextSituation;
        float waitTime;
        switch (type)
        {
            case eUnitActionEvent.Attack:
                {
                    nextSituation = eUnitSituation.Standby_Command;
                    waitTime = 0;
                }
                break;
            case eUnitActionEvent.Die:
                {
                    nextSituation = eUnitSituation.Return_Unit;
                    waitTime = 2f;
                    uHeadAnimator.SetPlay(false);
                    uBodyAnimator.SetPlay(false);
                }
                break;
            default:
                {
                    nextSituation = eUnitSituation.Situation_Clear;
                    waitTime = 0;
                    Debug.LogError($"{type} 타입은 대응하지 않습니다.");
                }
                break;
        }

        if(nextSituation != eUnitSituation.None)
        {
            // 대기 내부 이벤트 실행
            ai.SettingWaitEvent(
                ai.CurStatePriority,    // 공격을 실행시켰던 우선순위를 계승
                nextSituation,          // 공격 대기 상태로 변환
                waitTime);              // 이벤트 실행까지의 대기 시간
        }
    }

    #endregion 상태 이벤트

    #endregion AI

    #region 이미지 변경
    /// <summary> 스프라이트랜더러의 스프라이트를 변경 </summary>
    /// <param name="renderer"> 변경할 스프라이트 랜더러 </param>
    /// <param name="id"> UnitSpriteTableData의 ID 참조 </param>
    private void ChangeSprite(SpriteRenderer renderer, int id)
    {
        //테이블이 없거나 None일 경우 비활성화 후 종료
        if (!TableMgr.Get(id, out UnitSpriteTableData tbl) || tbl.Path == "None")
        {
            renderer.gameObject.SetActive(false);
            return;
        }

        //이미지 및 애니메이션 변경
        renderer.sprite = AssetsMgr.GetSprite(atlasType, tbl.Path);
        renderer.gameObject.SetActive(true);
    }

    /// <summary> 무기 변경 </summary>
    /// <param name="weaponID"> 무기의 ID </param>
    private void ChangeWeapon(int weaponID)
    {
        //무기 정보 변경
        data.SetWeaponData(weaponID);

        //이미지 및 애니메이션 변경
        if (data.weaponTbl.Path == "None")
        {
            weapon.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, data.weaponTbl.Path);
            weapon.gameObject.SetActive(true);
        }
        else
        {
            weapon.gameObject.SetActive(false);
        }
    }
    #endregion 이미지 변경
}