using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMove : MonoBehaviour
{
    public MapNode targetNode;
    
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
            Vector3 targetPos = parentTransform.position;
            targetNode = MapMgr.MapInstance.getMapNodeByWorldPosition(targetPos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
