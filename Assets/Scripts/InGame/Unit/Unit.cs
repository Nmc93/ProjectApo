using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

[System.Serializable]
public class Unit : MonoBehaviour
{
    #region �ν�����

    [Header("[���� �ִϸ��̼�]"), Tooltip("���� �ִϸ��̼�")]
    [SerializeField] private Animator animator;

    [Header("Ž�� ����")]
    [SerializeField] private BoxCollider2D searchArea;

    [Header("[���� ��������Ʈ ����]")]
    [Tooltip("�Ӹ�")]
    [SerializeField] private SpriteRenderer head;
    [Tooltip("��")]
    [SerializeField] private SpriteRenderer face;
    [Tooltip("�� ����")]
    [SerializeField] private SpriteRenderer faceDeco;
    [Tooltip("�Ӹ�ī��")]
    [SerializeField] private SpriteRenderer hair;
    [Tooltip("�޸Ӹ�")]
    [SerializeField] private SpriteRenderer backHair;
    [Tooltip("����")]
    [SerializeField] private SpriteRenderer hat;

    [Tooltip("����")]
    [SerializeField] private SpriteRenderer weapon;

    #endregion �ν�����

    #region ������

    /// <summary> ������ TID </summary>
    public int TID;

    /// <summary> �ش� ������ ���� </summary>
    public UnitData data;
    /// <summary> �ش� ������ AI </summary>
    public UnitAI ai;

    /// <summary> ���� ������ �ൿ </summary>
    public eUnitActionEvent uState;

    [Header("[Ÿ��]")]
    /// <summary> ���� ��� �� ID [���� ���� ��� : -1]</summary>
    public int tagetEnemyID = -1;
    /// <summary> ��ġ �����ȿ� �ִ� �� ID </summary>
    public List<int> searchEnemyID = new List<int>();

    [Header("[��ǥ ����]")]
    /// <summary> ������ ����Ʈ </summary>
    public Vector2 targetPoint;

    /// <summary> ������ �� ���� ���� </summary>
    public void Init(UnitData data)
    {
        //���� ������ ����
        this.data = data;

        // �Ӹ� ����
        ChangeSprite(head, data.headID);
        // ��
        ChangeSprite(face, data.faceID);
        //�� ���
        ChangeSprite(faceDeco, data.faceDecoID);
        //�Ӹ�ī�� ����
        ChangeSprite(hair, data.hairID);
        //�޸Ӹ� ����
        ChangeSprite(backHair, data.backHairID);
        //���� ����
        ChangeSprite(hat, data.hatID);

        //���� ����(�Ǽ��� ��� �������� ����)
        if (data.weaponTbl.Path != "None")
        {
            weapon.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, data.weaponTbl.Path);
        }

        //���� ��� �� ����
        RefreshStat();

        //��, �� ���� (�ִϸ��̼� ��Ʈ�ѷ�)
        animator.runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(data.bodyID);

