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

        [Header("Settings")]
        [SerializeField] private int defaultPriority = 10;
        [SerializeField] private int activePriority = 20;

        private List<CinemachineVirtualCamera> _allCameras = new();

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
        }

        public void ActivateBoardOverview()
        {
            BoardManager.Instance.SetFocusedPiece(null);
            ActivateCamera(boardOverviewCamera);
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
                ? BoardManager.Instance.GetNextPiece(PlayerEnum.Player2, PieceTypeEnum.King) 
                : BoardManager.Instance.GetPreviousPiece(PlayerEnum.Player2, PieceTypeEnum.King);
            
            var cameraLocal = FocusOnPiece(dynamicPieceCamera, piece);
            
            ActivateCamera(cameraLocal);
        }

        public CinemachineVirtualCamera FocusOnPiece([CanBeNull] CinemachineVirtualCamera virtualCamera, GameObject piece)
        {
            if (!virtualCamera)
            {
                virtualCamera = dynamicPieceCamera;
            }
            
            virtualCamera.Follow = piece.transform;
            virtualCamera.LookAt = piece.transform;
            
            BoardManager.Instance.SetFocusedPiece(piece);
            
            return virtualCamera;
        }
    }
}