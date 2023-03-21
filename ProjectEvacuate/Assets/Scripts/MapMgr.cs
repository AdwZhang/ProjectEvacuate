using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapMgr : AStarPathfinding
{
    public int mapX;
    
    public int mapZ;
    
    public float blockSize;
    
    public float blockHeight;

    public String loadMapTexture = "Map/mapDemo";

    private bool[,] isGoMapData;

    private MapNode[,] _mapNodes;

    private static MapMgr _mapInstance;
    // 静态实例
    public static MapMgr MapInstance{
        get
        {
            if (_mapInstance == null)
            {
                /*// 在场景中查找GlobalScript实例
                _mapInstance = FindObjectOfType<MapMgr>();

                // 如果没有找到，创建一个新的游戏对象并添加MapMgr组件
                if (_mapInstance == null)
                {
                    GameObject globalObject = new GameObject("MapMgr");
                    _mapInstance = globalObject.AddComponent<MapMgr>();
                }*/
                throw new Exception("在没有MapMgr结点的情况下试图获取地图信息");
            }
            return _mapInstance;
        }
        private set {}
    }   

    // 初始化
    public void Awake()
    {
        // 确保只有一个实例存在
        if (_mapInstance == null)
        {
            _mapInstance = this;
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
        _mapNodes = new MapNode[mapX, mapZ];

        if (mapTexture)
        {
            RemoveAllChildren();
            GameObject cubePrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Editor/AStarMapEditor/Prefabs/blockNoCollider.prefab");
                //AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Editor/AStarMapEditor/Prefabs/block.prefab");
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

                    _mapNodes[j,i] = new MapNode((rawData[i * mapX + j] != 0), j, i);
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

    public Vector3 getMapNodeWorldPosition(MapNode node)
    {
        Vector3 pos = new Vector3();
        pos.x = blockSize * (node.gridX + 0.5f);
        pos.y = blockSize * 0.5f;
        pos.z = blockSize * (node.gridY + 0.5f);
        Vector3 worldPos = gameObject.transform.TransformPoint(pos);
        return worldPos;
    }
    
    // 根据世界坐标获取对应的地图格子
    public MapNode getMapNodeByWorldPosition(Vector3 worldPos)
    {
        // 将世界坐标转移到MapMgr的子空间坐标系下
        Vector3 objectPos = gameObject.transform.InverseTransformPoint(worldPos);
        // 计算得出对应的x,y
        int girdX = (int)Math.Floor(objectPos.x / blockSize);
        int girdY = (int)Math.Floor(objectPos.z / blockSize);

        return MapInstance.getMapNodeByXY(girdX,girdY);
    }

    public MapNode getMapNodeByXY(int girdX, int girdY)
    {
        if (girdX >= this.mapX || girdY >= this.mapZ)
        {
            throw new Exception("试图计算地图范围外的结点");
        }
        return _mapNodes[girdX,girdY];
    }
    
    public List<MapNode> GetNeighbors(MapNode node)
    {
        List<MapNode> nodeList = new List<MapNode>();

        for (int i = node.gridX - 1; i <= node.gridX + 1; i++)
        {
            if (i >= this.mapX || i < 0)
            {
                continue;
            }
            for (int j = node.gridY - 1; j <= node.gridY + 1; j++)
            {
                if (j >= this.mapZ || j < 0)
                {
                    continue;
                }

                MapNode neighbor = this.getMapNodeByXY(i, j);
                if (neighbor.walkable)
                {
                    nodeList.Add(neighbor);
                }
            }
        }

        return nodeList;
    }

    // 调用这个方法以删除所有子节点
    public void RemoveAllChildren()
    {
        foreach (Transform child in transform)
        {
            // 销毁子节点
            Destroy(child.gameObject);
        }
    }

}