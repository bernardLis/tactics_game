using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Brain/Meele")]
public class MeeleBrain : Brain
{
    public override void Move()
    {
        Dictionary<GameObject, float> d = GetPlayerCharactersOderedByDistance();
        WorldTile attackPos = GetAttackPosition(d);
        Debug.Log("GetAttack position.x: " + attackPos.LocalPlace.x + ".y" + attackPos.LocalPlace.y);
        //GameObject target = //GetClosestPlayerCharacter();
        //Vector3 destination = GetDestination(target);
        // in the side 1, face to face 2, from the back 0, 

    }

    public override void Interact()
    {

    }

    // meele wants to attack anyone from the back
    // this means that they need a reachable  
    WorldTile GetAttackPosition(Dictionary<GameObject, float> _playerChars)
    {
        // TODO: does it make sense to get ALLL the tiles around ALLL the player characters and than work on that? 
        Dictionary<WorldTile, int> allAvailableTiles = new();
        foreach (KeyValuePair<GameObject, float> item in _playerChars)
        {
            Dictionary<WorldTile, int> targetTiles = GetFreeTilesAround(item.Key);
            foreach (KeyValuePair<WorldTile, int> tileItem in targetTiles)
            {
                allAvailableTiles[tileItem.Key] = tileItem.Value;
            }
        }

        // now I have list of all available tiles around all players, I need to know
        // whether there are reachable tiles where I can attack from the back 
        // i'd like to order the dict by value;
        allAvailableTiles.OrderBy(entry => entry.Value); // in the side 1, face to face 2, from the back 0

        // now I want to get the first tile that I can reach
        foreach (KeyValuePair<WorldTile, int> item in allAvailableTiles)
        {
            WorldTile tile = item.Key;
            if (!tile.WithinRange) // kinda sucky, but it works way better than calculating path and comparing to movement range
                continue;

            return tile;
        }

        return null;
    }
    // get destination will be different for each brain
    Vector3 GetDestination(GameObject _target)
    {
        Dictionary<WorldTile, int> targetTiles = GetFreeTilesAround(_target);
        foreach (KeyValuePair<WorldTile, int> item in targetTiles)
        {
            Debug.Log(item.Key + " " + item.Value);

        }
        /*
        Vector3 targetPos = new Vector3(targetTile.LocalPlace.x + 0.5f, targetTile.LocalPlace.y + 0.5f, targetTile.LocalPlace.z);
        Debug.Log("targetpos " + targetPos);

        Path p = seeker.StartPath(characterGameObject.transform.position, _target.transform.position);
        p.BlockUntilCalculated();
        // The path is calculated now
        // distance is the path length 
        // https://arongranberg.com/astar/docs_dev/class_pathfinding_1_1_path.php#a1076ed6812e2b4f98dca64b74dabae5d
        float distance = p.GetTotalLength();

        int distToTarget = Helpers.GetManhattanDistance(characterGameObject.transform.position, targetPos);
        
        Debug.Log("name: " + characterGameObject.name);

        Debug.Log("distance to target " + distToTarget);


        // it can't be man distance, i need to work on the path.
        // 1. we are next to the character = we don't move
        if (distToTarget <= 1)
        {
            Debug.Log("we are next to the character, right?");
            return Vector3.left;
        }
        // 2. we can reach empty tile next to the target = move to that tile
        if (distToTarget <= enemyStats.movementRange.GetValue())
        {
            Debug.Log("we can reach the target, right?");
            return Vector3.zero;

        }
        if (distToTarget > enemyStats.movementRange.GetValue())
            Debug.Log("we CANNOT reach the target, right?");

*/





        // 3. we cannot reach empty tile next to the target = move to the furthest point on path within our reach


        return Vector3.zero;
    }
}
