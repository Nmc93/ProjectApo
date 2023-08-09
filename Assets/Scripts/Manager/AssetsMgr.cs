using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetsMgr : MgrBase
{
    public static AssetsMgr instance;

    private void Awake()
    {
        instance = this;
    }

    /// <summary> ������ �⺻ ��� </summary>
    private const string UIPrefabPath = "Prefab/";

    #region UI ������ �ε�

    /// <summary> ��θ� �޾Ƽ� UI �������� �ε��ؼ� ��ȯ </summary>
    /// <param name="path"> ������Ʈ ��� </param>
    public static GameObject LoadResourcesUIPrefab(string path)
    {
        path = $"{UIPrefabPath}{path}";
        GameObject obj = Resources.Load<GameObject>(path);

        if (obj == null)
        {
            Debug.LogError($"�߸��� ����Դϴ�. [{path}]");
        }

        return obj;
    }

    /// <summary> ��θ� �޾Ƽ� UI �������� �ε��ؼ� ��ȯ </summary>
    /// <param name="path"> ������Ʈ ��� </param>
    public static bool LoadResourcesUIPrefab(string path, out GameObject obj)
    {
        path = $"{UIPrefabPath}{path}";
        
        //obj = PrefabUtility.InstantiatePrefab(Resources.Load(path)) as GameObject;
        obj = Resources.Load<GameObject>(path);

        if (obj == null)
        {
            Debug.LogError($"�߸��� ����Դϴ�. [{path}]");
        }

        return obj != null;
    }

    #endregion UI ������ �ε�
}