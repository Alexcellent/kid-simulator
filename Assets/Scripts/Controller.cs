// ENGL 398D - Videogames as Literature
// Final Project
//
// Alexandre Huot 9317023
//
// I certify this submission is my original work and meet's
// the Faculty's Expectations of Originality
//
// The logic behind code in this project is based in part 
// off of provided projects in my COMP 476 Lab sessions. 

using UnityEngine;
using System.Collections.Generic;

public class Controller : MonoBehaviour {

    /////////////////////////////////////////////
    // PREFABS
    public GroupBehaviour[] Groups;
    public GameObject[]     KidPrefabs;

    /////////////////////////////////////////////
    // LIST OF KIDS
    public List<GameObject> Kids;
    public int              NumKids;

    /////////////////////////////////////////////
    // COLLISIONS

    /////////////////////////////////////////////
    // GAME LOGIC
    public float StartDelay;
    public bool  GameStarted;
    public bool  GameEnd;
    public int   Wedgies = 0;

	void Start () {

        InitializeCollisionIgnore();

        for (int i = 0; i < NumKids; i++)
        {
            var position = new Vector2(Random.Range(-2F, 2F), Random.Range(-1F, 0.25F));
            var newKid = (GameObject)Instantiate(KidPrefabs[Random.Range(0,KidPrefabs.Length)], position, Quaternion.identity);
            Kids.Add(newKid);
        }

        AssignGroups();
	}
	
	void Update ()
	{

	    // CHECK GAME START
        if (!GameStarted)
	    {
	        StartDelay  = Mathf.Max(StartDelay - Time.deltaTime, 0);
            GameStarted = (StartDelay <= 0f);
	        return;
	    }

        foreach (var g in Groups)
        {
            g.AssignCollisionLayers();
        }

	    
	    // Present choices

	    // On input, split groups

	    // Countdown 5 seconds until recess start

	    // Groups allowed to wander, wedgies continue
	    // until 30seconds up or groups equal in size
        WinCondition();

	    // Output score and appicable message
	    // Play again? Y / N

	}

    void AssignGroups()
    {
        foreach (var k in Kids)
        {
            if (k.layer == 9)
            {
                k.layer = 8;
                Groups[0].Kids.Add(k);
                k.GetComponent<KidBehaviour>().Group = Groups[0];
            }
            else
            {
                k.layer = 8;
                Groups[1].Kids.Add(k);
                k.GetComponent<KidBehaviour>().Group = Groups[1];
            }
        }
    }

    void WinCondition()
    {
        foreach(var i in Groups)
        {
            if (i.Kids.Count == 0) continue;
            foreach (var j in Groups)
            {
                if (j.Kids.Count == 0) continue;
                if (!i.Equals(j) && i.Kids.Count > j.Kids.Count)
                {
                    return;
                }
            }
        }

        GameEnd = true;
        foreach (var g in Groups)
        {
            g.AssignCollisionLayers(true);
        }
    }

    void InitializeCollisionIgnore()
    {
        // Everything on the Ignores layer (8) ignores each other
        Physics2D.IgnoreLayerCollision( 8, 8, true);
        Physics2D.IgnoreLayerCollision( 9, 8, true);
        Physics2D.IgnoreLayerCollision(10, 8, true);
        Physics2D.IgnoreLayerCollision(11, 8, true);
        Physics2D.IgnoreLayerCollision(12, 8, true);

        // Groups Ignore Each Other
        Physics2D.IgnoreLayerCollision( 9,  9, true);
        Physics2D.IgnoreLayerCollision(10, 10, true);
        Physics2D.IgnoreLayerCollision(11, 11, true);
        Physics2D.IgnoreLayerCollision(12, 12, true);
    }
}
