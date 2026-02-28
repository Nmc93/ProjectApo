using System;
using UnityEngine;
using GEnum;
using UnityEngine.U2D.Animation;

[Serializable]
public class Unit : MonoBehaviour
{
    [Header("탐색 범위")]
    [SerializeField] BoxCollider2D searchArea;

    [Header("[머리 이미지]")]
    [SerializeField] SpriteLibrary headLib;
    [SerializeField] SpriteResolver head;
    [SerializeField] SpriteResolver faceDeco;
    [SerializeField] SpriteResolver hair;
    [SerializeField] SpriteResolver backHair;
    [SerializeField] SpriteResolver hat;

    [Header("[몸통 이미지]")]
    [SerializeField] SpriteLibrary bodyLib;
    [SerializeField] SpriteResolver body;
    [SerializeField] SpriteResolver frontArm;
    [SerializeField] SpriteResolver backArm;

    [Tooltip("무기")]
    [SerializeField] SpriteResolver weapon;

    [Header("[유닛 애니메이션]"),Tooltip("머리 애니메이션")]
    [SerializeField] UnitHeadAnimator uHeadAnimator;
    [Tooltip("몸통 애니메이션")]
    [SerializeField] UnitBodyAnimator uBodyAnimator;

    #region 데이터

    [Header("[유닛 데이터]")]
    /// <summary> 유닛의 UID </summary>
    public int UID;

    /// <summary> 해당 유닛의 정보 </summary>
    public UnitData Data;
    /// <summary> 해당 유닛의 AI </summary>
    public UnitAI AI;

    /// <summary> 현재 유닛의 행동 </summary>
    public eUnitActionEvent uState;

    [Header("[목표 지점]")]
    /// <summary> 목적지 포인트 </summary>
    public Vector2 targetPoint;

    private const string HeadAnimKey = "_Head";
    private const string BodyAnimKey = "_Body";

    /// <summary> 현재 HP </summary>
    public int CurHP
    {
        set
        {
            //현재 HP 세팅
            Data.f_CurHp = value;

            //현재 사망 체크
            if(value <= 0)
            {
                AI.SettingWaitEvent(
                        eUnitEventPriority.WaitState,
                        eUnitSituation.HP_Zero);
            }
        }
        get => Data.f_CurHp;
    }

    private void Start()
    {
        //공격 이벤트 세팅
        uBodyAnimator.attackEvent = TargetAttackEvnet;
        uBodyAnimator.endAnimEvent = EndAnimEvent;
        uBodyAnimator.OnPlayBodyAim = SetBodySprite;
        uBodyAnimator.OnPlayArmAim = SetArmSprite;
    }

