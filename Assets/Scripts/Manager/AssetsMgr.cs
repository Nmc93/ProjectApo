using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class AssetsMgr : MgrBase
{
    public static AssetsMgr instance;

    private void Awake()
    {
        instance = this;
    }

    #region ������

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

    #endregion ������

    #region ��Ʋ��

    /// <summary> ��Ʋ�� �⺻ ��� </summary>
    private const string AtlasPath = "Image/";
    /// <summary> ��Ʋ�� ���� ��ųʸ� <br/>[Key : ��Ʋ�� �� ��������Ʈ ���] </summary>
    private static Dictionary<string, SpriteAtlas> dicAtlas;

    /// <summary> �ش� ��Ʋ�� </summary>
    /// <param name="atlasPath"> SpriteAtlas ��� </param>
    /// <param name="spritePath"> SpriteAtlas�� ĳ�̵� Sprite ��� </param>
    public static Sprite GetSprite(string atlasPath, string spritePath)
    {
        //���, ��Ʋ�� ��ųʸ� Ű
        string key = $"{AtlasPath}{atlasPath}";

        if (!dicAtlas.TryGetValue(key, out SpriteAtlas atlas))
        {
            atlas = Resources.Load<SpriteAtlas>(key);

            //�ش� ��ο� ��Ʋ�󽺰� ���� ���
            if(atlas == null)
            {
                Debug.LogError($"{atlasPath}�� ��ο� �ش� ��������Ʈ ��Ʋ�󽺰� �����ϴ�.");
                return null;
            }

            //��Ʋ�� ĳ��
            dicAtlas.Add(key, atlas);
        }

        Sprite sprite = atlas.GetSprite(spritePath);

        if (sprite == null)
        {
            Debug.LogError($"{atlasPath}�� ��Ʋ�󽺿� {spritePath}�� ��ο� �ش� ��������Ʈ�� �����ϴ�.");
            return null;
        }

        return sprite;
    }

    #endregion ��Ʋ��

}