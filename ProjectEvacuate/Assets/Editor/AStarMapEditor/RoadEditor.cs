using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[CustomEditor(typeof(MapMgr))]
public class RoadEditor : Editor
{
    private MapMgr script;
    private bool placing = false;
    private bool enterPlacingBatMode = false;
    
    private string mapPath = "Assets/Resources/Map/mapDemo.asset";

    public void OnSceneGUI()
    {
        if (this.placing == false)
        {
            return;
        }

        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Space)     // 连续模式
            {
                Event.current.Use();    // 设置按键事件不下传
                this.enterPlacingBatMode = !this.enterPlacingBatMode;
            }
            else if (Event.current.keyCode == KeyCode.C)    // 单个模式
            {
                Event.current.Use();    // 设置按键事件不下传
                this.catchSetBlock();
                this.enterPlacingBatMode = false;
            }
        }

        if (this.enterPlacingBatMode == false) return;
        this.catchSetBlock(1);
    }

    private void catchSetBlock(int value = -1)
    {
        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hitInfo;
        if (!Physics.Raycast(worldRay, out hitInfo))
        {
            Debug.Log("未检测到碰撞");
            return;
        }

        if (hitInfo.collider.gameObject.name != "block")
        {
            Debug.Log("检测到非block块");
            return;
        }

        Debug.Log("检测到碰撞");
        this.setMapValue(ref hitInfo, value);
    }

    void setMapValue(ref RaycastHit hitInfo, int value)
    {
        BlockData data = hitInfo.collider.gameObject.GetComponent<BlockData>();
        if (data == null)
        {
            return;
        }

        // 如果是默认参数，即为-1时，不赋值，改为0、1切换
        if (value == -1)
        {
            data.isGo = data.isGo == 1 ? 0 : 1;
        }
        else
        {
            data.isGo = value;
        }
        
        if (data.isGo == 1)
        {
            hitInfo.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            hitInfo.collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
    
    private void OnDisable()
    {
        this.placing = false;
        this.enterPlacingBatMode = false;
    }

    public static SceneView GetSceneView()
    {
        SceneView view = SceneView.lastActiveSceneView;
        if (view == null)
        {
            view = EditorWindow.GetWindow<SceneView>();
        }
        return view;
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        this.script = (MapMgr)this.target;
        GUILayout.Label("设置配置数据文件的生成路径");
        this.mapPath = GUILayout.TextField(mapPath);

        SceneView view = GetSceneView();

        if (!this.placing && GUILayout.Button("Start Editing", GUILayout.Height(40)))
        {
            this.placing = true;
            this.enterPlacingBatMode = false;
            view.Focus();
        }

        GUI.backgroundColor = Color.yellow;

        if (this.placing && GUILayout.Button("Finish Editing", GUILayout.Height(40)))
        {
            this.placing = false;
            this.enterPlacingBatMode = false;
            this.ExportBitMap();
        }
        
    }

    // 导出成位图
    private void ExportBitMap()
    {
        Texture2D mapTex = new Texture2D(this.script.mapX, this.script.mapZ, TextureFormat.Alpha8,false);
        byte[] rawData = mapTex.GetRawTextureData();
        for (int i = 0; i < rawData.Length; i++)
        {
            rawData[i] = 0;
        }

        for (int i = 0; i < this.script.gameObject.transform.childCount; i++)
        {
            BlockData blockData = this.script.gameObject.transform.GetChild(i).GetComponent<BlockData>();
            rawData[i] = (byte) ((blockData.isGo == 0) ? 0 : 255);
        }
        
        mapTex.LoadRawTextureData(rawData);
        AssetDatabase.DeleteAsset(this.mapPath);
        AssetDatabase.CreateAsset(mapTex,this.mapPath);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
    }
}
