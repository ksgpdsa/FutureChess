using System;
using System.Collections.Generic;
using DatabasesInternal;
using DefaultNamespace;
using Enums;
using Pieces;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    public PlayerEnum currentPlayerEnum = PlayerEnum.Player1;
    public GameStateEnum gameStateEnum = GameStateEnum.Playing;

    [Header("Spawn Settings")]
    [SerializeField] public bool automaticSpawn = true;
    [SerializeField] public PieceDatabase pieceDatabase;
    
    [Header("Prefabs")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    
    private PieceManager _player1Pieces;
    private PieceManager _player2Pieces;

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

    private void Start()
    {
        BoardManager.Instance?.GenerateBoard();
        
        if (automaticSpawn)
        {
            // var player1Pieces = new PieceManager(1,1,5, 20, 5, 10);
            _player1Pieces = new PieceManager(1,1,1, 1, 1, 1);
            
            _player2Pieces = new PieceManager(1,1,1, 1, 1, 1);
            // var player2Pieces = new PieceManager(1,1,5, 20, 5, 10);
            
            SpawnPiecesForPlayer(PlayerEnum.Player1, _player1Pieces);
            SpawnPiecesForPlayer(PlayerEnum.Player2, _player2Pieces);
        }
    }

    public void CapturePiece(PlayerEnum ownerOfCaptured, PieceTypeEnum pieceType)
    {
        var manager = ownerOfCaptured switch
        {
            PlayerEnum.Player1 => _player1Pieces,
            PlayerEnum.Player2 => _player2Pieces,
            _ => throw new ArgumentOutOfRangeException(nameof(ownerOfCaptured), ownerOfCaptured, null)
        };

        switch (pieceType)
        {
            case PieceTypeEnum.King:
                manager.SetKingCount(manager.KingCount - 1);
                break;
            case PieceTypeEnum.Queen:
                manager.SetQueenCount(manager.QueenCount - 1);
                break;
            case PieceTypeEnum.Bishop:
                manager.SetBishopCount(manager.BishopCount - 1);
                break;
            case PieceTypeEnum.Rook:
                manager.SetRookCount(manager.RookCount - 1);
                break;
            case PieceTypeEnum.Knight:
                manager.SetKnightCount(manager.KnightCount - 1);
                break;
            case PieceTypeEnum.Pawn:
                manager.SetPawnCount(manager.PawnCount - 1);
                break;
        }

        if (manager.KingCount == 0)
        {
            EndGame(ownerOfCaptured == PlayerEnum.Player1 ? PlayerEnum.Player2 : PlayerEnum.Player1);
        }
    }

    private void SpawnPiecesForPlayer(PlayerEnum player, PieceManager pieceManager)
    {
        var availablePositions = GetAvailablePositions(player);

        var piecesToSpawn = GetPiecesToSpawn(pieceManager);

        Shuffle(availablePositions);

        var i = 0;
        
        foreach (var piece in piecesToSpawn)
        {
            if (i > availablePositions.Count)
            {
                Debug.LogWarning("Não há casas suficientes para todas as peças!");
                break;
            }

            var position = availablePositions[i];

            BoardManager.Instance.AddPiece(piece.gameObject, position, player, piece.pieceType);

            i++;
        }
    }

    private List<Piece> GetPiecesToSpawn(PieceManager pieceManager)
    {
        var piecesToSpawn = new List<Piece>();
        var kingsToSpawn = pieceManager.KingCount;
        var queuesToSpawn = pieceManager.QueenCount;
        var rooksToSpawn = pieceManager.RookCount;
        var knightsToSpawn = pieceManager.KnightCount;
        var pawnsToSpawn = pieceManager.PawnCount;
        var bishopsToSpawn = pieceManager.BishopCount;
        
        for (var i = 0; i < pieceManager.Count; i++)
        {
            var pieceType = PieceTypeEnum.King;
            
            if (kingsToSpawn > 0)
            {
                pieceType = PieceTypeEnum.King;
                kingsToSpawn--;
            }
            else if (queuesToSpawn > 0)
            {
                pieceType = PieceTypeEnum.Queen;
                queuesToSpawn--;
            }
            else if (rooksToSpawn > 0)
            {
                pieceType = PieceTypeEnum.Rook;
                rooksToSpawn--;
            }
            else if (bishopsToSpawn > 0)
            {
                pieceType = PieceTypeEnum.Bishop;
                bishopsToSpawn--;
            }
            else if (knightsToSpawn > 0)
            {
                pieceType = PieceTypeEnum.Knight;
                knightsToSpawn--;
            }
            else if (pawnsToSpawn > 0)
            {
                pieceType = PieceTypeEnum.Pawn;
                pawnsToSpawn--;
            }
            
            var prefab = pieceDatabase.GetPrefab(pieceType);
            
            if (prefab)
            {
                piecesToSpawn.Add(prefab.GetComponentInChildren<Piece>());
            }
        }

        return piecesToSpawn;
    }

    private static List<Vector2Int> GetAvailablePositions(PlayerEnum owner)
    {
        var positions = new List<Vector2Int>();

        foreach (var houseData in BoardManager.Instance.GetHouses(owner))
        {
            if (!houseData.occupant)
            {
                positions.Add(new Vector2Int(houseData.line, houseData.ring));
            }
        }

        return positions;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (var i = 0; i < list.Count; i++)
        {
            var rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }

    public void EndTurn()
    {
        if (gameStateEnum != GameStateEnum.Playing)
        {
            return;
        }

        currentPlayerEnum = currentPlayerEnum == PlayerEnum.Player1 ? PlayerEnum.Player2 : PlayerEnum.Player1;
        playerNameText.text = currentPlayerEnum.ToString();
        playerNameText.color = currentPlayerEnum == PlayerEnum.Player1 ? new Color32(52, 195, 217, 255) : new Color32(217, 52, 64, 255);

        switch (currentPlayerEnum)
        {
            case PlayerEnum.Player1:
                CameraManager.Instance.ActivatePlayer1View();
                break;
            case PlayerEnum.Player2:
                CameraManager.Instance.ActivatePlayer2View();
                break;
        }
    }

    public bool IsPlayerTurn(Piece piece)
    {
        return piece.owner == currentPlayerEnum;
    }

    public void PromotePawn(Pawn pawn, PieceTypeEnum pieceTypeEnum)
    {
        if (gameStateEnum != GameStateEnum.Playing)
        {
            return;
        }

        Debug.Log("Peão promovido!");

        gameStateEnum = GameStateEnum.Promotion;

        pawn.PromoteTo(pieceTypeEnum); // (no futuro podemos abrir UI para escolher)

        gameStateEnum = GameStateEnum.Playing;
    }

    private void EndGame(PlayerEnum winner)
    {
        gameStateEnum = GameStateEnum.Ended;
        Debug.Log("Fim de jogo! Vencedor: " + winner);
    }
}
