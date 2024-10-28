using UnityEngine;

public class CameraMove : MonoBehaviour, ICameraMove
{
    [Header(" Camera Move Control")]
    [SerializeField] float _smoothSpeed;
    [SerializeField] float _timeToCameraStatic;
    [Header(" Camera Height Borders")]
    [SerializeField] float _maxHeight;
    [SerializeField] float _minHeight;
    [SerializeField] LayerMask _groundLayer;
    [Header(" Camera Map Borders")]
    [SerializeField] float _westBorder;
    [SerializeField] float _eastBorder;
    [SerializeField] float _northBorder;
    [SerializeField] float _southBorder;

    private Transform _trCamera;
    private CameraAction _cameraAction;
    private bool _isCameraStatic;
    private float _timeCameraStatic;
    private float _cameraHeight;

    private Vector3 _newMovePosition;
    private Vector3 _newZoomPosition;

    private void Awake() {
        _trCamera = GetComponent<Transform>();

        _newMovePosition = _trCamera.position;
        _newZoomPosition = _trCamera.position;

        _cameraHeight = _trCamera.position.y;
    }

    private void Update() {
        if (_isCameraStatic && Time.time > _timeCameraStatic) _cameraAction = CameraAction.CameraOnStatic;

        switch (_cameraAction) {
            case CameraAction.CameraOnStatic:
                return;
            //break;

            case CameraAction.CameraMove:
                MoveCamera();
            break;

            case CameraAction.CameraZoom:
                ZoomCamera();
            break;
            
            case CameraAction.CameraLooked:
                //Camera looks on the lookTarget
            break;
        }
    }

    private void MoveCamera() {
        Vector3 hightPosition = CheckHeight(Vector3.Lerp(_trCamera.position, _newMovePosition, _smoothSpeed));

        Vector3 newPosition = CheckMapBorder(hightPosition);

        _trCamera.position = newPosition;
    }

    private void ZoomCamera() {
        _cameraHeight = Mathf.Clamp (
            Mathf.Lerp(_cameraHeight, _cameraHeight + _newZoomPosition.y - _trCamera.position.y, _smoothSpeed), 
            _minHeight, 
            _maxHeight
        );

        Vector3 hightPosition = CheckHeight(Vector3.Lerp(_trCamera.position, _newZoomPosition, _smoothSpeed));
        
        Vector3 newPosition = CheckMapBorder(hightPosition);

        _trCamera.position = newPosition;
    }

    private Vector3 CheckMapBorder(Vector3 cameraPosition) {
        float x = Mathf.Clamp(cameraPosition.x, _westBorder, _eastBorder);
        float z = Mathf.Clamp(cameraPosition.z, _southBorder, _northBorder);

        return new Vector3(x, cameraPosition.y, z);
    }

    private Vector3 CheckHeight(Vector3 cameraPosition) {
        if (Physics.Raycast(cameraPosition, Vector3.down, out RaycastHit hit, 100f, _groundLayer)) {
            return new Vector3 (
                cameraPosition.x, 
                cameraPosition.y + _cameraHeight - hit.distance, 
                cameraPosition.z
            );
        }
        else {
            Debug.Log("The terrain is lost!");

            return cameraPosition;
        }
    }

    public void SetNewMovePosition(Vector3 vec3) {
        _newMovePosition += vec3;
    }

    public void SetNewZoomPosition(Vector3 vec3) {
        if (vec3.y > 0f && _cameraHeight != _maxHeight) {
            _newZoomPosition += vec3;
        }
        else if (vec3.y < 0f && _cameraHeight != _minHeight){
            _newZoomPosition += vec3;
        }
    }

    public void SwitchCameraAction(CameraAction cameraAction) {
        _cameraAction = cameraAction;

        _newMovePosition = _trCamera.position;
        _newZoomPosition = _trCamera.position;
    }

    public void CameraStaticOnEnable(bool onEnable) {
        if (onEnable) {
            _timeCameraStatic = Time.time + _timeToCameraStatic;

            _isCameraStatic = true;
        }
        else {
            _isCameraStatic = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 topLeft = new Vector3(_westBorder, 0, _northBorder);
        Vector3 topRight = new Vector3(_eastBorder, 0, _northBorder);
        Vector3 bottomLeft = new Vector3(_westBorder, 0, _southBorder);
        Vector3 bottomRight = new Vector3(_eastBorder, 0, _southBorder);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}

public enum CameraAction {
    CameraOnStatic,
    CameraMove,
    CameraZoom,
    CameraLooked
}

public interface ICameraMove {
    public void SwitchCameraAction(CameraAction cameraAction);
    public void SetNewMovePosition(Vector3 vec3);
    public void SetNewZoomPosition(Vector3 vec3);
    public void CameraStaticOnEnable(bool onEnable);
}