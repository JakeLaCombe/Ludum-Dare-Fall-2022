using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PrefabsManager : MonoBehaviour
{
    public static PrefabsManager instance;
    public TileBase PLANKED_HOLE;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
