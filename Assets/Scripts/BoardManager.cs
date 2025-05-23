using System.Collections.Generic;
using System.Linq;
using Enums;
using JetBrains.Annotations;
using Pieces;
using TMPro;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    
    private static readonly int IsSelected = Shader.PropertyToID("_isSelected");
    private static readonly int MainColor = Shader.PropertyToID("_MainColor");
    private static readonly int HighlightColor = Shader.PropertyToID("_HighlightColor");

    private readonly List<HouseData> _houses = new();
    private readonly List<GameObject> _pieces = new();
    private readonly List<RingData> _rings = new();
    private readonly List<Vector2Int> _highlightedPositions = new();
    
    private GameObject _currentFocusedPiece;

    [Header("Prefabs")]
    [SerializeField] protected GameObject housePrefab;
    [SerializeField] protected GameObject board;
    [SerializeField] private GameObject pieces;

    [Header("Settings")]
    [SerializeField] protected float initialRadius = 1f;
    [SerializeField] protected float ringSpacing = 1f;
    [SerializeField] protected float houseSize = 0.5f;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GenerateBoard()
    {
        var board1 = new Board(initialRadius, ringSpacing);
    }

    public void SwitchOwner(Vector2Int gridPos, AreaTypeEnum owner)
    {
        var house = _houses.FirstOrDefault(x => x.ring == gridPos.y && x.line == gridPos.x);
    
        if (house != null)
        {
            house.owner = owner;
        }
    }

    public Vector3 GetWorldPosition(Vector2Int gridPos, float height = 0)
    {
        var house = _houses.First(x => x.ring == gridPos.y && x.line == gridPos.x);

        var worldPos = new Vector3(house.transform.position.x, height, house.transform.position.z);

        return worldPos;
    }

    private void OccupyHouse(Vector2Int pos, Piece piece)
    {
        var house = _houses.FirstOrDefault(x => x.ring == pos.y && x.line == pos.x);
    
        if (house)
        {
            house.occupant = piece;
            piece.currentHouse = house;
        }
    }

    public void FreeHouse(Vector2Int pos)
    {
        var house = _houses.FirstOrDefault(x => x.ring == pos.y && x.line == pos.x);
    
        if (house)
        {
            house.occupant = null;
        }
    }

    public List<HouseData> GetHouses(PlayerEnum? owner = null)
    {
        return owner.HasValue ? _houses.Where(x => x.owner == (AreaTypeEnum)owner.Value).ToList() : _houses;
    }

    public RingData GetRing(int ringNumber)
    {
        return _rings.FirstOrDefault(x => x.ringNumber == ringNumber);
    }

    public void AddRing(RingData ringData)
    {
        _rings.Add(ringData);
    }

    public void AddHighLightedPosition(Vector2Int pos)
    {
        _highlightedPositions.Add(pos);
    }
    
    public void AddHouse(int line, int ring, AreaTypeEnum area, Vector3 position, bool isCentral)
    {
        var house = Instantiate(housePrefab, position, Quaternion.identity, board.transform);
        var houseData = house.GetComponent<HouseData>();
        
        houseData.line = line;
        houseData.ring = ring;
        houseData.owner = area;
        
        var houseText = house.GetComponentInChildren<TextMeshPro>();
        houseText.text = $"LINE: {line} RING: {ring}";
        
        _houses.Add(houseData);
        
        if (isCentral)
        {
            house.transform.localScale *= houseSize * 1.5f; // 1.5x maior
            house.name = "House_Center";
        }
        else
        {
            house.transform.localScale *= houseSize;
            house.name = $"House_Ring{ring}_Line{line}";
        }
            
        var rendererComponent = house.GetComponentInChildren<Renderer>();
            
        if (rendererComponent)
        {
            Color materialColor;
            
            if (isCentral)
            {
                materialColor = Color.yellow;
            }
            else
            {
                materialColor = area switch
                {
                    AreaTypeEnum.Player1 => Color.blue,
                    AreaTypeEnum.Neutral => Color.gray,
                    AreaTypeEnum.Player2 => Color.red,
                    _ => rendererComponent.material.color
                };
            }
            
            rendererComponent.material.SetColor(MainColor, materialColor);
            rendererComponent.material.SetColor(HighlightColor, Color.magenta);
        }
    }

    public void AddPiece(GameObject prefab, Vector2Int position, PlayerEnum owner, PieceTypeEnum pieceType)
    {
        var instantiateGameObject = Instantiate(prefab, GetWorldPosition(position, 0.7f), Quaternion.identity, pieces.transform);
            
        var piece = instantiateGameObject.GetComponentInChildren<Piece>();

        if (piece)
        {
            piece.currentPosition = position;
            piece.owner = owner;
            piece.pieceType = pieceType;

            var rendererComponent = piece.GetComponent<Renderer>();

            if (rendererComponent)
            {
                var materialColor = piece.owner switch
                {
                    PlayerEnum.Player1 => (Color)new Color32(83, 83, 236, 255),
                    PlayerEnum.Player2 => (Color)new Color32(236, 83, 83, 255),
                    _ => rendererComponent.material.color
                };
            
                rendererComponent.material.SetColor(MainColor, materialColor);
                rendererComponent.material.SetColor(HighlightColor, Color.magenta);
            }
        }
 
        _pieces.Add(instantiateGameObject);
        
        OccupyHouse(position, piece);
    }

    public GameObject GetPieceByType(PieceTypeEnum pieceType, PlayerEnum owner)
    {
        return _pieces.First(x =>
        {
            var piece = x.GetComponentInChildren<Piece>();

            return piece.owner == owner && piece.pieceType == pieceType;
        });
    }
    
    public GameObject GetNextPiece()
    {
        if (!FilterPiece(null, out var filteredPieces, out var currentIndex))
        {
            return null;
        }

        // Próximo índice, com wrap
        var nextIndex = (currentIndex + 1) % filteredPieces.Count;

        _currentFocusedPiece = filteredPieces[nextIndex];

        return _currentFocusedPiece;
    }
    
    public GameObject GetPreviousPiece()
    {
        if (!FilterPiece(null, out var filteredPieces, out var currentIndex))
        {
            return null;
        }

        // Próximo índice, com wrap
        var nextIndex = (currentIndex - 1) % filteredPieces.Count;

        nextIndex = nextIndex < 0 ? filteredPieces.Count - 1 : nextIndex; 
        
        _currentFocusedPiece = filteredPieces[nextIndex];

        return _currentFocusedPiece;
    }

    private bool FilterPiece(PieceTypeEnum? pieceType, out List<GameObject> filteredPieces, out int currentIndex)
    {
        currentIndex = 0;
        
        // Filtra as peças do jogador (e tipo, se especificado)
        filteredPieces = _pieces.Where(x =>
        {
            var piece = x.GetComponentInChildren<Piece>();
            return pieceType == null || piece.pieceType == pieceType;
        }).ToList();

        if (filteredPieces.Count == 0)
        {
            return false;
        }

        if (_currentFocusedPiece)
        {
            currentIndex = filteredPieces.IndexOf(_currentFocusedPiece);
        
            if (currentIndex == -1) {
                currentIndex = 0; // caso a peça atual não esteja mais na lista
            }
        }

        return true;
    }

    public void SetFocusedPiece([CanBeNull] GameObject focusedPiece)
    {
        _currentFocusedPiece = focusedPiece;
    }

    public HouseData GetHouse(int line, int ring)
    {
        return _houses.FirstOrDefault(house => house.ring == ring && house.line == line);
    }

    public void OnHouseClicked(HouseData houseData)
    {
        if (houseData.occupant)
        {
            houseData.occupant.HasCaptured();
        }
        
        var piece = _currentFocusedPiece.GetComponent<Piece>();
        piece.Move(new Vector2Int(houseData.line, houseData.ring));
        
        GameManager.Instance.EndTurn();
    }
    
    public void ClearHighlights(Piece selectedPiece)
    {
        if (_currentFocusedPiece)
        {
            var currentFocusRenderer = _currentFocusedPiece.GetComponentInChildren<Renderer>();
            currentFocusRenderer.material.SetFloat(IsSelected, 0);
        }
        
        if (selectedPiece)
        {
            var pieceRenderer = selectedPiece.gameObject.GetComponentInChildren<Renderer>();
        
            if (pieceRenderer)
            {
                pieceRenderer.material.SetFloat(IsSelected, 0);
            }
        }
            
        foreach (var pos in _highlightedPositions)
        {
            var houseObj = GetHouse(pos.x, pos.y);
                
            if (houseObj)
            {
                var houseRenderer = houseObj.GetComponentInChildren<Renderer>();
                    
                if (houseRenderer)
                {
                    houseRenderer.material.SetFloat(IsSelected, 0);
                }

            }
        }

        _highlightedPositions.Clear();
    }
}