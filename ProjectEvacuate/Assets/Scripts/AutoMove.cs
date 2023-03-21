using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMove : MonoBehaviour
{
    private MapNode targetNode;
    private MapNode beginNode;
    private int curNodeIndex;
    private List<MapNode> path;
    private Rigidbody rb;
    private bool isMoving;
    private float timeToResume = 1.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isMoving = true;
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
        
        Vector3 beginPos = gameObject.transform.TransformPoint(Vector3.zero);
        beginNode = MapMgr.MapInstance.getMapNodeByWorldPosition(beginPos);
        beginNode.Print();
        // 测试寻路
        path = MapMgr.MapInstance.FindPath(beginNode, targetNode);
        path.Insert(0,beginNode);
        Debug.Log(path.Count);
        /*foreach (var node in path)
        {
            node.Print();
        }*/
        Debug.Log(targetNode.FCost);
        curNodeIndex = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            Move();
        }
    }

    void Move()
    {
        Vector3 targetPosition = path[curNodeIndex].getMapNodeWorldPosition();
        targetPosition.y = transform.position.y;
        // 使用Vector3.Lerp插值计算当前帧的位置
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, 2.0f * Time.deltaTime);
        RotateToMove(transform.position,targetPosition);
        // 将物体移动到新位置
        transform.position = newPosition;

        if (Vector3.Distance(targetPosition, newPosition) <= 0.1f && curNodeIndex + 1 < path.Count)
        {
            curNodeIndex += 1;
        }
        else if(Vector3.Distance(targetPosition, newPosition) <= 0.5f && curNodeIndex + 1 == path.Count)
        {
            isMoving = false;
            gameObject.GetComponentInChildren<Animator>().SetBool("isIdle",true);
        }
    }

    void RotateToMove(Vector3 beginPos, Vector3 targetPos)
    {
        Vector3 moveDir = Vector3.Normalize(targetPos - beginPos);
        // 计算面向移动方向的旋转
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);

        // 计算当前帧的旋转，使用Quaternion.Slerp插值
        float rotationSpeed = 2.0f; // 您可以根据需要调整旋转速度
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 将物体的旋转设置为计算出的旋转
        transform.rotation = newRotation;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (rb != null && isMoving)
        {
            isMoving = false;
            StartCoroutine(ResumeMoving());
        }
    }

    IEnumerator ResumeMoving()
    {
        yield return new WaitForSeconds(timeToResume);
        isMoving = true;
    }
}
