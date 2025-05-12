using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Enums;
using JetBrains.Annotations;

namespace DefaultNamespace
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;

        [Header("Cameras")]
        [SerializeField] private CinemachineVirtualCamera boardOverviewCamera;
        [SerializeField] private CinemachineVirtualCamera player1Camera;
        [SerializeField] private CinemachineVirtualCamera player2Camera;
        [SerializeField] private CinemachineVirtualCamera dynamicPieceCamera;
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private float zoomStep = 1f;

        [Header("Settings")]
        [SerializeField] private int defaultPriority = 10;
        [SerializeField] private int activePriority = 20;
        [SerializeField] private float rotateSpeed = 1;
        [SerializeField] private float panSpeed = 0.01f;

        private readonly List<CinemachineVirtualCamera> _allCameras = new();
        private CinemachineVirtualCamera _activeCamera;
        private Vector3 _lastMousePosition;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private float _initialFOV;
        private Vector3 _initialCameraPosition;
        private Quaternion _initialCameraRotation;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // Carrega todas as câmeras numa lista pra facilitar a troca
            _allCameras.Add(boardOverviewCamera);
            _allCameras.Add(player1Camera);
            _allCameras.Add(player2Camera);
            _allCameras.Add(dynamicPieceCamera);
        }

        private void Start()
        {
            // Começa com a câmera de overview ativa
            ActivateCamera(boardOverviewCamera);
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
            _initialCameraPosition = _activeCamera.transform.position;
            _initialCameraRotation = _activeCamera.transform.rotation;
            _initialFOV = _activeCamera.m_Lens.FieldOfView;
        }

        private void Update()
        {
            ManageInputZoom();
            HandleRotation();
            HandlePan();
        }

        public void ActivateCamera([CanBeNull] CinemachineVirtualCamera camToActivate)
        {   
            if (!camToActivate)
            {
                camToActivate = dynamicPieceCamera;
            }
            
            foreach (var cam in _allCameras)
            {
                cam.Priority = cam == camToActivate ? activePriority : defaultPriority;
            }
            
            _activeCamera = camToActivate;
        }

        private void ManageInputZoom()
        {
            if (!_activeCamera) return;

            var currentFOV = _activeCamera.m_Lens.FieldOfView;
            
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                currentFOV -= scroll * zoomStep;
            }

            // Pinça (mobile)
            if (Input.touchCount == 2)
            {
                var touchZero = Input.GetTouch(0);
                var touchOne = Input.GetTouch(1);

                // Distância entre os toques atuais e anteriores
                var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                currentFOV += deltaMagnitudeDiff * 0.1f; // sensibilidade do pinch
            }
            
            if (Input.GetKey(KeyCode.KeypadPlus)) // tecla "+"
            {
                currentFOV -= zoomStep * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.KeypadMinus)) // tecla "-"
            {
                currentFOV += zoomStep * Time.deltaTime;
            }
            
            // Clamp final
            currentFOV = Mathf.Clamp(currentFOV, 0, 100);
            
            _activeCamera.m_Lens.FieldOfView = currentFOV;
        }

        private void HandleRotation()
        {
            // Botão direito
            if (Input.GetMouseButtonDown(1))
            {
                _lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(1))
            {
                var delta = Input.mousePosition - _lastMousePosition;
                _lastMousePosition = Input.mousePosition;

                var angleY = delta.x * rotateSpeed * Time.deltaTime;
                var angleX = -delta.y * rotateSpeed * Time.deltaTime;

                transform.RotateAround(_activeCamera.transform.position, Vector3.up, angleY);
                transform.RotateAround(_activeCamera.transform.position, transform.right, angleX);
            }
        }

        private void HandlePan()
        {
            // Botão esquerdo
            if (Input.GetMouseButtonDown(0))
            {
                _lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                var delta = Input.mousePosition - _lastMousePosition;
                _lastMousePosition = Input.mousePosition;

                var right = transform.right;
                var up = transform.up;

                var move = (-right * delta.x - up * delta.y) * panSpeed;

                transform.position += move;
                _activeCamera.transform.position += move;
            }
        }
        
        public void ZoomIn()
        {
            _activeCamera.m_Lens.FieldOfView = Mathf.Max(0, _activeCamera.m_Lens.FieldOfView - zoomStep);
        }

        public void ZoomOut()
        {
            _activeCamera.m_Lens.FieldOfView = Mathf.Min(100, _activeCamera.m_Lens.FieldOfView + zoomStep);
        }
        
        private void ResetCamera()
        {
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
            _activeCamera.transform.position = _initialCameraPosition;
            _activeCamera.transform.rotation = _initialCameraRotation;
            _activeCamera.m_Lens.FieldOfView = _initialFOV;
        }

        public void ActivateBoardOverview()
        {
            ActivateCamera(boardOverviewCamera);
            ResetCamera();
        }

        public void ActivatePlayer1View()
        {
            var piece = BoardManager.Instance.GetPieceByType(PieceTypeEnum.King, PlayerEnum.Player1);
            var cameraLocal = FocusOnPiece(player1Camera, piece);
            
            ActivateCamera(cameraLocal);
        }

        public void ActivatePlayer2View()
        {
            var piece = BoardManager.Instance.GetPieceByType(PieceTypeEnum.King, PlayerEnum.Player2);
            var cameraLocal = FocusOnPiece(player2Camera, piece);
            
            ActivateCamera(cameraLocal);
        }

        public void ActivateDynamicPieceView(bool isNext)
        {
            var piece = isNext 
                ? BoardManager.Instance.GetNextPiece() 
                : BoardManager.Instance.GetPreviousPiece();
            
            var cameraLocal = FocusOnPiece(dynamicPieceCamera, piece);
            
            ActivateCamera(cameraLocal);
        }

        public CinemachineVirtualCamera FocusOnPiece([CanBeNull] CinemachineVirtualCamera virtualCamera, GameObject piece, List<Vector2Int> validMoves = null)
        {
            if (!virtualCamera)
            {
                virtualCamera = dynamicPieceCamera;
            }
            
            targetGroup.m_Targets = new CinemachineTargetGroup.Target[1];

            targetGroup.m_Targets[0] = new CinemachineTargetGroup.Target
            {
                target = piece.transform,
                weight = 1,
                radius = 0
            };
            
            if (validMoves != null)
            {
                targetGroup.m_Targets = new CinemachineTargetGroup.Target[validMoves.Count + 1];
                
                for (var i = 0; i < validMoves.Count; i++)
                {
                    targetGroup.m_Targets[i + 1] = new CinemachineTargetGroup.Target
                    {
                        target = BoardManager.Instance.GetHouse(validMoves[i].x, validMoves[i].y).transform,
                        weight = 0.5f,
                        radius = 0
                    };
                }
            }
            
            BoardManager.Instance.SetFocusedPiece(piece);
            
            return virtualCamera;
        }
    }
}