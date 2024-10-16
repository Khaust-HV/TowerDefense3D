using UnityEngine;
using InputSystemActions;

public sealed class InputManager : MonoBehaviour
{
    [SerializeField] private float _sensitivityMove;
    [SerializeField] private float _sensitivityZoom;

    private TouchscreenInputActions _touchscreenInputActions;
    private ICameraMove _icameraMove;
    private float _oldDistanceTouchPosition;

    private void Awake() {
        _icameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();

        _touchscreenInputActions = new TouchscreenInputActions(new InputMap());

        _touchscreenInputActions.SingleSwipeDelta += CameraMove;
        _touchscreenInputActions.DoubleTouchPositions += CameraZoom;
        _touchscreenInputActions.DoubleTouchActive += DistanceTouchPositionRefresh;
    }

    private void CameraMove(Vector2 vec2) {
        vec2 = vec2 * _sensitivityMove * Time.deltaTime;

        Vector3 newPosition =  new Vector3(vec2.x, 0f, vec2.y);

        _icameraMove.SetNewMovePosition(newPosition);
    }

    private void DistanceTouchPositionRefresh() {
        _oldDistanceTouchPosition = 0f;
    }

    private void CameraZoom(Vector2 firstVec2, Vector2 secondVec2) {
        float correntTouchDistance = Vector2.Distance(firstVec2, secondVec2);

        if (_oldDistanceTouchPosition == 0f) {
            _oldDistanceTouchPosition = correntTouchDistance;
            return;
        }
        
        Vector3 vec3;

        if (correntTouchDistance > _oldDistanceTouchPosition) {
            vec3 = new Vector3(0f, -1f, 1f) * _sensitivityZoom * Time.deltaTime;
        }
        else {
            vec3 = new Vector3(0f, 1f, -1f) * _sensitivityZoom * Time.deltaTime;
        }

        _icameraMove.SetNewZoomPosition(vec3);

        _oldDistanceTouchPosition = correntTouchDistance;
    }
}