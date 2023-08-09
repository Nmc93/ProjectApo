using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using GEnum;

public class UIMgr : MgrBase
{
    public static UIMgr instance;

    /// <summary> �� UI ĵ���� </summary>
    private static CanvasData scene;
    /// <summary> ������ UI ĵ���� </summary>
    private static CanvasData page;
    /// <summary> �˾� UI ĵ���� </summary>
    private static CanvasData popup;

    /// <summary> ��Ȱ��ȭ�� UI�� �����ϴ� Ǯ </summary>
    private static RectTransform uiPool;

    /// <summary> UI ����� </summary>
    private static Dictionary<eUI, UIData> dicUI = new Dictionary<eUI, UIData>();
    /// <summary> ���� �����ִ� UI ����Ʈ </summary>
    private static List<eUI> openList = new List<eUI>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;

        //ĵ���� ����
        CanvasSetting();

        //UI Ǯ, ������ ����
        UIDataSetting();
    }

    #region Setting

    /// <summary> ĵ������ �����ϰ� ���� </summary>
    private void CanvasSetting()
    {
        //�̺�Ʈ �ý��� ����
        gameObject.AddComponent<EventSystem>();
        gameObject.AddComponent<StandaloneInputModule>();

        //ĵ���� ����
        GameObject canvasParent = new GameObject();
        canvasParent.transform.SetParent(transform);
        canvasParent.name = "UICanvas";

        #region �� ĵ����
        //�� ĵ���� ����
        Canvas sceneCanvas = new GameObject().AddComponent<Canvas>();
        sceneCanvas.transform.SetParent(canvasParent.transform);
        sceneCanvas.name = "sceneCanvas";
        sceneCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        sceneCanvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
            AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
        //�� ĵ���� �����Ϸ� ����
        CanvasScaler sceneScale = sceneCanvas.gameObject.AddComponent<CanvasScaler>();
        sceneScale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sceneScale.referenceResolution = new Vector2(Screen.width, Screen.height);
        sceneScale.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        //ĵ���� ������ ����
        scene = new CanvasData(sceneCanvas, sceneScale, sceneCanvas.gameObject.AddComponent<GraphicRaycaster>());

        #endregion �� ĵ����

        #region ������ ĵ����
        //������ ĵ���� ����
        Canvas pageCanvas = new GameObject().AddComponent<Canvas>();
        pageCanvas.transform.SetParent(canvasParent.transform);
        pageCanvas.name = "PageCanvas";
        pageCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        pageCanvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | 
            AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
        //������ ĵ���� �����Ϸ� ����
        CanvasScaler pageScale = pageCanvas.gameObject.AddComponent<CanvasScaler>();
        pageScale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        pageScale.referenceResolution = new Vector2(Screen.width,Screen.height);
        pageScale.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        //ĵ���� ������ ����
        page = new CanvasData(pageCanvas, pageScale, pageCanvas.gameObject.AddComponent<GraphicRaycaster>());
        #endregion ������ ĵ����

        #region �˾� ĵ����
        //�˾� ĵ���� ����
        Canvas popupCanvas = new GameObject().AddComponent<Canvas>();
        popupCanvas.transform.SetParent(canvasParent.transform);
        popupCanvas.name = "PopupCanvas";
        popupCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        popupCanvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | 
            AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
        //�˾� ĵ���� �����Ϸ� ����
        CanvasScaler popupScale = popupCanvas.gameObject.AddComponent<CanvasScaler>();
        popupScale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        popupScale.referenceResolution = new Vector2(Screen.width, Screen.height);
        popupScale.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        //�˾� ĵ���� ����ĳ���� ����
        popup = new CanvasData(popupCanvas, popupScale, popupCanvas.gameObject.AddComponent<GraphicRaycaster>());
        #endregion �˾� ĵ����

        #region Ǯ ������Ʈ

        // Ǯ ������Ʈ ����
        RectTransform uiPool = new GameObject().AddComponent<RectTransform>();
        uiPool.transform.SetParent(canvasParent.transform);
        uiPool.sizeDelta = new Vector2(Screen.width, Screen.height);
        uiPool.position = new Vector2(Screen.width / 2, Screen.height / 2);
        uiPool.gameObject.SetActive(false);
        uiPool.name = "UIPool";
        UIMgr.uiPool = uiPool;

        #endregion Ǯ ������Ʈ

    }

    /// <summary> UI�� ������Ʈ Ǯ�� UI �����͸� ���� </summary>
    private void UIDataSetting()
    {
        //�ε� ȭ�� UI
        dicUI.Add(eUI.UILoading, new UIData("UI/UILoading"));

        //�κ� �� ���� ȭ�� UI
        dicUI.Add(eUI.UILobby, new UIData("UI/UILobby"));
    }

    #endregion Setting

    #region Open

    /// <summary> UI ���� </summary>
    /// <typeparam name="T">UIBase�� ��ӹ��� UI�� ���� ������Ʈ Ÿ��</typeparam>
    /// <returns> UI ���¿� �����ϸ� true </returns>
    public static bool OpenUI<T>() where T : UIBase
    {
        //eUI�� UI�� ��ǥ ������Ʈ�� �̸��� �����ؾ���
        return OpenUI((eUI)Enum.Parse(typeof(eUI), typeof(T).Name));
    }

    /// <summary> UI ���� </summary>
    /// <param name="ui"> UI�� ������ enum </param>
    /// <returns> UI ���¿� �����ϸ� true </returns>
    public static bool OpenUI(eUI ui)
    {
        // 0. �̹� �����ִ� ��� ������
        if(openList.Contains(ui))
        {
            Debug.LogError($"{ui}�� �̹� �����ִ� UI �Դϴ�.");
            return false;
        }

        //�ش� UI�� �����͸� Ȯ��
        if(dicUI.TryGetValue(ui, out UIData data))
        {
            UIBase uiBase = data.uiClass;
            
            //1. �ѹ��� ������� ���� UI�� ��� �ε��ؼ� ������ �� ������ ������
            if (uiBase == null)
            {
                // 1-1-1. UI �ε忡 �������� ��� ������ ����
                if(AssetsMgr.LoadResourcesUIPrefab(data.path,out GameObject obj))
                {
                    data.uiClass = Instantiate(obj, uiPool).GetComponent<UIBase>();
                    uiBase = data.uiClass;
                }
                // 1-1-2. UI�� ã�� �� ���� ��� ���� ����
                else
                {
                    Debug.LogError($"UIOpenFailed : [{ui}]�� ��ϵ��� ���� UI�Դϴ�.");
                    return false;
                }
            }

            //2. Page Ÿ���� UI�� ������ ��� Scene ĵ������ ��Ȱ��ȭ �ϰ� �ٸ� Page�� PopupŸ���� UI�� ������
            if (uiBase.canvasType == eCanvas.Page)
            {
                //2-1. �� ĵ���� ��Ȱ��ȭ
                scene.SetActivate(false);

                //2-2. ���� �����ִ� UI�� üũ
                for (int i = openList.Count - 1; i >= 0; -- i)
                {
                    //2-3. ���� UI�� �ƴ� Scene Ÿ�� UI�� ���� ���� ����
                    UIBase temp = dicUI[openList[i]].uiClass;
                    if (temp.uiType != data.uiClass.uiType && temp.canvasType != eCanvas.Scene)
                    {
                        instance.CloseUI(temp.uiType, false);
                    }
                }
            }

            //3. UI�� ĵ������ �ø��� UI�� Ȱ��ȭ
            uiBase.transform.SetParent(GetCanvas(uiBase.canvasType).transform);
            openList.Add(ui);
            uiBase.Open();

            Debug.Log($"UIOpen : [{ui}], CanvasType : [{data.uiClass.canvasType}]");
            return true;
        }

        //���� ����
        Debug.LogError($"UIOpenFailed : [{ui}]�� ��ϵ��� ���� UI�Դϴ�.");
        return false;
    }

    #endregion Open

    #region Close

    /// <summary> �� �������� ���� ���� �����ִ� ��� UI ���� </summary>
    public static void SceneChangeAllUIClose()
    {
        // 1. ���� �����ִ� ��� UI�� üũ
        for (int i = openList.Count - 1; i >= 0; --i)
        {
            // 2. ���� ��� ������ ȹ��
            eUI type = openList[i];
            UIBase uIBase = dicUI[type].uiClass;

            // 3. �� ����� �����ϱ�� �� UI���� üũ
            if (uIBase.IsSceneChangeClose)
            {
                // 4. UI ����
                uIBase.DataClear();
                openList.Remove(type);
            }
        }
    }

    /// <summary> UI ���� </summary>
    /// <typeparam name="T"> UIBase�� ��ӹ��� UI�� ���� ������Ʈ Ÿ�� </typeparam>
    /// <returns> ���ῡ �����ϸ� true </returns>
    public bool CloseUI<T>()
    {
        //eUI�� UI�� ��ǥ ������Ʈ�� �̸��� �����ؾ���
        return CloseUI((eUI)Enum.Parse(typeof(eUI), typeof(T).Name));
    }

    /// <summary> UI ���� </summary>
    /// <param name="ui"> ������ UI </param>
    /// <param name="isChainClose"> UI�� ����ɶ� Ÿ�Կ� ���� �߰��� �ٸ� UI�� �����ϴ� �̺�Ʈ�� �������� ���� </param>
    /// <returns> ���ῡ �����ϸ� true </returns>
    public bool CloseUI(eUI ui, bool isChainClose = true)
    {
        // 1. ���� ����Ʈ üũ
        if(!openList.Contains(ui))
        {
            return false;
        }

        // 2. ���� ��� UI ���� ȹ��
        if(!dicUI.TryGetValue(ui,out UIData uiData))
        {
            Debug.LogError($"UICloseFailed : [{ui}]�� �������� ���� Ÿ���� UI�Դϴ�.");
            return false;
        }

        // 3. �߰��� ���Ḧ �ϰ� �ش� UI�� ������ Ÿ���� ���
        if (isChainClose && uiData.uiClass.canvasType == eCanvas.Page)
        {
            // 3-1.�� ĵ������ Ȱ��ȭ
            scene.SetActivate(true);

            // 3-2. ���� �����ִ� UI�� üũ
            for (int i = openList.Count - 1; i >= 0; --i)
            {
                // 3-3. ���� UI�� �ƴϰ� �������� ��� ����
                UIBase temp = dicUI[openList[i]].uiClass;
                if (temp.uiType != uiData.uiClass.uiType && temp.canvasType != eCanvas.Scene)
                {
                    instance.CloseUI(temp.uiType, false);
                }
            }
        }

        //��� UI ���� ���μ��� ����
        uiData.uiClass.DataClear();
        openList.Remove(ui);
        return true;
    }

    //���� �� Pool�� ���ư�
    public void ReturnToUIPool(UIBase uiBase)
    {
        //UI Ǯ�� �̵�
        Debug.Log($"UIClose : [{uiBase.uiType}], CanvasType : [{uiBase.canvasType}]");
        uiBase.transform.SetParent(uiPool);
    }

    #endregion Close

    #region Get

    #region UI�� ���� ������Ʈ ��ȯ (GetUI)

    /// <summary> UI Ŭ������ �޴� �Լ� </summary>
    /// <typeparam name="T"> ��� UI�� �ִ� UIBase�� ��ӹ��� ���� Ŭ���� </typeparam>
    /// <returns> �˻� ���н� null ��ȯ </returns>
    public T GetUI<T>() where T : UIBase
    {
        return GetUI((eUI)Enum.Parse(typeof(eUI), typeof(T).Name)) as T;
    }

    /// <summary> UI Ŭ������ �޴� �Լ� </summary>
    /// <param name="ui"> ��� UI�� �Ҵ�� eUI </param>
    /// <returns> �˻� ���н� null ��ȯ </returns>
    public UIBase GetUI(eUI ui)
    {
        //UI�� ��ϵǾ� ���� ���
        if (dicUI.TryGetValue(ui, out UIData data))
        {
            //UI�� �ε����� ���
            if (data.uiClass != null)
            {
                return data.uiClass;
            }
            //UI�� �� �ѹ��� Ȱ��ȭ ������ ���� ���
            else
            {
                Debug.LogError($"[{ui}]Ÿ���� UI�� ���� ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        //UI�� ��� ������ �Ǿ� ���� ���
        else
        {
            Debug.LogError($"[{ui}]Ÿ���� UI�� ����� �����Ǿ� �ֽ��ϴ�. UIMgr.UIDataSetting() �� �����Ͻʽÿ�");
        }

        return null;
    }

    /// <summary> UI Ŭ������ �޴� �Լ� </summary>
    /// <typeparam name="T">UIBase �� ��ӹ��� UI Ŭ����</typeparam>
    /// <param name="uiBase"> �˻� ��� ��ȯ </param>
    /// <returns> �˻� ������ true </returns>
    public bool GetUI<T>(out T uiBase) where T : UIBase
    {
        uiBase = GetUI((eUI)Enum.Parse(typeof(eUI), typeof(T).Name)) as T;
        return uiBase != null;
    }

    /// <summary> UI Ŭ������ �޴� �Լ� </summary>
    /// <param name="ui"> ��� UI�� �Ҵ�� eUI </param>
    /// <param name="uiBase"> �˻� ��� ��ȯ </param>
    /// <returns> �˻� ������ true </returns>
    public bool GetUI(eUI ui, out UIBase uiBase)
    {
        if (!dicUI.TryGetValue(ui, out UIData data))
        {
            Debug.LogError($"[{ui}]Ÿ���� UI�� ã�� �� �����ϴ�.");
        }

        uiBase = data.uiClass;
        return uiBase != null;
    }

    #endregion UI�� ���� ������Ʈ ��ȯ (GetUI)

    /// <summary> Ÿ�Կ� �´� ĵ������ Transform�� ��ȯ </summary>
    public static Canvas GetCanvas(eCanvas uIType)
    {
        switch (uIType)
        {
            case eCanvas.Scene:
                return scene.canvas;
            case eCanvas.Page:
                return page.canvas;
            case eCanvas.Popup:
                return popup.canvas;
        }

        return null;
    }

    #endregion Get
}

#region ĵ���� ����
/// <summary> ĵ���� ������ </summary>
public class CanvasData
{
    public CanvasData(Canvas canvas, CanvasScaler scale, GraphicRaycaster rayCast)
    {
        this.canvas = canvas;
        this.scale = scale;
        this.rayCast = rayCast;
    }

    /// <summary> ĵ���� </summary>
    public Canvas canvas;
    /// <summary> �����Ϸ� </summary>
    public CanvasScaler scale;
    /// <summary> �׷��� ����ĳ��Ʈ </summary>
    public GraphicRaycaster rayCast;

    /// <summary> ĵ���� ������Ʈ Ȱ��ȭ ���� </summary>
    public bool SetActivate(bool isActive)
    {
        canvas.enabled = isActive;
        rayCast.enabled = isActive;

        return isActive;
    }
}
#endregion ĵ���� ����

#region UI ����
/// <summary> UI�� ���� </summary>
public class UIData
{
    public UIData(string path)
    {
        this.path = path;
    }

    /// <summary> ������ �н� </summary>
    public string path;
    /// <summary> UIŬ���� </summary>
    public UIBase uiClass;
}
#endregion UI ����