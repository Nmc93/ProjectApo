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
    [SerializeField] BoxCollider2D searchArea;

    [Header("[���� ��������Ʈ]")]
    [Tooltip("�Ӹ�")]
    [SerializeField] SpriteRenderer head;
    [Tooltip("�� ����")]
    [SerializeField] SpriteRenderer faceDeco;
    [Tooltip("�Ӹ�ī��")]
    [SerializeField] SpriteRenderer hair;
    [Tooltip("�޸Ӹ�")]
    [SerializeField] SpriteRenderer backHair;
    [Tooltip("����")]
    [SerializeField] SpriteRenderer hat;

    [Tooltip("����")]
    [SerializeField] SpriteRenderer weapon;

    [Header("[���� �ִϸ��̼�]"),Tooltip("�Ӹ� �ִϸ��̼�")]
    [SerializeField] UnitHeadAnimator uHeadAnimator;
    [Tooltip("���� �ִϸ��̼�")]
    [SerializeField] UnitBodyAnimator uBodyAnimator;

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
        uHeadAnimator.SetAnimatior(data.headAnimID);
        //��, �� ���� (�ִϸ��̼� ��Ʈ�ѷ�)
        uBodyAnimator.SetAnimatior(data.bodyAnimID);

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

        //ai ����
        ai.Setting(this);

        UnitEventData eventData = UnitMgr.GetUnitEvent();
        eventData.SetData(
            eUnitEventPriority.Situation_Response,
            eUnitSituation.Standby_Command,
            eUnitWaitEventStartTiming.RunImmediately,
            0f);

        ai.Refresh(eventData);
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

    /// <summary> ���� ���� </summary>
    /// <param name="state"> ���� ���� </param>
    /// <param name="key"> ���� �ִϸ��̼� Ű </param>
    public void ChangeState(eUnitActionEvent state, string[] key)
    {
        //���� ����
        uState = state;

        //�Ӹ�, �� �ִϸ��̼� ����
        uHeadAnimator.SetTrigger(key[0]);
        uHeadAnimator.SetTrigger(key[1]);
        //�� + �ٸ�, �� �ִϸ��̼� ����
        uBodyAnimator.SetTrigger(key[2]);
        uBodyAnimator.SetTrigger(key[3]);
    }

    #endregion �ൿ

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

    #endregion �׽�Ʈ �ڵ�
}