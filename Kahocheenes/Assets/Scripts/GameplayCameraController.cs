using UnityEngine;

public class GameplayCameraController : MonoBehaviour
{
    [SerializeField] private Vector3 minCameraOffset;
    [SerializeField] private Vector3 maxCameraOffset;
    [SerializeField] private bool destroyPlayersByDistance; //TODO: MAKE CAMERA FOLLOW BEHIND PLAYERS
    [SerializeField] private float distanceToDestroyPlayer;
    [SerializeField] private float moveLerpSpeed;
    [SerializeField] private float rotationLerpSpeed;
    [SerializeField] private PlayerPositionTracker positionTracker;

    private Vector3 _goalPos;

    private Transform _transform;

    private GameplayControlsHandler _lastPlayer;

    private void Awake()
    {
        _transform = transform;
    }

    private void LateUpdate()
    {
        Vector3 currentPos = _transform.position;
        Vector3 avgPosition = positionTracker.AveragePosition;
        Vector3 nextPos = positionTracker.AveragePositionOnSpline;

        Vector3 resultPos = Vector3.Lerp(currentPos, nextPos, moveLerpSpeed * Time.deltaTime) + minCameraOffset;        //hotfix
        _transform.position = resultPos;

        Vector3 forward = (avgPosition - resultPos).normalized;
        Vector3 right = Vector3.Cross(forward, Vector3.up);
        Vector3 up = Vector3.Cross(right, forward);

        Quaternion goalRotation = Quaternion.LookRotation(forward, up);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, goalRotation, rotationLerpSpeed * Time.deltaTime);
    }
}