using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;

// https://learn.unity.com/tutorial/level-generation?uv=5.x&projectId=5c514a00edbc2a0020694718#5c7f8528edbc2a002053b6f6
public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 10;
    public int rows = 10;

    public Tilemap backgroundTilemap;
    public Tilemap middlegroundTilemap;
    public Tilemap foregroundTilemap;

    public TilemapFlavour[] tilemapFlavours;

    public Count floorAdditionsCount = new Count(3, 7);
    public Count stoneCount = new Count(2, 5);
    public Count trapCount = new Count(1, 2);
    public Count enemyCount = new Count(1, 3);

    public GameObject stone;
    public GameObject trap;

    public Character[] enemyCharacters;
    public GameObject enemyGO;


    public Transform boardHolder;
    List<Vector3> gridPositions = new();

    void InitaliseList()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++) // -1 to leave outer ring of tiles free 
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        // TODO: choose map flavour
        TilemapFlavour flav = tilemapFlavours[0];
        TileBase[] floorTiles = flav.floorTiles;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++) // +-1 to create an edge;
            {
                backgroundTilemap.SetTile(new Vector3Int(x, y), floorTiles[Random.Range(0, floorTiles.Length)]);

                // tiles are overwritten in the process

                // edge
                if(x == -1)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), flav.edgeW);
                if(x == columns)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), flav.edgeE);
                if(y == -1)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), flav.edgeS);
                if(y == rows)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), flav.edgeN);

                // corners
                if (x == -1 && y == -1)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), flav.cornerSE);
                if (x == columns && y == -1)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), flav.cornerNE);
                if (x == -1 && y == rows)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), flav.cornerSW);
                if (x == columns && y == rows)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), flav.cornerNW);
            }
        }

    }

    Vector3 GetRandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex); // only one thing can occupy a position

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject obj, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosiiton = GetRandomPosition();
            Instantiate(obj, new Vector3(randomPosiiton.x + 0.5f, randomPosiiton.y + 0.5f, randomPosiiton.z), Quaternion.identity);
        }
    }

    void SpawnEnemies(int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Character instantiatedSO = Instantiate(enemyCharacters[Random.Range(0, enemyCharacters.Length)]);
            GameObject newCharacter = Instantiate(enemyGO, GetRandomPosition(), Quaternion.identity);

            instantiatedSO.Initialize(newCharacter);
            newCharacter.name = instantiatedSO.characterName;

            newCharacter.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);

            // face right
            CharacterRendererManager characterRendererManager = newCharacter.GetComponentInChildren<CharacterRendererManager>();
            characterRendererManager.Face(Vector2.right);
            characterRendererManager.Face(Vector2.zero);
        }
    }

    [ContextMenu("SetupScene")]
    public void SetupScene()
    {
        BoardSetup();
        InitaliseList();
        LayoutObjectAtRandom(stone, stoneCount.minimum, stoneCount.maximum);
        LayoutObjectAtRandom(trap, trapCount.minimum, trapCount.maximum);
        //SpawnEnemies(enemyCount.minimum, enemyCount.maximum);

        // TODO: spawn player chars / set-up player spawn positions;
    }

}
