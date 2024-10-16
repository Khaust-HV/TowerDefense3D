using UnityEngine;

public class CameraMove : MonoBehaviour, ICameraMove
{
    [SerializeField] float _smoothMoveSpeed;
    [SerializeField] float _smoothZoomSpeed;

    private Transform _trCamera;
    private Vector3 _newMovePosition;
    private Vector3 _newZoomPosition;

    private void Awake() {
        _trCamera = GetComponent<Transform>();

        _newMovePosition = _trCamera.position;
        _newZoomPosition = _trCamera.position;
    }

    private void Update() {
        MoveCamera();
        ZoomCamera();
    }

    private void MoveCamera() {
        _trCamera.position = Vector3.Lerp(_trCamera.position, _newMovePosition, _smoothMoveSpeed);
    }

    private void ZoomCamera() {
        _trCamera.position = Vector3.Lerp(_trCamera.position, _newZoomPosition, _smoothZoomSpeed);
    }

    public void SetNewMovePosition(Vector3 vec3) {
        _newMovePosition += vec3;
    }

     public void SetNewZoomPosition(Vector3 vec3) {
        _newZoomPosition += vec3;
    }
}

public interface ICameraMove {
    public void SetNewMovePosition(Vector3 vec3);
    public void SetNewZoomPosition(Vector3 vec3);
}