using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathBuilding : MonoBehaviour
{
    // Start is called before the first frame update
    public Tilemap possiblePath;
    public Tilemap playerPath;
    public TileBase basePathTile;
    public IInputable playerInput;
    public Vector3Int currentLocation;
    private List<AStarNode> newPlayerPath;
    private LevelState lastState;

    void Start()
    {
        playerInput = GetComponent<IInputable>();

        GameObject startingPoint = GameObject.Find("Player");
        Vector3Int tilePosition = Vector3Int.FloorToInt(startingPoint.transform.position);

        currentLocation = Vector3Int.FloorToInt(playerPath.WorldToCell(startingPoint.transform.position));

        newPlayerPath = new List<AStarNode>();
        newPlayerPath.Add(new AStarNode(false, tilePosition.x, tilePosition.y));
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.instance.currentState == LevelState.PLANNING)
        {
            PlayerInput();
        }

        if (lastState == LevelState.PLANNING && LevelManager.instance.currentState != lastState)
        {
            TransferPathToPlayer();
        }

        if (lastState == LevelState.ACTIVE && LevelManager.instance.currentState != lastState)
        {
            ResetPlayer();
        }

        if (lastState == LevelState.OBSERVING && LevelManager.instance.currentState != lastState)
        {
            playerPath.SetTile(currentLocation, basePathTile);
        }

        lastState = LevelManager.instance.currentState;

    }

    public void PlayerInput()
    {
        bool updateTiles = true;
        Vector3Int nextLocation = new Vector3Int(currentLocation.x, currentLocation.y, currentLocation.z);

        if (playerInput.Left())
        {
            nextLocation.x -= 1;
        }
        else if (playerInput.Right())
        {
            nextLocation.x += 1;
        }
        else if (playerInput.Up())
        {
            nextLocation.y += 1;
        }
        else if (playerInput.Down())
        {
            nextLocation.y -= 1;
        }
        else
        {
            updateTiles = false;
        }

        if (updateTiles && possiblePath.GetTile(nextLocation) != null && playerPath.GetTile(nextLocation) != basePathTile)
        {
            currentLocation = nextLocation;
            newPlayerPath.Add(new AStarNode(false, nextLocation.x, nextLocation.y));
            playerPath.SetTile(currentLocation, basePathTile);
        }
    }

    public void TransferPathToPlayer()
    {
        GameObject playerObject = GameObject.Find("Player");

        if (playerObject != null)
        {
            Player player = playerObject.GetComponent<Player>();
            player.SetTravelingPath(newPlayerPath);
            newPlayerPath = new List<AStarNode>();

            playerPath.ClearAllTiles();
        }
    }

    public void ResetPlayer()
    {
        GameObject playerObject = GameObject.Find("Player");

        if (playerObject != null)
        {
            Player player = playerObject.GetComponent<Player>();

            player.ResetMovement();
            currentLocation = Vector3Int.FloorToInt(playerPath.WorldToCell(playerObject.transform.position));
        }
    }
}
