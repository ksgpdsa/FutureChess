using System.Collections.Generic;
using UnityEngine;
using Pieces;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class PieceSelector : MonoBehaviour
    {
        private static readonly int IsSelected = Shader.PropertyToID("_isSelected");
        private static readonly int MainColor = Shader.PropertyToID("_MainColor");
        private Piece _selectedPiece;
        private readonly List<Vector2Int> _highlightedPositions = new();

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                HandleSelection();
            }
        }

        private void HandleSelection()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (!Physics.Raycast(ray, out var hit)) return;
            
            var clickedPiece = hit.collider.GetComponentInChildren<Piece>();

            if (clickedPiece /*&& GameManager.Instance.IsPlayerTurn(clickedPiece)*/) //todo: tirar o comentário
            {
                SelectPiece(clickedPiece);
            }
        }

        private void SelectPiece(Piece piece)
        {
            ClearHighlights();

            _selectedPiece = piece;
            CameraManager.Instance.FocusOnPiece(null, piece.gameObject);
            CameraManager.Instance.ActivateCamera(null);

            var validMoves = piece.GetValidMoves();
            
            foreach (var move in validMoves)
            {
                var houseObj = BoardManager.Instance.GetHouse(move.x, move.y);

                if (houseObj)
                {
                    var houseRenderer = houseObj.gameObject.GetComponent<Renderer>();
                    var pieceRenderer = piece.gameObject.GetComponent<Renderer>();

                    if (pieceRenderer)
                    {
                        pieceRenderer.material.SetColor(MainColor, Color.blue);
                        pieceRenderer.material.SetFloat(IsSelected, 1);
                    }
                    
                    if (houseRenderer)
                    {
                        houseRenderer.material.SetColor(MainColor, Color.black);
                        houseRenderer.material.SetFloat(IsSelected, 1);
                        _highlightedPositions.Add(move);
                    }
                }
            }
        }

        private void ClearHighlights()
        {
            if (_selectedPiece)
            {
                var pieceRenderer = _selectedPiece.gameObject.GetComponent<Renderer>();
        
                if (pieceRenderer)
                {
                    pieceRenderer.material.SetFloat(IsSelected, 0);
                }
            }
            
            foreach (var pos in _highlightedPositions)
            {
                var houseObj = BoardManager.Instance.GetHouse(pos.x, pos.y);
                
                if (houseObj)
                {
                    var houseRenderer = houseObj.GetComponent<Renderer>();
                    
                    if (houseRenderer)
                    {
                        houseRenderer.material.SetFloat(IsSelected, 0);
                    }

                }
            }

            _highlightedPositions.Clear();
        }
    }
}