    /// <summary> 데이터 및 기초 세팅 </summary>
    public void Init(UnitData data)
    {
        if (data == null)
            return;

        //유닛 데이터 세팅
        Data = data;

        var unitTypeStr = data.unitType.ToString();
        // 머리 라이브러리
        SetLib(headLib, $"{unitTypeStr}{HeadAnimKey}_{data.HeadLibID}");
        // 몸통 라이브러리
        SetLib(bodyLib, $"{unitTypeStr}{BodyAnimKey}_{data.BodyLibID}");

        // 머리, 얼굴, 머리카락, 뒷머리, 모자 세팅
        SetSprite(head, data.HeadID);
        SetSprite(faceDeco, data.FaceDecoID);
        SetSprite(hair, data.HairID);
        SetSprite(backHair, data.BackHairID);
        SetSprite(hat, data.HatID);

        // 무기 세팅
        SetWeaponSprite();

        //머리 세팅 (애니메이션 컨트롤러)
        uHeadAnimator.SetAnimatior($"{unitTypeStr}{HeadAnimKey}");

        //몸, 팔 세팅 (애니메이션 컨트롤러)
        uBodyAnimator.SetAnimatior($"{unitTypeStr}{BodyAnimKey}");

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
        Data.RefreshStat();

        // 탐색 범위 적용
        searchArea.size = new Vector2(Data.f_DetectionRange, 1);
        searchArea.offset = new Vector2(-((float)Data.f_DetectionRange / 2), 0);
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
            AI.AddTarget(uID);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (int.TryParse(collision.name, out int uID))
        {
            AI.RemoveTarget(uID);
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
        switch (Data.unitType)
        {
            case eUnitType.Human:   //인간 AI 생성
                {
                    if(AI != null)
                    {
                        if(AI is NormalHumanAI)
                        {
                            AI.Init(this);
                        }
                        else
                        {
                            AI.Release();
                            AI = new NormalHumanAI(this);
                        }
                    }
                    else
                    {
                        AI = new NormalHumanAI(this);
                    }
                }
                break;
            case eUnitType.Zombie:  //좀비 AI 생성
                {
                    if (AI != null)
                    {
                        if (AI is NomalZombieAI)
                        {
                            AI.Init(this);
                        }
                        else
                        {
                            AI.Release();
                            AI = new NomalZombieAI(this);
                        }
                    }
                    else
                    {
                        AI = new NomalZombieAI(this);
                    }
                }
                break;
        }

        // 대기 내부 이벤트 실행
        AI.SettingWaitEvent(
            eUnitEventPriority.WaitState,
            eUnitSituation.Standby_Command);
    }

    /// <summary> 유닛 업데이트 함수 </summary>
    private void UnitUpdate()
    {
        //AI의 업데이트
        if (AI != null)
        {
            AI.Update();
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
        UnitMgr.instance.AttackUnit(AI.tagetEnemyID, Data.f_Damage);
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
            AI.SettingWaitEvent(
                AI.CurStatePriority,    // 공격을 실행시켰던 우선순위를 계승
                nextSituation,          // 공격 대기 상태로 변환
                waitTime);              // 이벤트 실행까지의 대기 시간
        }
    }

    #endregion 상태 이벤트

    #endregion AI

    #region 이미지 변경
    
    /// <summary> 스프라이트 라이브러리 변경 </summary>
    public void SetLib(SpriteLibrary spriteLib, string name)
    {
        spriteLib.spriteLibraryAsset = AssetsMgr.GetSpriteLibraryAsset(name);
    }

    /// <summary> SpriteResolver의 Sprite 변경 </summary>
    private void SetSprite(SpriteResolver resolver, int label)
    {
        //테이블이 없거나 None일 경우 비활성화 후 종료
        //if (TableMgr.Get(id, out UnitSpriteTableData tbl) == false || tbl.Category == "None")
        //{
        //    resolver.gameObject.SetActive(false);
        //    return;
        //}

        //tbl.

        //이미지 및 애니메이션 변경
        resolver.SetCategoryAndLabel(resolver.GetCategory(), label.ToString());
        resolver.ResolveSpriteToSpriteRenderer();
        //resolver.sprite = AssetsMgr.GetSprite(atlasType, tbl.Path);
        resolver.gameObject.SetActive(true);
    }

    private void SetBodySprite(string label)
    {
        body.SetCategoryAndLabel(body.GetCategory(), label);

        if (body.GetLabel() == label)
            body.ResolveSpriteToSpriteRenderer();
    }

    private void SetArmSprite(string label)
    {
        frontArm.SetCategoryAndLabel(frontArm.GetCategory(), label);
        frontArm.ResolveSpriteToSpriteRenderer();

        if (frontArm.GetLabel() == label)
            frontArm.ResolveSpriteToSpriteRenderer();

        backArm.SetCategoryAndLabel(backArm.GetCategory(), label);
        backArm.ResolveSpriteToSpriteRenderer();

        if (backArm.GetLabel() == label)
            backArm.ResolveSpriteToSpriteRenderer();
    }

    private void SetWeaponSprite()
    {
        if (Data == null)
            return;

        //무기 세팅(맨손일 경우 세팅하지 않음)
        if (Data.unitType == eUnitType.Human && Data.weaponTbl.Category != "None")
        {
            weapon.SetCategoryAndLabel(Data.weaponTbl.Category, Data.weaponTbl.Label);
        }
    }

    /// <summary> 무기 변경 </summary>
    /// <param name="weaponID"> 무기의 ID </param>
    private void ChangeWeapon(int weaponID)
    {
        //무기 정보 변경
        Data.SetWeaponData(weaponID);

        //이미지 및 애니메이션 변경
        if (Data.weaponTbl.Category == "None")
        {
            weapon.SetCategoryAndLabel(Data.weaponTbl.Category, Data.weaponTbl.Label);
            weapon.gameObject.SetActive(true);
        }
        else
        {
            weapon.gameObject.SetActive(false);
        }
    }
    #endregion 이미지 변경
}