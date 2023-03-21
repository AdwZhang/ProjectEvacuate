using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMove : MonoBehaviour
{
    private MapNode targetNode;
    private MapNode beginNode;
    private int curNodeIndex;
    private List<MapNode> path;
    
    // Start is called before the first frame update
    void Start()
    {
        Transform parentTransform = transform.parent;
        Debug.Log(MapMgr.MapInstance.mapX);
        Debug.Log(MapMgr.MapInstance.mapZ);
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
            targetNode.Print();
        }

        beginNode = MapMgr.MapInstance.getMapNodeByXY(1, 1);
        beginNode.Print();
        // 测试寻路
        path = MapMgr.MapInstance.FindPath(beginNode, targetNode);
        path.Insert(0,beginNode);
        Debug.Log(path.Count);
        foreach (var node in path)
        {
            node.Print();
        }
        Debug.Log(targetNode.FCost);
        curNodeIndex = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = path[curNodeIndex].getMapNodeWorldPosition();
        targetPosition.y = transform.position.y;
        // 使用Vector3.Lerp插值计算当前帧的位置
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, 2.0f * Time.deltaTime);
        
        // 将物体移动到新位置
        transform.position = newPosition;

        if (Vector3.Distance(targetPosition, newPosition) <= 0.1f && curNodeIndex + 1 < path.Count)
        {
            curNodeIndex += 1;
        }
    }
}
