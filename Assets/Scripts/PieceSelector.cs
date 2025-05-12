using UnityEngine;
using Pieces;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class PieceSelector : MonoBehaviour
    {
        private static readonly int IsSelected = Shader.PropertyToID("_isSelected");
        private static readonly int MainColor = Shader.PropertyToID("_MainColor");

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                HandleSelection();
            }
        }

        private void HandleSelection()
        {
            // ReSharper disable once PossibleNullReferenceException
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (!Physics.Raycast(ray, out var hit))
            {
                return;
            }

            var clickedPiece = hit.collider.GetComponentInChildren<Piece>();

            if (clickedPiece)
            {
                SelectPiece(clickedPiece);
            }
        }

        private void SelectPiece(Piece piece)
        {
            BoardManager.Instance.ClearHighlights(piece);
            
            if (!GameManager.Instance.IsPlayerTurn(piece))
            {
                var house = BoardManager.Instance.GetHouse(piece.currentPosition.x, piece.currentPosition.y);

                if (house)
                {
                    house.ClickByPiece();
                }
                
                return;
            }
            
            var pieceRenderer = piece.gameObject.GetComponentInChildren<Renderer>();
                    
            if (pieceRenderer)
            {
                pieceRenderer.material.SetFloat(IsSelected, 1);
            }
            
            var validMoves = piece.GetValidMoves();
            
            CameraManager.Instance.FocusOnPiece(null, piece.gameObject, validMoves);
            CameraManager.Instance.ActivateCamera(null);
            
            foreach (var move in validMoves)
            {
                var houseObj = BoardManager.Instance.GetHouse(move.x, move.y);

                if (houseObj)
                {
                    var houseRenderer = houseObj.gameObject.GetComponentInChildren<Renderer>();
                    
                    if (houseRenderer)
                    {
                        houseRenderer.material.SetFloat(IsSelected, 1);
                        BoardManager.Instance.AddHighLightedPosition(move);
                    }
                }
            }
        }
    }
}