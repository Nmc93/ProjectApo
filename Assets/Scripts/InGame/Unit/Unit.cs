using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

[Serializable]
public class Unit : MonoBehaviour
{
    #region �ν�����

    [Header("Ž�� ����")]
    [SerializeField] private BoxCollider2D searchArea;

    [Header("[���� ��������Ʈ]")]
    [Tooltip("�Ӹ�")]
    [SerializeField] private SpriteRenderer head;
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

    [Header("[���� �ִϸ�����]"),Tooltip("�Ӹ� �ִϸ�����")]
    [SerializeField] private Animator headAnimator;
    [Tooltip("���� �ִϸ��̼�")]
    [SerializeField] private Animator bodyAnimator;

    #endregion �ν�����

    #region ������

    [Header("[���� ������]")]
    /// <summary> ������ UID </summary>
    public int UID;

    /// <summary> �ش� ������ ���� </summary>
    public UnitData data;
    /// <summary> �ش� ������ AI </summary>
    public UnitAI ai;

    /// <summary> ���� ������ �ൿ </summary>
    public eUnitActionEvent uState;

    [Header("[Ÿ��]")]
    /// <summary> ���� ��� �� ID [���� ���� ��� : -1]</summary>
    public int tagetEnemyID = -1;
    /// <summary> ��ġ �����ȿ� �ִ� �� ID ��� </summary>
    public List<int> searchEnemyList = new List<int>();

    [Header("[��ǥ ����]")]
    /// <summary> ������ ����Ʈ </summary>
    public Vector2 targetPoint;
    /// <summary> �ش� ������ ��Ʋ�� Ÿ�� </summary>
    private eAtlasType atlasType;

    /// <summary> �ൿ �ݹ� </summary>
    private Action animCallBack;

    /// <summary> ������ �� ���� ���� </summary>
    public void Init(UnitData data)
    {
        //���� ������ ����
        this.data = data;

        atlasType = data.unitType switch
        {
            eUnitType.Human => eAtlasType.Unit_Human,
            eUnitType.Zombie => eAtlasType.Unit_Zombie,
            _ => eAtlasType.Unit_Human,
        };

        // �Ӹ� ����
        ChangeSprite(head, data.headID);
        //�� ���
        ChangeSprite(faceDeco, data.faceDecoID);
        //�Ӹ�ī�� ����
        ChangeSprite(hair, data.hairID);
        //�޸Ӹ� ����
        ChangeSprite(backHair, data.backHairID);
        //���� ����
        ChangeSprite(hat, data.hatID);

        //���� ����(�Ǽ��� ��� �������� ����)
        if (data.weaponTbl.Path != "None" && atlasType == eAtlasType.Unit_Human)
        {
            weapon.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, data.weaponTbl.Path);
        }

        //�Ӹ� ���� (�ִϸ��̼� ��Ʈ�ѷ�)
        headAnimator.runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(data.headAnimID);
        //��, �� ���� (�ִϸ��̼� ��Ʈ�ѷ�)
        bodyAnimator.runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(data.bodyAnimID);
        
        //���� ��� �� ����
        RefreshStat();

