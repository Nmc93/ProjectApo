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

    [Header("[타겟]")]
    /// <summary> 공격 대상 적 ID [적이 없을 경우 : -1]</summary>
    public int tagetEnemyID = -1;
    /// <summary> 서치 범위안에 있는 적 ID 목록 </summary>
    public List<int> searchEnemyList = new List<int>();

    [Header("[목표 지점]")]
    /// <summary> 목적지 포인트 </summary>
    public Vector2 targetPoint;
    /// <summary> 해당 유닛의 아틀라스 타입 </summary>
    private eAtlasType atlasType;

    /// <summary> 행동 콜백 </summary>
    private Action animCallBack;

    /// <summary> 데이터 및 기초 세팅 </summary>
    public void Init(UnitData data)
    {
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
        //센서는 감지하지 않음
        if(collision.gameObject.layer == 11)
        {
            return;
        }

        // 유닛의 이름은 UID로 유효하고 해당 UID를 가진 유닛이 적대적인 경우
        if (int.TryParse(collision.name, out int uID) && UnitMgr.GetUnitType(uID) != data.unitType)
        {
            //발견된 대상이 
            if (false == searchEnemyList.Contains(uID))
            {
                searchEnemyList.Add(uID);

                //현재 타겟이 없을 경우
                if(searchEnemyList.Count == 1 && tagetEnemyID == -1)
                {
                    //타겟 지정 및 이벤트 세팅
                    tagetEnemyID = searchEnemyList[0];

                    UnitEventData data = UnitMgr.GetUnitEvent();
                    data.SetData(
                        eUnitEventPriority.Situation_Response,
                        eUnitSituation.Creature_Encounter,
                        eUnitWaitEventStartTiming.RunImmediately,
                        0f);

                    ai.Refresh(data);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!int.TryParse(collision.name, out int tid))
        {
            return;
        }

        //벗어난 대상을 목록에서 제거
        if (searchEnemyList.Contains(tid))
        {
            searchEnemyList.Remove(tid);
            //tid를 이용해서 추적 및 탐색을 할지 선택함
        }
    }

    #endregion 유니티 오버라이드

    #region AI

    /// <summary> 타입에 맞는 AI를 생성 및 세팅 </summary>
    private void SetAI()
    {
        tagetEnemyID = -1;
        searchEnemyList.Clear();

        //기본 상태로 변경
        uState = eUnitActionEvent.Idle;
        //타입에 맞는 AI 세팅
        ai = null;

        switch (data.unitType)
        {
            case eUnitType.Human:
                {
                    ai = new NormalHumanAI();
                }
                break;
            case eUnitType.Zombie:
                {
                    ai = new NomalZombieAI();
                }
                break;
        }

        //ai 세팅
        ai.Setting(this);

        UnitEventData eventData = UnitMgr.GetUnitEvent();
        eventData.SetData(
            eUnitEventPriority.Situation_Response,
            eUnitSituation.Standby_Command,
            eUnitWaitEventStartTiming.RunImmediately,
            0f);

        ai.Refresh(eventData);
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

    #region 행동

    /// <summary> 상태 변경 </summary>
    /// <param name="state"> 변경 상태 </param>
    /// <param name="key"> 변경 애니메이션 키 </param>
    public void ChangeState(eUnitActionEvent state, string[] key)
    {
        //상태 변경
        uState = state;

        //머리, 얼굴 애니메이션 변경
        uHeadAnimator.SetTrigger(key[0]);
        uHeadAnimator.SetTrigger(key[1]);
        //몸 + 다리, 팔 애니메이션 변경
        uBodyAnimator.SetTrigger(key[2]);
        uBodyAnimator.SetTrigger(key[3]);
    }

    #endregion 행동

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

    #region 테스트 코드

    //public void testInit()
    //{
    //    Init(UnitMgr.CreateUnitDate(0, 1));
    //}
    //
    //public void testIdle()
    //{
    //    ai.Refresh(eUnitSituation.StandbyCommand);
    //}
    //public void testMove()
    //{
    //    ai.Refresh(eUnitSituation.MoveCommand);
    //}
    //public void testBattleReady()
    //{
    //    ai.Refresh(eUnitSituation.CreatureEncounter);
    //}
    //public void testAttack()
    //{
    //    ai.Refresh(eUnitSituation.StrikeCommand);
    //}

    #endregion 테스트 코드
}