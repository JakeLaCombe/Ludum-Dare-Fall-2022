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
        TileBase tile = possiblePath.GetTile(Vector3Int.FloorToInt(startingPoint.transform.position));
        Debug.Log(tile);

        playerPath.SetTile(Vector3Int.FloorToInt(startingPoint.transform.position), basePathTile);
        possiblePath.SetTile(Vector3Int.FloorToInt(startingPoint.transform.position), basePathTile);

        currentLocation = Vector3Int.FloorToInt(startingPoint.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        bool updateTiles = true;

        if (playerInput.Left())
        {
            currentLocation.x -= 1;
        }
        else if (playerInput.Right())
        {
            currentLocation.x += 1;
        }
        else if (playerInput.Up())
        {
            currentLocation.y += 1;
        }
        else if (playerInput.Down())
        {
            currentLocation.y -= 1;
        }
        else
        {
            updateTiles = false;
        }

        if (updateTiles)
        {
            Debug.Log(possiblePath.GetTile(currentLocation));

            if (possiblePath.GetTile(currentLocation) != null)
            {
                playerPath.SetTile(currentLocation, basePathTile);
            }
        }
    }
}
