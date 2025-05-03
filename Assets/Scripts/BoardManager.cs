using System.Collections.Generic;using System.Linq;using Enums;
using JetBrains.Annotations;
using Pieces;
using TMPro;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    private static readonly int MainColor = Shader.PropertyToID("_MainColor");

    private List<HouseData> _houses = new();
    private List<GameObject> _pieces = new();
    private List<RingData> _rings = new();
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

    public bool IsValidMove(Piece piece, Vector2Int targetPos)
    {
        var house = _houses.FirstOrDefault(x => x.ring == targetPos.y && x.line == targetPos.x);
    
        if (!house)
        {
            return false;
        }

        // Regra básica: não pode mover para casa ocupada por peça sua
        return !house.occupant || house.occupant.owner != piece.owner;
    }

    public Vector3 GetWorldPosition(Vector2Int gridPos, float height = 0)
    {
        var house = _houses.First(x => x.ring == gridPos.y && x.line == gridPos.x);

        var worldPos = new Vector3(house.transform.position.x, height, house.transform.position.z);

        return worldPos;
    }

    public void OccupyHouse(Vector2Int pos, Piece piece)
    {
        var house = _houses.FirstOrDefault(x => x.ring == pos.y && x.line == pos.x);
    
        if (house != null)
        {
            house.occupant = piece;
        }
    }

    public void FreeHouse(Vector2Int pos)
    {
        var house = _houses.FirstOrDefault(x => x.ring == pos.y && x.line == pos.x);
    
        if (house != null)
        {
            house.occupant = null;
        }
    }

    public AreaTypeEnum GetAreaType(Vector2Int currentPos)
    {
        var house = _houses.FirstOrDefault(x => x.ring == currentPos.y && x.line == currentPos.x);
        
        if (!house)
        {
            Debug.LogError("line: " + currentPos.x + " ring: " + (currentPos.y));
            Debug.LogError("No house found: ");

            foreach (var houseData in _houses)
            {
                Debug.LogError("line: " + houseData.line + " ring: " + houseData.ring);
            }
            
            return AreaTypeEnum.Neutral;
        }
        
        return house.owner;
    }

    public bool IsPositionInsideBoard(Vector2Int target)
    {
        var ring = target.x;
        var houseIndex = target.y;

        if (ring < 0 || ring >= _rings.Count) return false;

        var housesInThisRing = _rings.First(x => x.ringNumber == ring).houseQuantity;

        return houseIndex >= 0 && houseIndex < housesInThisRing;
    }

    public Piece GetPieceAt(Vector2Int target)
    {
        return _houses.FirstOrDefault(x => x.ring == target.y && x.line == target.x)!.occupant;
    }

    public List<HouseData> GetHouses(PlayerEnum? owner = null)
    {
        return owner.HasValue ? _houses.Where(x => x.owner == (AreaTypeEnum)owner.Value).ToList() : _houses;
    }

    public RingData GetRing(int ringNumber)
    {
        return _rings.First(x => x.ringNumber == ringNumber);
    }

    public List<RingData> GetRings()
    {
        return _rings;
    }

    public void AddRing(RingData ringData)
    {
        _rings.Add(ringData);
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
            house.transform.localScale *= (houseSize * 1.5f); // 1.5x maior
            house.name = "House_Center";
        }
        else
        {
            house.transform.localScale *= houseSize;
            house.name = $"House_Ring{ring}_Line{line}";
        }
            
        var rendererComponent = house.GetComponent<Renderer>();
            
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
    
    public GameObject GetNextPiece(PlayerEnum owner, PieceTypeEnum? pieceType = null)
    {
        if (!FilterPiece(owner, pieceType, out var filteredPieces, out var currentIndex)) return null;

        // Próximo índice, com wrap
        var nextIndex = (currentIndex + 1) % filteredPieces.Count;

        _currentFocusedPiece = filteredPieces[nextIndex];

        return _currentFocusedPiece;
    }
    
    public GameObject GetPreviousPiece(PlayerEnum owner, PieceTypeEnum? pieceType = null)
    {
        if (!FilterPiece(owner, pieceType, out var filteredPieces, out var currentIndex)) return null;

        // Próximo índice, com wrap
        var nextIndex = (currentIndex - 1) % filteredPieces.Count;

        nextIndex = nextIndex < 0 ? filteredPieces.Count - 1 : nextIndex; 
        
        _currentFocusedPiece = filteredPieces[nextIndex];

        return _currentFocusedPiece;
    }

    private bool FilterPiece(PlayerEnum owner, PieceTypeEnum? pieceType, out List<GameObject> filteredPieces, out int currentIndex)
    {
        currentIndex = 0;
        
        // Filtra as peças do jogador (e tipo, se especificado)
        filteredPieces = _pieces.Where(x =>
        {
            var piece = x.GetComponentInChildren<Piece>();
            return piece.owner == owner && (pieceType == null || piece.pieceType == pieceType);
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
        return _houses.First(house => house.ring == ring && house.line == line);
    }

    public int GetHousesCount(int? ring)
    {
        return !ring.HasValue ? _houses.Count : _houses.Count(house => house.ring == ring.Value);
    }
}