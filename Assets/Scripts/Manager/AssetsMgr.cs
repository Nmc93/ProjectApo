using GEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;

public class AssetsMgr : MgrBase
{
    public static AssetsMgr instance;

    private void Awake()
    {
        instance = this;
    }

    #region 기본 경로
    /// <summary> 프리팹 기본 경로 </summary>
    private const string UIPrefabPath = "Prefab/";
    /// <summary> 아틀라스 기본 경로 </summary>
    private const string AtlasPath = "Image/";
    /// <summary> 스프라이트 라이브러리 경로 </summary>
    private const string LibPath = "Image/";
    /// <summary> 애니메이터 컨트롤러 기본 경로 </summary>
    private const string AniCtlrPath = "Ani/";
    #endregion 기본 경로

    #region 프리팹

    /// <summary> 경로에 있는 프리팹을 복사해서 반환 </summary>
    /// <param name="path"> 오브젝트 경로 </param>
    public static GameObject LoadResourcesPrefab(string path)
    {
        path = $"{UIPrefabPath}{path}";
        GameObject obj = Instantiate(Resources.Load<GameObject>(path));

        if (obj == null)
        {
            Debug.LogError($"잘못된 경로입니다. [{path}]");
        }

        return obj;
    }

    /// <summary> 경로에 있는 프리팹을 복사해서 obj에 저장하고 성공 여부를 반환 </summary>
    /// <param name="path"> 오브젝트 경로 </param>
    public static bool LoadResourcesPrefab(string path, out GameObject obj)
    {
        path = $"{UIPrefabPath}{path}";
        
        obj = Instantiate(Resources.Load<GameObject>(path));

        if (obj == null)
        {
            Debug.LogError($"잘못된 경로입니다. [{path}]");
        }

        return obj != null;
    }

    #endregion 프리팹

    #region 스프라이트

    /// <summary> 아틀라스 저장 딕셔너리 <br/>[Key : 아틀라스 내 스프라이트 경로] </summary>
    private static Dictionary<string, SpriteAtlas> _dicAtlas = new();
    private static Dictionary<string, SpriteLibraryAsset> _dicLib = new();

    /// <summary> 해당 아틀라스 </summary>
    /// <param name="atlasType"> SpriteAtlas 경로 </param>
    /// <param name="spritePath"> SpriteAtlas에 캐싱된 Sprite 경로 </param>
    public static Sprite GetSprite(eAtlasType atlasType, string spritePath)
    {
        if(string.IsNullOrEmpty(spritePath) || spritePath == "None")
        {
            return null;
        }

        //경로, 아틀라스 딕셔너리 키
        string key = $"{AtlasPath}{ConvertEnumToPathStr(atlasType)}";

        if (_dicAtlas.TryGetValue(key, out SpriteAtlas atlas) == false)
        {
            atlas = Resources.Load<SpriteAtlas>(key);

            //해당 경로에 아틀라스가 없을 경우
            if(atlas == null)
            {
                Debug.LogError($"{atlasType}에 지정된 경로에 해당 스프라이트 아틀라스가 없습니다.");
                return null;
            }

            //아틀라스 캐싱
            _dicAtlas.Add(key, atlas);
        }

        Sprite sprite = atlas.GetSprite(spritePath);

        if (sprite == null)
        {
            Debug.LogError($"{atlasType}타입의 아틀라스에 {spritePath}의 경로에 해당 스프라이트가 없습니다.");
            return null;
        }

        return sprite;
    }

    /// <summary> eAtlasType를 지정된 아틀라스 경로 string으로 변환 </summary>
    /// <returns></returns>
    private static string ConvertEnumToPathStr(eAtlasType type)
    {
        switch(type)
        {
            case eAtlasType.Unit_Human:
                return "Human";
            case eAtlasType.Unit_Zombie:
                return "Zombie";
            default:
                return string.Empty;
        }
    }

    public static SpriteLibraryAsset GetSpriteLibraryAsset(string name)
    {
        if (string.IsNullOrEmpty(name) || name == "None")
        {
            return null;
        }

        //경로, 라이브러리 딕셔너리 키
        string key = $"{LibPath}{name}";

        if (_dicLib.TryGetValue(key, out SpriteLibraryAsset lib) == false)
        {
            lib = Resources.Load<SpriteLibraryAsset>(key);

            //해당 경로에 아틀라스가 없을 경우
            if (_dicLib == null)
            {
                Debug.LogError($"{key}에 경로에 해당 스프라이트 라이브러리에셋이 없습니다.");
                return null;
            }

            //아틀라스 캐싱
            _dicLib.Add(key, lib);
        }

        return lib;
    }

    #endregion 스프라이트

    #region 애니메이터 컨트롤러

    ///// <summary> Animator의 컨트롤러를 반환 </summary>
    ///// <param name="animType"> 애니메이션 타입 </param>
    //public static RuntimeAnimatorController GetUnitRuntimeAnimatorController(int animType)
    //{
    //    if (TableMgr.Get(animType, out UnitAnimatorTableData tbl) == false)
    //    {
    //        Debug.LogError($"{animType}의 ID를 가진 UnitAnimatorTableData가 없습니다.");
    //        return null;
    //    }
    //
    //    return GetRuntimeAnimatorController(tbl.Path);
    //}

    /// <summary> Animator의 컨트롤러를 반환 </summary>
    /// <param name="name"> 경로 </param>
    public static RuntimeAnimatorController GetRuntimeAnimatorController(string name)
    {
        name = $"{AniCtlrPath}{name}";

        RuntimeAnimatorController ctlr = Resources.Load<RuntimeAnimatorController>(name); ;

        return ctlr;
    }

    #endregion 애니메이터 컨트롤러 
}