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
    // GAME LOGIC
    public bool  Assigned = false;
    public float StartDelay;
    public bool  GameStarted;
    public float GameTimer;
    public bool  GameEnd;
    public float GameEndTimer;
    

    /////////////////////////////////////////////
    // ATTRIBUTES
    public enum Attribute
    {
        none,
        glass3D,
        beanie
    };
    public Attribute Choice;

    ////////////////////////////////////////////
    // GUI
    public int Wedgies = 0;
    public TextMesh Value;
    public TextMesh Timer;

    ////////////////////////////////////////////
    // SOUND
    public AudioClip BellSound;
    public AudioClip BackGroundSound;

	void Start ()
	{

	    Choice = Attribute.none;
        
        AudioSource.PlayClipAtPoint(BackGroundSound, transform.position);

        InitializeCollisionIgnore();

        for (int i = 0; i < NumKids; i++)
        {
            var position = new Vector2(Random.Range(-2F, 2F), Random.Range(-0.8F, -0.1F));
            var newKid = (GameObject)Instantiate(KidPrefabs[Random.Range(0,100) % 3], position, Quaternion.identity);
            Kids.Add(newKid);
        }
	}
	
	void Update ()
	{
        // Udpate GUI
        UpdateHUD();

        // Until Choice, do nothing
	    if (Choice == Attribute.none) return;

        // Assign groups once
	    if (!Assigned)
	    {
	        Assigned = true;
            AssignGroups();
	    }

	    // CHECK GAME START
        if (!GameStarted)
	    {
	        StartDelay  = Mathf.Max(StartDelay - Time.deltaTime, 0);
            GameStarted = (StartDelay <= 0f);

	        if (GameStarted)
	        {
                AudioSource.PlayClipAtPoint(BellSound, transform.position);

	            GameTimer = 15f;
                foreach (var g in Groups)
                {
                    g.AssignCollisionLayers();
                }
	        }

	        return;
	    }

        WinCondition();

	    if (GameEnd)
	    {
	        GameEndTimer = Mathf.Max(0, GameEndTimer - Time.deltaTime);
            if (GameEndTimer <= 0f) Application.LoadLevel("test");
	    }
	}

    void UpdateHUD()
    {
        Value.text = "x " + Wedgies.ToString();
        Timer.text = "Recess ends in: " + GameTimer.ToString("0.00");
    }

    void AssignGroups()
    {
        foreach (var k in Kids)
        {

            switch (Choice)
            {
                case Attribute.glass3D:
                    if (k.GetComponent<KidBehaviour>().propBeanie) // Assign 'lame'kids to 1 group
                    {
                        k.layer = 8;
                        Groups[0].Kids.Add(k);
                        k.GetComponent<KidBehaviour>().Group = Groups[0];
                    }
                    else // Assign 'cool' kids and naked followers to other
                    {
                        k.layer = 8;
                        Groups[1].Kids.Add(k);
                        k.GetComponent<KidBehaviour>().Group = Groups[1];
                    }
                    break;

                case Attribute.beanie:
                    if (k.GetComponent<KidBehaviour>().glasses3D)
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
                    break;
            }
            
        }
    }

    void WinCondition()
    {

        GameTimer = Mathf.Max(0, GameTimer - Time.deltaTime);
        if (GameTimer <= 0f && !GameEnd)
        {
            GameEnd = true;
            foreach (var g in Groups)
            {
                g.AssignCollisionLayers(true);
            }
            AudioSource.PlayClipAtPoint(BellSound, transform.position);
            return;
        }


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

        if (!GameEnd) { 
            GameEnd = true;
            foreach (var g in Groups)
            {
                g.AssignCollisionLayers(true);
            }
            GameEndTimer = 10f;

            AudioSource.PlayClipAtPoint(BellSound, transform.position);
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
