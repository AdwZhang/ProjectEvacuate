using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AStarEditor : EditorWindow
{
    private int mapX = 10;
    private int mapZ = 10;
    private int blockSize = 1;
    private int blockHeight = 1;
    public GameObject root;
    
    [MenuItem("Tools/MapEditorGen")]
    static void hello()
    {
        EditorWindow.GetWindow<AStarEditor>();
    }

    // 编辑器主体
    public void OnGUI()
    {
        GUILayout.Label("地图X方向块数");
        this.mapX = Convert.ToInt32(GUILayout.TextField(this.mapX.ToString()));
        
        GUILayout.Label("地图Z方向块数");
        this.mapZ = Convert.ToInt32(GUILayout.TextField(this.mapZ.ToString()));
        
        GUILayout.Label("地图块大小");
        this.blockSize = Convert.ToInt32(GUILayout.TextField(this.blockSize.ToString()));
        
        GUILayout.Label("地图块高度");
        this.blockHeight = Convert.ToInt32(GUILayout.TextField(this.blockHeight.ToString()));
        
        GUILayout.Label("选择地图原点");
        if (Selection.activeGameObject != null)
        {
            GUILayout.Label(Selection.activeGameObject.name);
            this.root = Selection.activeGameObject; 
        }
        else
        {
            GUILayout.Label("当前没有选中的UI节点，无法生成");
        }

        this.addCreateMapBtn();
        this.addResetBtn();
        this.addClearBtn();
    }
    
    // ------------------------------- 按钮回调 ------------------------------

    // 添加生成地图块按钮
    private void addCreateMapBtn()
    {
        if (GUILayout.Button("在原点下生成地图块"))
        {
            if (Selection.activeGameObject != null)
            {
                Debug.Log("开始生成");
                this.createBlocks();
                Debug.Log("生成结束");
            }
            else
            {
                Debug.Log("当前没有选中的UI节点，无法生成");
            }
        }
    }

    
    // 添加重置地图块按钮
    private void addResetBtn()
    {
        if (GUILayout.Button("重置地图块"))
        {
            if (Selection.activeGameObject != null)
            {
                Debug.Log("开始重置");
                this.ResetBlocks();
                Debug.Log("重置结束");
            }
        }
    }
    
    // 清空地图块按钮
    private void addClearBtn()
    {
        if (GUILayout.Button("清空地图块"))
        {
            if (Selection.activeGameObject != null)
            {
                Debug.Log("开始清空");
                this.clearBlocks();
                Debug.Log("清空结束");
            }
        }
    }

    // ------------------------------- 功能函数 ------------------------------

    private void createBlocks()
    {
        this.clearBlocks();
        MapMgr mgr = this.root.GetComponent<MapMgr>();
        // 给原点添加 地图数据管理组件
        if (!mgr)
        {
            mgr = this.root.AddComponent<MapMgr>();
        }
        mgr.mapX = this.mapX;
        mgr.mapZ = this.mapZ;
        mgr.blockHeight = this.blockHeight;
        mgr.blockSize = this.blockSize;   

        GameObject cubePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Editor/AStarMapEditor/Prefabs/block.prefab");
        Vector3 startPos = new Vector3(this.blockSize*0.5f,this.blockHeight * 0.5f,this.blockSize * 0.5f);
        Vector3 pos = startPos;
        for (int i = 0; i < this.mapZ; i++)
        {
            pos.x = startPos.x;
            for (int j = 0; j < this.mapX; j++)
            {
                GameObject cube = PrefabUtility.InstantiatePrefab(cubePrefab) as GameObject;
                cube.transform.SetParent(this.root.transform,false);
                cube.transform.localScale = new Vector3(this.blockSize, this.blockHeight, this.blockSize);
                cube.transform.localPosition = pos;
                cube.name = "block";
                
                // 给每个地图块添加地图块数据组件
                BlockData block = cube.AddComponent<BlockData>();
                block.mapX = j;
                block.mapZ = i;
                block.isGo = 0;
                pos.x += this.blockSize;
            }
            pos.z += this.blockSize;
        }
    }

    // 重置地图块
    private void ResetBlocks()
    {
        int count = this.root.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            GameObject cube = this.root.transform.GetChild(i).gameObject;
            cube.GetComponent<BlockData>().isGo = 0;
        }
    }
    
    // 清空地图块
    private void clearBlocks()
    {
        int count = this.root.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            DestroyImmediate(this.root.transform.GetChild(0).gameObject);
        }
        
        /*MapMgr mgr = this.root.GetComponent<MapMgr>();
        // 删除原点的 地图数据管理组件
        if (mgr)
        {
            DestroyImmediate(mgr);
        }*/
    }
    
    private void OnSelectionChange()
    {
        this.Repaint();
    }
}
