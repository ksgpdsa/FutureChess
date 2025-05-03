using System.Collections.Generic;using DatabasesInternal;using DefaultNamespace;using Enums;using Pieces;using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    public PlayerEnum currentPlayerEnum = PlayerEnum.Player1;
    public GameStateEnum gameStateEnum = GameStateEnum.Playing;

    [Header("Spawn Settings")]
    [SerializeField] public bool automaticSpawn = true;
    [SerializeField] public PieceDatabase pieceDatabase;

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
            var player1Pieces = new PieceManager(1,0,0, 0, 0, 0);
            
            var player2Pieces = new PieceManager(1,0,0, 0, 0, 0);
            // var player2Pieces = new PieceManager(1,1,5, 20, 5, 10);
            
            SpawnPiecesForPlayer(PlayerEnum.Player1, player1Pieces);
            SpawnPiecesForPlayer(PlayerEnum.Player2, player2Pieces);
        }
    }

    private void SpawnPiecesForPlayer(PlayerEnum player, PieceManager pieceManager)
    {
        var availablePositions = GetAvailablePositions(player);

        var piecesToSpawn = GetPiecesToSpawn(pieceManager);

        // Shuffle(availablePositions);

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
        
        for (var i = 0; i < pieceManager.Count; i++)
        {
            var pieceType = PieceTypeEnum.King;
            
            if (pieceManager.KingCount > 0)
            {
                pieceType = PieceTypeEnum.King;
                pieceManager.KingCount--;
            }
            else if (pieceManager.QueenCount > 0)
            {
                pieceType = PieceTypeEnum.Queen;
                pieceManager.QueenCount--;
            }
            else if (pieceManager.RookCount > 0)
            {
                pieceType = PieceTypeEnum.Rook;
                pieceManager.RookCount--;
            }
            else if (pieceManager.BishopCount > 0)
            {
                pieceType = PieceTypeEnum.Bishop;
                pieceManager.BishopCount--;
            }
            else if (pieceManager.KnightCount > 0)
            {
                pieceType = PieceTypeEnum.Knight;
                pieceManager.KnightCount--;
            }
            else if (pieceManager.PawnCount > 0)
            {
                pieceType = PieceTypeEnum.Pawn;
                pieceManager.PawnCount--;
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
        if (gameStateEnum != GameStateEnum.Playing) return;

        currentPlayerEnum = currentPlayerEnum == PlayerEnum.Player1 ? PlayerEnum.Player2 : PlayerEnum.Player1;
        Debug.Log("Turno de: " + currentPlayerEnum);
    }

    public bool IsPlayerTurn(Piece piece)
    {
        return (piece.owner == currentPlayerEnum);
    }

    public void PromotePawn(Pawn pawn, PieceTypeEnum pieceTypeEnum)
    {
        if (gameStateEnum != GameStateEnum.Playing) return;

        Debug.Log("Peão promovido!");

        gameStateEnum = GameStateEnum.Promotion;

        pawn.PromoteTo(pieceTypeEnum); // (no futuro podemos abrir UI para escolher)

        gameStateEnum = GameStateEnum.Playing;
    }

    public void EndGame(PlayerEnum winner)
    {
        gameStateEnum = GameStateEnum.Ended;
        Debug.Log("Fim de jogo! Vencedor: " + winner);
    }
}
