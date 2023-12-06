using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

[System.Serializable]
public class Unit : MonoBehaviour
{
    #region 인스펙터

    [Header("[유닛 애니메이션]"), Tooltip("유닛 애니메이션")]
    [SerializeField] private Animator animator;

    [Header("탐색 범위")]
    [SerializeField] private BoxCollider2D searchArea;

    [Header("[유닛 스프라이트 정보]")]
    [Tooltip("머리")]
    [SerializeField] private SpriteRenderer head;
    [Tooltip("얼굴")]
    [SerializeField] private SpriteRenderer face;
    [Tooltip("얼굴 데코")]
    [SerializeField] private SpriteRenderer faceDeco;
    [Tooltip("머리카락")]
    [SerializeField] private SpriteRenderer hair;
    [Tooltip("뒷머리")]
    [SerializeField] private SpriteRenderer backHair;
    [Tooltip("모자")]
    [SerializeField] private SpriteRenderer hat;

    [Tooltip("무기")]
    [SerializeField] private SpriteRenderer weapon;

    #endregion 인스펙터

    #region 데이터

    /// <summary> 유닛의 TID </summary>
    public int TID;

    /// <summary> 해당 유닛의 정보 </summary>
    public UnitData data;
    /// <summary> 해당 유닛의 AI </summary>
    public UnitAI ai;

    /// <summary> 현재 유닛의 행동 </summary>
    public eUnitActionEvent uState;

    [Header("[타겟]")]
    /// <summary> 공격 대상 적 ID [적이 없을 경우 : -1]</summary>
    public int tagetEnemyID = -1;
    /// <summary> 서치 범위안에 있는 적 ID </summary>
    public List<int> searchEnemyID = new List<int>();

    [Header("[목표 지점]")]
    /// <summary> 목적지 포인트 </summary>
    public Vector2 targetPoint;

    /// <summary> 데이터 및 기초 세팅 </summary>
    public void Init(UnitData data)
    {
        //유닛 데이터 세팅
        this.data = data;

        // 머리 세팅
        ChangeSprite(head, data.headID);
        // 얼굴
        ChangeSprite(face, data.faceID);
        //얼굴 장식
        ChangeSprite(faceDeco, data.faceDecoID);
        //머리카락 세팅
        ChangeSprite(hair, data.hairID);
        //뒷머리 세팅
        ChangeSprite(backHair, data.backHairID);
        //모자 세팅
        ChangeSprite(hat, data.hatID);

        //무기 세팅(맨손일 경우 세팅하지 않음)
        if (data.weaponTbl.Path != "None")
        {
            weapon.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, data.weaponTbl.Path);
        }

        //스탯 계산 및 적용
        RefreshStat();

        //몸, 팔 세팅 (애니메이션 컨트롤러)
        animator.runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(data.bodyID);

