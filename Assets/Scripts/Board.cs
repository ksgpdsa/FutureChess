using Enums;
using UnityEngine;

public class Board
{
    private readonly float _initialRadius;
    private readonly float _ringSpacing;

    public Board(float initialRadius, float ringSpacing)
    {
        _initialRadius = initialRadius;
        _ringSpacing = ringSpacing;

        GenerateBoard();
    }

    private void GenerateBoard()
    {
        var currentRadius = _initialRadius;
        var ringNumber = 0;
        
        CreateRing(ringNumber, 0, 1, AreaTypeEnum.Player2, true);
        ringNumber++;

        // Jogador 1 - 8 anéis de 16 casas
        for (var i = 0; i < 8; i++)
        {
            CreateRing(ringNumber, currentRadius, 16, AreaTypeEnum.Player1);
            currentRadius += _ringSpacing;
            ringNumber++;
        }

        // Área Neutra - 4 anéis de 32 casas
        for (var i = 0; i < 4; i++)
        {
            CreateRing(ringNumber, currentRadius, 32, AreaTypeEnum.Neutral);
            currentRadius += _ringSpacing;
            ringNumber++;
        }

        // Jogador 2 - 2 anéis de 64 casas
        for (var i = 0; i < 2; i++)
        {
            CreateRing(ringNumber, currentRadius, 64, AreaTypeEnum.Player2);
            currentRadius += _ringSpacing;
            ringNumber++;
        }
    }

    private static void CreateRing(int ringNumber, float radius, int houseCount, AreaTypeEnum area, bool isCentral = false)
    {
        BoardManager.Instance.AddRing(new RingData(ringNumber, houseCount));
        
        for (var i = 1; i <= houseCount; i++)
        {
            var angle = i * (360f / houseCount);
            var position = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                0f,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );
            
            BoardManager.Instance.AddHouse(i, ringNumber, area, position, isCentral);
        }
    }
}