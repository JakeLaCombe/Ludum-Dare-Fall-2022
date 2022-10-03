using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static SoundManager instance;
    public AudioSource PLAYER_HIT;
    public AudioSource ENEMY_HIT;
    public AudioSource PLANK_USE;
    public AudioSource PICKUP;
    public AudioSource VICTORY;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            PLAYER_HIT = this.transform.Find("PlayerDamage").GetComponent<AudioSource>();
            ENEMY_HIT = this.transform.Find("EnemyDamage").GetComponent<AudioSource>();
            PLANK_USE = this.transform.Find("WoodUse").GetComponent<AudioSource>();
            PICKUP = this.transform.Find("PlayerDamage").GetComponent<AudioSource>();
            VICTORY = this.transform.Find("Victory").GetComponent<AudioSource>();
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
