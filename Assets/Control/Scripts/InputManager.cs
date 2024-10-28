using UnityEngine;
using InputSystemActions;

public sealed class InputManager : MonoBehaviour
{
    [Header("Camera Control")]
    [SerializeField] private float _sensitivityMove;
    [SerializeField] private float _sensitivityZoom;

    private TouchscreenInputActions _touchscreenInputActions;
    private ICameraMove _icameraMove;
    private CameraAction _cameraAction;
    private bool _isCameraStaticStarted;
    private float _oldDistanceTouchPosition;

    private void Awake() {
        _icameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();

        _touchscreenInputActions = new TouchscreenInputActions(new InputMap());

        _touchscreenInputActions.FirstTouchActive += CameraMoveOnEnable;
        _touchscreenInputActions.SingleSwipeDelta += CameraMove;

        _touchscreenInputActions.SecondTouchActive += CameraZoomOnEnable;
        _touchscreenInputActions.DoubleTouchPositions += CameraZoom;
        
        GameplayInputActionMapOnEnable(true);
    }

    #region GameplayInputActionMap

        private void GameplayInputActionMapOnEnable(bool onEnable) {
            _touchscreenInputActions.GameplayInputOnEnable(onEnable);
        }

        private void CameraMoveOnEnable(bool onEnable) {
            if (onEnable) {
                if (_isCameraStaticStarted) _icameraMove.CameraStaticOnEnable(_isCameraStaticStarted = false);

                _icameraMove.SwitchCameraAction(_cameraAction = CameraAction.CameraMove);
            }
            else {
                if (_cameraAction == CameraAction.CameraMove) _icameraMove.CameraStaticOnEnable(_isCameraStaticStarted = true);
            }
        }

        private void CameraZoomOnEnable(bool onEnable) {
            if (onEnable) {
                if (_isCameraStaticStarted) _icameraMove.CameraStaticOnEnable(_isCameraStaticStarted = false);

                _oldDistanceTouchPosition = 0f;

                _icameraMove.SwitchCameraAction(_cameraAction = CameraAction.CameraZoom);
            }
            else {
                if (_cameraAction == CameraAction.CameraZoom) _icameraMove.CameraStaticOnEnable(_isCameraStaticStarted = true);
            }
        }

        private void CameraMove(Vector2 vec2) {
            vec2 = vec2 * _sensitivityMove * Time.deltaTime;

            Vector3 position =  new Vector3(vec2.x, 0f, vec2.y);

            _icameraMove.SetNewMovePosition(position);
        }

        private void CameraZoom(Vector2 firstVec2, Vector2 secondVec2) {
            float correntTouchDistance = Vector2.Distance(firstVec2, secondVec2);

            if (_oldDistanceTouchPosition == 0f) {
                _oldDistanceTouchPosition = correntTouchDistance;
                return;
            }
            
            Vector3 position;

            if (correntTouchDistance > _oldDistanceTouchPosition) {
                position = new Vector3(0f, -1f, 1f) * _sensitivityZoom * Time.deltaTime;
            }
            else {
                position = new Vector3(0f, 1f, -1f) * _sensitivityZoom * Time.deltaTime;
            }

            _icameraMove.SetNewZoomPosition(position);

            _oldDistanceTouchPosition = correntTouchDistance;
        }
    #endregion
}