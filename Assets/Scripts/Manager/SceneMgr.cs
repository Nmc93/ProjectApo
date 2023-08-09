using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GEnum;

public class SceneMgr : MgrBase
{
    public static SceneMgr instance;

    #region �̺�Ʈ
    /// <summary> �� ���� ���൵ ���� �̺�Ʈ </summary>
    public Action<float> onGetchanProgress;
    /// <summary> �� ���� ���� �̺�Ʈ </summary>
    public Action<eLoadingState> onGetChanState;
    #endregion �̺�Ʈ

    #region ������Ƽ

    /// <summary> ���� �� </summary>
    public eScene CurScene 
    { 
        get => curScene;
    }

    /// <summary> ���� �� ���� ���� </summary>
    public eLoadingState CurState
    {
        get => curState;
        private set
        {
            curState = value;

            if(onGetChanState != null)
            {
                onGetChanState(value);
            }
        }
    }

    #endregion ������Ƽ

    #region private
    /// <summary> ���� �� </summary>
    private eScene curScene = eScene.LobbyScene;
    /// <summary> ���� �� ���� ���� </summary>
    private eLoadingState curState = eLoadingState.None; 

    /// <summary> �񵿱� �� ���� �ڷ�ƾ </summary>
    private Coroutine changeCoroutine;
    /// <summary> �񵿱� �� ���� ���۷��̼� </summary>
    private AsyncOperation operation;

    /// <summary> �� ���� �غ� �Ϸ�� �ٷ� �������� �ʰ� ����ϴ��� ���� </summary>
    private bool isWaitForNextScene;
    #endregion private

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;

        //���� ���̺��� ���� �ڿ� ���̺� ���� ����(������ ������ ���� ����)
        isWaitForNextScene = OptionMgr.GetBoolOption("IsWaitNextScene", false);
    }

    /// <summary> ������ ������ ���� </summary>
    /// <param name="scene"> ����� �� </param>
    public void ChangeScene(eScene scene)
    {
        //��������� ��������� �̵��ϴ°� �Ұ���
        if (CurScene == scene)
        {
            Debug.LogError("���� ���� ���� �����δ� �̵��� �� �����ϴ�.");
            return;
        }

        //�� ���� �ڷ�ƾ�� ���� ���� ���� ��쿡��
        if (changeCoroutine == null)
        {
            //---------------------------- �� ���� UI ���� ------------------------------
            if(!UIMgr.OpenUI(eUI.UILoading))
            {
                //UIȰ��ȭ�� �����ߴµ� ����� ��� ������ �̵��� �Ұ����ϴ� ���� �̵�
                isWaitForNextScene = false;
            }

            //----------------------------- ���� �� ���� --------------------------------
            CurState = eLoadingState.CloseCurScene;
            // ��� PageUI ����
            UIMgr.SceneChangeAllUIClose();

            //----------------------------- ���� �� �غ� --------------------------------
            //�ε� UI�� ���¸� �� ���������� ����
            CurState = eLoadingState.SceneChange;
            //�񵿱� �� ��ȯ ����
            changeCoroutine = StartCoroutine(OpenScene(scene));
        }
        else
        {
            Debug.LogError($"�� �����߿� ���� ������ �� �����ϴ�.");
        }
    }

    #region �� ����

    /// <summary> �� ����  </summary>
    private IEnumerator OpenScene(eScene sceneType)
    {
        //����� ���� ����Ŵ
        operation = SceneManager.LoadSceneAsync(sceneType.ToString());
        operation.allowSceneActivation = false;

        //----------------------------- �� �ε� ���� --------------------------------
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            //�ε��� 90% ����ȴٸ� ������ ���� ���� �����ϰų� ���
            if (operation.progress >= 0.90f)
            {
                //�ٷ� �������� �ʰ� ����Ѵٸ�
                if (isWaitForNextScene)
                {
                    CurState = eLoadingState.WaitChangeScene;
                }
                //�غ�ȴ�� �ٷ� ���� ������ ���
                else
                {
                    operation.allowSceneActivation = true;
                }
                break;
            }

            // ���� �� ���� ���൵ �ۼ�Ʈ ����
            if (onGetchanProgress != null)
            {
                onGetchanProgress(operation.progress);
            }
        }

        //�� �غ� �ۼ�Ʈ�� 100%�� ����
        if (onGetchanProgress != null)
        {
            onGetchanProgress(1f);
        }

        //----------------------------- �� ���� ��� --------------------------------
        while (!operation.allowSceneActivation)
        {
            yield return new WaitForSeconds(0.5f);
        }

        //------------------------------ �� ���� ------------------------------------
        //���� �� ����, ���µ� �� ����
        curScene = sceneType;
        OpenCurScene();
    }

    /// <summary> �� ���� </summary>
    public void OpenCurScene()
    {
        //�� ���� �ڷ�ƾ�� �������̶�� ����
        if (changeCoroutine != null)
        {
            StopCoroutine(changeCoroutine);
            changeCoroutine = null;
        }

        //�Ϸ��ϰ� ���� ������, ���� �°� ����
        switch (CurScene)
        {
            case eScene.LobbyScene:
                {
                    //�κ� �� ����
                    UIMgr.OpenUI(eUI.UILobby);
                }
                break;
            case eScene.GameScene:
                {

                }
                break;
        }

        //�� ���� ����
        CurState = eLoadingState.None;
    }

    #endregion �� ����

    /// <summary> �� ���� �㰡 </summary>
    public void MoveNextScene()
    {
        //�������̰� ���൵�� ���� �̻� �������� ���
        if(operation != null && operation.progress >= 0.9f)
        {
            //�� ���� �㰡
            operation.allowSceneActivation = true;
        }
    }
}
