using GEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UILoading : UIBase
{
    public override eCanvas canvasType => eCanvas.Page;

    public override eUI uiType => eUI.UILoading;


    #region �ν�����

    [Header("------------------ UI -----------------")]
    [Header("[�ε� �ؽ�Ʈ]"), Tooltip("[�ε� �ؽ�Ʈ]")]
    [SerializeField] private TextMeshProUGUI txt;
    [Header("[�ε� ��ũ�� ��]"), Tooltip("�ε� ��ũ�� ��")]
    [SerializeField] private Scrollbar scrollbar;

    #endregion �ν�����

    /// <summary> �ؽ�Ʈ ����Ʈ </summary>
    private Coroutine textMove;
    /// <summary> UI�� ���� ���� </summary>
    private eLoadingState curState = eLoadingState.None;
    /// <summary> �ؽ�Ʈ�� ����� ���� </summary>
    private string moveString;

    public override void DataSetting()
    {
        //������ �ʱ�ȭ
        curState = eLoadingState.None;
        moveString = string.Empty;
        scrollbar.size = 0;
        //�ؽ�Ʈ ����Ʈ�� Ȱ��ȭ ���¶�� ��Ȱ��ȭ
        if (DOTween.IsTweening(txt))
        {
            txt.DOKill();
        }

        //�ڷ�ƾ ����
        textMove = StartCoroutine(TextMove());

        //�̺�Ʈ ���(���α׷�����, ���� ����, Ŭ�� �̺�Ʈ)
        AddEvent();

        base.DataSetting();
    }

    public override void DataClear()
    {
        //�̺�Ʈ ����(���α׷�����, ���� ����, Ŭ�� �̺�Ʈ)
        RemoveEvent();

        //�ڷ�ƾ ����
        if (textMove != null)
        {
            StopCoroutine(textMove);
            textMove = null;
        }

        base.DataClear();
    }

    #region �̺�Ʈ ���, ����

    /// <summary> �̺�Ʈ ��� �Լ� </summary>
    private void AddEvent()
    {
        SceneMgr.instance.onGetchanProgress += SetProgress;
        SceneMgr.instance.onGetChanState += ChangeState;
        InputMgr.AddKeyEvent(eInputType.MoveNextScene, OnClickNextScene);
    }
    /// <summary> �̺�Ʈ ���� �Լ� </summary>
    private void RemoveEvent()
    {
        //�̺�Ʈ ����(���α׷�����, ���� ����, Ŭ�� �̺�Ʈ)
        SceneMgr.instance.onGetchanProgress -= SetProgress;
        SceneMgr.instance.onGetChanState -= ChangeState;
        InputMgr.RemoveKeyEvent(eInputType.MoveNextScene, OnClickNextScene);
    }

    #endregion �̺�Ʈ ���, ���� 

    #region �̺�Ʈ

    /// <summary> �ε� ���� ���� </summary>
    /// <param name="state"></param>
    public void ChangeState(eLoadingState state)
    {
        curState = state;

        switch (curState)
        {
            // ���� ���� ����
            case eLoadingState.CloseCurScene:
                {
                    moveString = TableMgr.Get<StringTableData>("Loading1").Text;
                    scrollbar.gameObject.SetActive(false);
                }
                break;
            // �� ����
            case eLoadingState.SceneChange:
                {
                    moveString = TableMgr.Get<StringTableData>("Loading2").Text;
                    scrollbar.gameObject.SetActive(true);
                }
                break;
            // �����۾��� �Ϸ��ϰ� ���
            case eLoadingState.WaitChangeScene:
                {
                    //�ڷ�ƾ ����
                    if (textMove != null)
                    {
                        StopCoroutine(textMove);
                        textMove = null;
                    }

                    //�ؽ�Ʈ ���� �� �����̴� ����Ʈ ����
                    moveString = TableMgr.Get<StringTableData>("LoadingCom").Text;
                    txt.text = moveString;
                    txt.DOFade(0.0f, 1).SetLoops(-1,LoopType.Yoyo);
                }
                break;
            //UI ���� ������
            case eLoadingState.None:
                {
                    //�ش� UI ����
                    UIMgr.instance.CloseUI(uiType);
                }
                break;
        }
    }

    /// <summary> ���� �� ���� </summary>
    public void SetProgress(float value)
    {
        scrollbar.size = value;
    }

    /// <summary> ���� ������ �̵� </summary>
    public void OnClickNextScene()
    {
        SceneMgr.instance.MoveNextScene();
    }

    #endregion �̺�Ʈ

    #region �ؽ�Ʈ �̵� �ڷ�ƾ
    /// <summary> �ؽ�Ʈ �̵� �ڷ�ƾ </summary>
    IEnumerator TextMove()
    {
        string middleStr = string.Empty;
        while (scrollbar.size < 1)
        {
            if (string.IsNullOrEmpty(moveString))
            {
                //�ؽ�Ʈ ����Ʈ
                if (middleStr.Length > 6)
                    middleStr = $"{middleStr}.";
                else
                    middleStr = string.Empty;

                txt.text = string.Format("{0}{1}", moveString, middleStr);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
    #endregion �ؽ�Ʈ �̵� �ڷ�ƾ
}