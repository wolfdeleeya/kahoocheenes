using UnityEngine;

public class GameplayCameraController : MonoBehaviour
{
    [SerializeField] private float yOffset;
    [SerializeField] private float zOffset;
    [SerializeField] private float moveLerpSpeed;
    [SerializeField] private float rotationLerpSpeed;
    [SerializeField] private PlayerPositionTracker positionTracker;
    
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void LateUpdate()
    {
        Vector3 currentPos = _transform.position;
        Vector3 avgPosition = positionTracker.AveragePosition;
        Vector3 nextPos = positionTracker.AveragePositionOnSpline;

        Vector3 resultPos = Vector3.Lerp(currentPos, nextPos, moveLerpSpeed * Time.fixedTime);

        Vector3 trackForward = positionTracker.AveragePositionForward;
        Vector3 right = Vector3.Cross(trackForward, Vector3.up);
        Vector3 offset = Vector3.Cross(Vector3.up, right) * zOffset;
        
        resultPos += offset;
        resultPos.y = yOffset;
        _transform.position = resultPos;

        Vector3 forward = (avgPosition - resultPos).normalized;
        Vector3 up = Vector3.Cross(right, forward);
        

        Quaternion goalRotation = Quaternion.LookRotation(forward, up);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, goalRotation, rotationLerpSpeed * Time.fixedTime);
    }
}