        //AI ����
        SetAI();
    }

    /// <summary> ĳ���� ���� ��� �� ���� </summary>
    private void RefreshStat()
    {
        //���� ���
        data.RefreshStat();

        // Ž�� ���� ����
        searchArea.size = new Vector2(data.f_DetectionRange, 1);
        searchArea.offset = new Vector2(-((float)data.f_DetectionRange / 2), 0);
    }

    #endregion ������

    #region ����Ƽ �������̵�

    #region �̺�Ʈ ���, ����

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

    #endregion �̺�Ʈ ���, ����

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //������ �������� ����
        if(collision.gameObject.layer == 11)
        {
            return;
        }

        // ������ �̸��� UID�� ��ȿ�ϰ� �ش� UID�� ���� ������ �������� ���
        if (int.TryParse(collision.name, out int uID) && UnitMgr.GetUnitType(uID) != data.unitType)
        {
            //�߰ߵ� ����� 
            if (false == searchEnemyList.Contains(uID))
            {
                searchEnemyList.Add(uID);

                //���� Ÿ���� ���� ���
                if(searchEnemyList.Count == 1 && tagetEnemyID == -1)
                {
                    //Ÿ�� ���� �� �̺�Ʈ ����
                    tagetEnemyID = searchEnemyList[0];
                    ai.Refresh(eUnitSituation.CreatureEncounter);
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

        //��� ����� ��Ͽ��� ����
        if (searchEnemyList.Contains(tid))
        {
            searchEnemyList.Remove(tid);
            //tid�� �̿��ؼ� ���� �� Ž���� ���� ������
        }
    }

    #endregion ����Ƽ �������̵�

    #region AI

    /// <summary> Ÿ�Կ� �´� AI�� ���� �� ���� </summary>
    private void SetAI()
    {
        tagetEnemyID = -1;
        searchEnemyList.Clear();

        //�⺻ ���·� ����
        uState = eUnitActionEvent.Idle;
        //Ÿ�Կ� �´� AI ����
        ai = null;
        switch (data.unitType)
        {
            case eUnitType.Human:
                ai = new NormalHumanAI();
                ai.SetStateAction(new Action<string[], Action>[] { Idle, Move, BattleReady, Attack, Die });
                break;
            case eUnitType.Zombie:
                ai = new NomalZombieAI();
                ai.SetStateAction(new Action<string[], Action>[] { Idle, Move, BattleReady, Attack, Die });
                break;
        }

        //ai ����
        ai.Setting(this);
        ai.Refresh(eUnitSituation.StandbyCommand);
    }

    /// <summary> ���� ������Ʈ �Լ� </summary>
    private void UnitUpdate()
    {
        //AI�� ������Ʈ
        if (ai != null)
        {
            ai.Update();
        }
    }

    #region �ൿ

    /// <summary> ��� </summary>
    private void Idle(string[] key, Action callBack)
    {
        uState = eUnitActionEvent.Idle;
        ChangeAnim(key);

        //�ݹ� ����
        animCallBack = callBack;
    }

    /// <summary> �̵� </summary>
    public void Move(string[] key, Action callBack)
    {
        uState = eUnitActionEvent.Move;
        ChangeAnim(key);

        //�ݹ� ó��
        CallBackHandling(callBack);
    }

    /// <summary> �����غ�, ��� </summary>
    public void BattleReady(string[] key, Action callBack)
    {
        uState = eUnitActionEvent.BattleReady;
        ChangeAnim(key);

        //�ݹ� ó��
        CallBackHandling(callBack);
    }

    /// <summary> ���� </summary>
    private void Attack(string[] key, Action callBack)
    {
        uState = eUnitActionEvent.Attack;
        ChangeAnim(key);

        //��� ���� ����
        UnitMgr.instance.AttackUnit(tagetEnemyID, data.f_Damage);

        //�ݹ� ó��
        CallBackHandling(callBack);
    }

    /// <summary> ��� </summary>
    private void Die(string[] key, Action callBack)
    {
        uState = eUnitActionEvent.Die;
        ChangeAnim(key);

        //�ݹ� ó��
        CallBackHandling(callBack);
    }

    private void CallBackHandling(Action callBack)
    {
        switch(ai.timing)
        {
            //��� ����
            case eUnitWaitEventStartTiming.StartAnim:
                {
                    callBack();
                }
                break;
            //�ִϸ��̼��� ����� �� ����
            case eUnitWaitEventStartTiming.EndAnim:
                {
                    animCallBack = callBack;
                }
                break;
        }
    }

    #endregion �ൿ

    /// <summary> �ִϸ��̼� ���� </summary>
    /// <param name="key"> �ִϸ��̼� Ű </param>
    private void ChangeAnim(string[] key)
    {
        //�Ӹ�
        headAnimator.SetTrigger(key[0]);
        //��
        headAnimator.SetTrigger(key[1]);
        //�� + �ٸ�
        bodyAnimator.SetTrigger(key[2]);
        //��
        bodyAnimator.SetTrigger(key[3]);
    }

    #endregion AI

    #region �̹��� ����
    /// <summary> ��������Ʈ�������� ��������Ʈ�� ���� </summary>
    /// <param name="renderer"> ������ ��������Ʈ ������ </param>
    /// <param name="id"> UnitSpriteTableData�� ID ���� </param>
    private void ChangeSprite(SpriteRenderer renderer, int id)
    {
        //���̺��� ���ų� None�� ��� ��Ȱ��ȭ �� ����
        if (!TableMgr.Get(id, out UnitSpriteTableData tbl) || tbl.Path == "None")
        {
            renderer.gameObject.SetActive(false);
            return;
        }

        //�̹��� �� �ִϸ��̼� ����
        renderer.sprite = AssetsMgr.GetSprite(atlasType, tbl.Path);
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