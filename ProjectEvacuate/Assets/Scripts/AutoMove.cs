using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMove : MonoBehaviour
{
    private MapNode targetNode;
    private MapNode beginNode;
    
    // Start is called before the first frame update
    void Start()
    {
        Transform parentTransform = transform.parent;
        if (parentTransform == null)
        {
            Debug.Log("物体没有父节点");
        }
        else
        {
            PlayerParent playerParent = parentTransform.GetComponent<PlayerParent>();
            GameObject targetObject = playerParent.targetObject;
            Vector3 targetPos = targetObject.transform.TransformPoint(Vector3.zero);
            targetNode = MapMgr.MapInstance.getMapNodeByWorldPosition(targetPos);
            Debug.Log(targetNode.gridX);
            Debug.Log(targetNode.gridY);
            targetNode.Print();
        }

        beginNode = MapMgr.MapInstance.getMapNodeByXY(0, 0);
        
        // 测试寻路
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