        //AI ����
        SetAI();
    }

    /// <summary> ĳ���� ���� ��� �� ���� </summary>
    private void RefreshStat()
    {
        //���� ���
        data.RefreshStat();

        // Ž�� ���� ����
        searchArea.size = new Vector2(data.sSize, 1);
        searchArea.offset = new Vector2(-((float)data.sSize / 2), 0);
    }

    #endregion ������

    #region ����Ƽ �������̵�

    private void OnEnable()
    {
        //������Ʈ ���
        UnitMgr.AddUpdateEvent(UnitUpdate);
    }

    private void OnDisable()
    {
        //������Ʈ ����
        UnitMgr.RemoveUpdateEvent(UnitUpdate);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TID�� �̸����� ������ ���� �ݶ��̴��� ������ �ƴ�
        if (!int.TryParse(collision.name, out int tid))
        {
            return;
        }
        //���� Ÿ���� ������ ��� �ø��� ����
        else if (UnitMgr.GetUnitType(tid) == data.unitType)
        {
            return;
        }

        //���� Ÿ���� �ƴ� �߰ߵ� ����� ��Ͽ� �߰�
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

        //��� ����� ��Ͽ��� ����
        if (searchEnemyID.Contains(tid))
        {
            searchEnemyID.Remove(tid);
            //tid�� �̿��ؼ� ���� �� Ž���� ���� ������
        }
    }

    private void Update()
    {
        
    }

    #endregion ����Ƽ �������̵�

    #region AI

    /// <summary> Ÿ�Կ� �´� AI�� ���� �� ���� </summary>
    private void SetAI()
    {
        tagetEnemyID = -1;
        searchEnemyID.Clear();

        //�⺻ ���·� ����
        uState = eUnitActionEvent.Idle;
        //Ÿ�Կ� �´� AI ����
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

        //ai ����
        ai.Setting(this);
        ai.Refresh(eUnitSituation.StandbyCommand);
    }

    /// <summary> ���� ������Ʈ �Լ� </summary>
    private void UnitUpdate()
    {
        //���� ����� ����µ� ������ ����� ���� ��� Ÿ�� ����
        if (tagetEnemyID == -1 && searchEnemyID.Count > 0)
        {
            //Ÿ�� ���� �� �̺�Ʈ ����
            tagetEnemyID = searchEnemyID[0];
            searchEnemyID.RemoveAt(0);
            ai.Refresh(eUnitSituation.CreatureEncounter);
        }

        //AI�� ������Ʈ
        if (ai != null)
        {
            ai.Update();
        }
    }

    #region �ൿ

    /// <summary> �̺�Ʈ ���� </summary>
    public void SetSituation(eUnitSituation eSituationType)
    {
        ai.Refresh(eSituationType);
    }

    /// <summary> ��� </summary>
    private void Idle(string key)
    {
        uState = eUnitActionEvent.Idle;
        ChangeAnim(key);
    }

    /// <summary> �̵� </summary>
    public void Move(string key)
    {
        uState = eUnitActionEvent.Move;
        ChangeAnim(key);
    }

    /// <summary> �����غ�, ��� </summary>
    public void BattleReady(string key)
    {
        uState = eUnitActionEvent.BattleReady;
        ChangeAnim(key);
    }

    /// <summary> ���� </summary>
    private void Attack(string key)
    {
        uState = eUnitActionEvent.Attack;
        ChangeAnim(key);
    }

    /// <summary> ��� </summary>
    private void Die(string key)
    {
        uState = eUnitActionEvent.Die;
        ChangeAnim(key);
    }


    #endregion �ൿ

    /// <summary> �ִϸ��̼� ���� </summary>
    /// <param name="key"> �ִϸ��̼� Ű </param>
    private void ChangeAnim(string key)
    {
        animator.SetTrigger(key);
    }

    #endregion AI

    #region �̹��� ����
    /// <summary> ��������Ʈ�������� ��������Ʈ�� ���� </summary>
    /// <param name="renderer"> ������ ��������Ʈ ������ </param>
    /// <param name="id"> UnitAppearanceTableData�� ID ���� </param>
    private void ChangeSprite(SpriteRenderer renderer, int id)
    {
        //���̺��� ���ų� None�� ��� ��Ȱ��ȭ �� ����
        if (!TableMgr.Get(id, out UnitAppearanceTableData tbl) || tbl.Path == "None")
        {
            renderer.gameObject.SetActive(false);
            return;
        }

        //�̹��� �� �ִϸ��̼� ����
        renderer.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        renderer.gameObject.SetActive(true);
    }

    /// <summary> ���� ���� </summary>
    /// <param name="weaponID"> ������ ID </param>
    private void ChangeWeapon(int weaponID)
    {
        //���� ���� ����
        data.SetWeaponData(weaponID);

        //�̹��� �� �ִϸ��̼� ����
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
    #endregion �̹��� ����

    #region �׽�Ʈ �ڵ�

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

    #endregion �׽�Ʈ �ڵ�
}