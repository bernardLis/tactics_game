using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;

public class RatBattleManger : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    TurnManager _turnManager;

    [Header("General")]
    [SerializeField] GameObject _envObjectsHolder;
    [SerializeField] TextAsset _graphData;

    [Header("Rats")]
    [SerializeField] GameObject _ratPrefab;
    [SerializeField] Brain[] _ratBrains;

    [Header("Player")]
    [SerializeField] GameObject _playerPrefab;


    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _turnManager = TurnManager.Instance;
        MapSetUp();
    }

    async void MapSetUp()
    {
        _battleManager.GetComponent<TileManager>().SetUp();

        await SetupAstar();
        await SpawnEnemies();
        await SpawnPlayer();
    }

    async Task SetupAstar()
    {
        AstarData data = AstarPath.active.data;

        data.DeserializeGraphs(_graphData.bytes);
        GridGraph gg = data.gridGraph;

        // Setup a grid graph with some values
        int width = 20;
        int depth = 20;
        float nodeSize = 1;
        gg.center = new Vector3(5, 5, 0);

        // Updates internal size from the above values
        gg.SetDimensions(width, depth, nodeSize);

        // Scans all graphs
        AstarPath.active.Scan();

        await Task.Delay(10);
    }

    async Task SpawnEnemies()
    {
        Vector3Int[] ratSpawnPositions = new Vector3Int[]{
            new Vector3Int(2,1,1),
            new Vector3Int(3,2,1),
            new Vector3Int(7,2,1),
            new Vector3Int(7,4,1)
        };

        for (int i = 0; i < 4; i++)
        {

            EnemyCharacter enemySO = (EnemyCharacter)ScriptableObject.CreateInstance<EnemyCharacter>();

            enemySO.CreateEnemy(1, _ratBrains[Random.Range(0, _ratBrains.Length)]);

            Vector3 spawnPos = new Vector3(ratSpawnPositions[i].x + 0.5f, ratSpawnPositions[i].y + 0.5f);
            Character instantiatedSO = Instantiate(enemySO);
            GameObject newCharacter = Instantiate(_ratPrefab, spawnPos, Quaternion.identity);

            instantiatedSO.Initialize(newCharacter);
            newCharacter.name = instantiatedSO.CharacterName;
            newCharacter.transform.parent = _envObjectsHolder.transform;

            newCharacter.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);
            newCharacter.GetComponent<CharacterStats>().MovementRange.BaseValue = 1;

            CharacterRendererManager characterRendererManager = newCharacter.GetComponentInChildren<CharacterRendererManager>();

            characterRendererManager.transform.localPosition = Vector3.zero; // normally, characters are moved by 0.5 on y axis
            characterRendererManager.Face(Vector2.left);
        }

        await Task.Delay(10);
    }

    async Task SpawnPlayer()
    {
        Character playerCharacter = _gameManager.PlayerTroops[0];
        Vector3 placementPosition = new Vector3(-3.5f, 5.5f);
        GameObject playerGO = Instantiate(_playerPrefab, placementPosition, Quaternion.identity);

        playerGO.name = playerCharacter.CharacterName;
        Character instantiatedSO = Instantiate(playerCharacter);
        instantiatedSO.Initialize(playerGO);
        playerGO.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);

        playerGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.right);

        await Task.Delay(10);

        _turnManager.UpdateBattleState(BattleState.PlayerTurn);
    }



}
