using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;
    
    [Header("位置偏移")]
    public Vector3 offset = new Vector3(0f, 1.5f, 0f);
    
    [Header("跟随设置")]
    public float followSpeed = 10f;
    public bool smoothFollow = true;
    
    [Header("视角设置")]
    public float distance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float zoomSpeed = 2f;
    
    private float currentDistance;
    private Vector3 currentVelocity;
    
    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
        
        currentDistance = distance;
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        HandleZoom();
        FollowTarget();
    }
    
    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scrollInput * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }
    
    void FollowTarget()
    {
        // 计算目标位置（加上偏移）
        Vector3 targetPosition = target.position + offset;
        
        // 计算相机应该所在的位置
        // 注意：实际的相机位置由PlayerController中的鼠标控制决定
        // 这个脚本主要用于平滑跟随和缩放
        
        if (smoothFollow)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1f / followSpeed);
        }
        else
        {
            transform.position = targetPosition;
        }
    }
    
    // 设置新的跟随目标
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
