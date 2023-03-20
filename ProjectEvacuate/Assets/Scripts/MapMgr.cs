using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapMgr : MonoBehaviour
{
    public int mapX;
    
    public int mapZ;
    
    public float blockSize;
    
    public float blockHeight;

    public String loadMapTexture = "Map/mapDemo";

    private bool[,] isGoMapData;

    private static MapMgr _mapInstance;
    // 静态实例
    public static MapMgr MapInstance{
        get
        {
            if (_mapInstance == null)
            {
                // 在场景中查找GlobalScript实例
                _mapInstance = FindObjectOfType<MapMgr>();

                // 如果没有找到，创建一个新的游戏对象并添加MapMgr组件
                if (_mapInstance == null)
                {
                    GameObject globalObject = new GameObject("MapMgr");
                    _mapInstance = globalObject.AddComponent<MapMgr>();
                }
            }
            return _mapInstance;
        }
        private set {}
    }   

    // 初始化
    public void Awake()
    {
        // 确保只有一个实例存在
        if (MapInstance == null)
        {
            MapInstance = this;
            DontDestroyOnLoad(gameObject); // 使实例在加载新场景时不被销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        this.initMap();
    }

    private void initMap()
    {
        Texture2D mapTexture = Resources.Load(loadMapTexture) as Texture2D;
        mapX = mapTexture.width;
        mapZ = mapTexture.height;
        isGoMapData = new bool[mapX,mapZ];

        if (mapTexture)
        {
            GameObject cubePrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Editor/AStarMapEditor/Prefabs/block.prefab");
            byte[] rawData = mapTexture.GetRawTextureData();
            Vector3 startPos = new Vector3(blockSize * 0.5f, blockHeight * 0.5f, blockSize * 0.5f);
            Vector3 pos = startPos;
            for (int i = 0; i < mapZ; i++)
            {
                pos.x = startPos.x;
                for (int j = 0; j < mapX; j++)
                {
                    GameObject block = PrefabUtility.InstantiatePrefab(cubePrefab) as GameObject;
                    block.transform.localScale = new Vector3(blockSize, blockSize, blockHeight);
                    block.transform.localPosition = pos;
                    block.transform.SetParent(this.transform, false);
                    if (rawData[i * mapX + j] == 0)
                    {
                        isGoMapData[j, i] = false;
                        block.GetComponent<MeshRenderer>().enabled = true;
                    }
                    else
                    {
                        isGoMapData[j, i] = true;
                        block.GetComponent<MeshRenderer>().enabled = false;
                    }

                    pos.x += blockSize;
                }

                pos.z += blockSize;
            }
        }
    }

    public void Update()
    {
        //Texture2D mapTexture = Resources.Load("Image/board") as Texture2D;
    }

    public MapNode getMapNodeByWorldPosition(Vector3 worldPos)
    {
        MapNode x = null;
        return x;
    }
}