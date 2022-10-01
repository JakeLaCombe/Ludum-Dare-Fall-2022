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

    void Start()
    {
        playerInput = GetComponent<IInputable>();

        GameObject startingPoint = GameObject.Find("StartingPoint");
        playerPath.SetTile(Vector3Int.FloorToInt(startingPoint.transform.position), basePathTile);
        currentLocation = Vector3Int.FloorToInt(playerPath.WorldToCell(startingPoint.transform.position));
    }

    // Update is called once per frame
    void Update()
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
            playerPath.SetTile(currentLocation, basePathTile);
        }
    }
}