        //AI 세팅
        SetAI();
    }

    /// <summary> 캐릭터 스탯 계산 및 적용 </summary>
    private void RefreshStat()
    {
        //스탯 계산
        data.RefreshStat();

        // 탐색 범위 적용
        searchArea.size = new Vector2(data.sSize, 1);
        searchArea.offset = new Vector2(-((float)data.sSize / 2), 0);
    }

    #endregion 데이터

    #region 유니티 오버라이드

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TID를 이름으로 가지지 않은 콜라이더는 유닛이 아님
        if (!int.TryParse(collision.name, out int tid))
        {
            return;
        }
        //같은 타입의 유닛은 대상에 올리지 않음
        else if (UnitMgr.GetUnitType(tid) == data.unitType)
        {
            return;
        }

        //현재 타겟이 아닌 발견된 대상을 목록에 추가
        if (tagetEnemyID != tid && !searchEnemyID.Contains(tid))
        {
            searchEnemyID.Add(tid);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!int.TryParse(collision.name, out int tid))
        {
            return;
        }

        //벗어난 대상을 목록에서 제거
        if (searchEnemyID.Contains(tid))
        {
            searchEnemyID.Remove(tid);
            //tid를 이용해서 추적 및 탐색을 할지 선택함
        }
    }

    private void Update()
    {
        
    }

    #endregion 유니티 오버라이드

    #region AI

    /// <summary> 타입에 맞는 AI를 생성 및 세팅 </summary>
    private void SetAI()
    {
        tagetEnemyID = -1;
        searchEnemyID.Clear();

        //기본 상태로 변경
        uState = eUnitActionEvent.Idle;
        //타입에 맞는 AI 세팅
        ai = null;
        switch (data.unitType)
        {
            case eUnitType.Human:
                ai = new NormalHumanAI();
                ai.SetStateAction(Idle, Move, BattleReady, Attack, Die);
                break;
            case eUnitType.Zombie:
                //ai = 
                break;
        }

        //ai 세팅
        ai.Setting(this);
        ai.Refresh(eUnitSituation.StandbyCommand);
    }

    /// <summary> 유닛 업데이트 함수 </summary>
    private void UnitUpdate()
    {
        //공격 대상이 비었는데 감지된 대상이 있을 경우 타겟 지정
        if (tagetEnemyID == -1 && searchEnemyID.Count > 0)
        {
            //타겟 지정 및 이벤트 세팅
            tagetEnemyID = searchEnemyID[0];
            searchEnemyID.RemoveAt(0);
            ai.Refresh(eUnitSituation.CreatureEncounter);
        }

        //AI의 업데이트
        if (ai != null)
        {
            ai.Update();
        }
    }

    #region 행동

    /// <summary> 이벤트 조우 </summary>
    public void SetSituation(eUnitSituation eSituationType)
    {
        ai.Refresh(eSituationType);
    }

    /// <summary> 대기 </summary>
    private void Idle(string key)
    {
        uState = eUnitActionEvent.Idle;
        ChangeAnim(key);
    }

    /// <summary> 이동 </summary>
    public void Move(string key)
    {
        uState = eUnitActionEvent.Move;
        ChangeAnim(key);
    }

    /// <summary> 전투준비, 경계 </summary>
    public void BattleReady(string key)
    {
        uState = eUnitActionEvent.BattleReady;
        ChangeAnim(key);
    }

    /// <summary> 공격 </summary>
    private void Attack(string key)
    {
        uState = eUnitActionEvent.Attack;
        ChangeAnim(key);
    }

    /// <summary> 사망 </summary>
    private void Die(string key)
    {
        uState = eUnitActionEvent.Die;
        ChangeAnim(key);
    }


    #endregion 행동

    /// <summary> 애니메이션 변경 </summary>
    /// <param name="key"> 애니메이션 키 </param>
    private void ChangeAnim(string key)
    {
        animator.SetTrigger(key);
    }

    #endregion AI

    #region 이미지 변경
    /// <summary> 스프라이트랜더러의 스프라이트를 변경 </summary>
    /// <param name="renderer"> 변경할 스프라이트 랜더러 </param>
    /// <param name="id"> UnitAppearanceTableData의 ID 참조 </param>
    private void ChangeSprite(SpriteRenderer renderer, int id)
    {
        //테이블이 없거나 None일 경우 비활성화 후 종료
        if (!TableMgr.Get(id, out UnitAppearanceTableData tbl) || tbl.Path == "None")
        {
            renderer.gameObject.SetActive(false);
            return;
        }

        //이미지 및 애니메이션 변경
        renderer.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
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

    public void testInit()
    {
        Init(UnitMgr.CreateUnitDate(0, 1));
    }

    public void testIdle()
    {
        ai.Refresh(eUnitSituation.StandbyCommand);
    }
    public void testMove()
    {
        ai.Refresh(eUnitSituation.MoveCommand);
    }
    public void testBattleReady()
    {
        ai.Refresh(eUnitSituation.CreatureEncounter);
    }
    public void testAttack()
    {
        ai.Refresh(eUnitSituation.StrikeCommand);
    }

    #endregion 테스트 코드
}