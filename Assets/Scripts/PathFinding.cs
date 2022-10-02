using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinding : MonoBehaviour
{
    public Tilemap tilemap;
    private Dictionary<int, Hashtable> nodes;


    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        nodes = GetCells();
        // List<AStarNode> nodes = FindPath(new Vector2(-7.0f, -1.5f), new Vector2(1.0f, 3.0f));
    }

    Dictionary<int, Hashtable> GetCells()
    {
        Dictionary<int, Hashtable> nodes = new Dictionary<int, Hashtable>();
        List<Vector3> tileWorldLocations = new List<Vector3>();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tilemap.CellToWorld(localPlace);

            if (tilemap.HasTile(localPlace))
            {
                TileBase tile = tilemap.GetTile(localPlace);
                tileWorldLocations.Add(place);

                if (!nodes.ContainsKey(pos.x))
                {
                    nodes[pos.x] = new Hashtable();
                }

                nodes[pos.x][pos.y] = new AStarNode(tilemap.GetColliderType(pos) == Tile.ColliderType.Sprite, pos.x, pos.y);
            }
        }

        return nodes;
    }

    public List<AStarNode> FindPath(Vector3 startPosition, Vector3 targetPos)
    {
        Dictionary<int, Hashtable> nodes = GetCells();
        Vector3Int startVertex = tilemap.WorldToCell(startPosition);
        Vector3Int targetVertex = tilemap.WorldToCell(targetPos);

        AStarNode start = (AStarNode)nodes[startVertex.x][startVertex.y];
        AStarNode target = (AStarNode)nodes[targetVertex.x][targetVertex.y];

        List<AStarNode> openNodes = new List<AStarNode>();
        HashSet<AStarNode> closedNodes = new HashSet<AStarNode>();

        openNodes.Add(start);

        while (openNodes.Count > 0 && target != null)
        {
            AStarNode currentNode = openNodes[openNodes.Count - 1];

            openNodes.RemoveAt(openNodes.Count - 1);
            closedNodes.Add(currentNode);

            if (currentNode == target)
            {
                return MakePath(start, target);
            }

            List<AStarNode> newNeighbors = new List<AStarNode>();

            foreach (AStarNode neighbor in getNeighbors(currentNode, nodes))
            {
                if (neighbor.isSolid || closedNodes.Contains(neighbor))
                {
                    continue;
                }

                if (!openNodes.Contains(neighbor))
                {
                    neighbor.parent = currentNode;
                    neighbor.gCost = currentNode.gCost + getManDistance(neighbor, currentNode);
                    neighbor.hCost = getManDistance(neighbor, target);
                    newNeighbors.Add(neighbor);
                }
            }

            newNeighbors.Sort((x, y) => y.fCost - x.fCost);
            openNodes = mergeLists(openNodes, newNeighbors);
        }

        return new List<AStarNode>();
    }

    public AStarNode GetNode(Vector3 position)
    {
        Vector3Int startVertex = tilemap.WorldToCell(position);

        return (AStarNode)nodes[startVertex.x][startVertex.y];
    }

    public int getManDistance(AStarNode start, AStarNode end)
    {
        //return Manhattan Distance between nodes
        return Mathf.Abs(start.posX - end.posX) + Mathf.Abs(start.posY - end.posY);
    }

    public List<AStarNode> mergeLists(List<AStarNode> list1, List<AStarNode> list2)
    {
        //performs a merge from mergesort on two node lists, preferring list2
        List<AStarNode> result = new List<AStarNode>();
        int i = 0, j = 0;

        while (i < list1.Count || j < list2.Count)
        {
            if (i >= list1.Count)
            {
                result.Add(list2[j]);
                j++;
                continue;
            }
            if (j >= list2.Count)
            {
                result.Add(list1[i]);
                i++;
                continue;
            }

            if (list1[i].fCost > list2[j].fCost)
            {
                result.Add(list1[i]);
                i++;
            }
            else
            {
                result.Add(list2[j]);
                j++;
            }
        }

        return result;
    }

    public List<AStarNode> MakePath(AStarNode start, AStarNode end)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode current = end;

        //iterate across the elements adding them and their then parents, etc.
        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    public List<AStarNode> getNeighbors(AStarNode node, Dictionary<int, Hashtable> map)
    {
        List<AStarNode> neighbors = new List<AStarNode>();

        if (map[node.posX].Contains(node.posY - 1))
        {
            neighbors.Add((AStarNode)map[node.posX][node.posY - 1]);
        }

        if (map[node.posX].Contains(node.posY + 1))
        {
            neighbors.Add((AStarNode)map[node.posX][node.posY + 1]);
        }

        if (map.ContainsKey(node.posX - 1) && map[node.posX - 1].Contains(node.posY))
        {
            neighbors.Add((AStarNode)map[node.posX - 1][node.posY]);
        }

        if (map.ContainsKey(node.posX + 1) && map[node.posX + 1].Contains(node.posY))
        {
            neighbors.Add((AStarNode)map[node.posX + 1][node.posY]);
        }

        return neighbors;
    }

    public Vector3 GetWorldCoordinates(AStarNode node)
    {
        return tilemap.CellToWorld(new Vector3Int(node.posX, node.posY, 0));
    }

    public Vector3Int GetCellPosition(Vector3 position)
    {
        return tilemap.WorldToCell(position);
    }

    public TileBase GetTile(Vector3 position)
    {
        Vector3Int cellPosition = GetCellPosition(position);
        if (tilemap.HasTile(cellPosition))
        {
            return tilemap.GetTile(cellPosition);
        }

        return null;
    }
}
