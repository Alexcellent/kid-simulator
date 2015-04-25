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

using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class KidBehaviour : MonoBehaviour
{
    /////////////////////////////////////////////
    // STRATEGIC AI
    public GroupBehaviour Group;

    /////////////////////////////////////////////
    // MOVEMENT
    public float MaxSpeed;
    public float SeparationWeight;
    public float AlignmentWeight;
    public float CohesionWeight;
    public float SeekWeight;
    public float NeighborRadius;
    public float SeprationRadius;

    Vector2 _separation;
    Vector2 _alignment;
    Vector2 _cohesion;
    Vector2 _seek;
    Vector2 _intialVelocity;
    
    float _invincibleTimer = 0f;
    float _wanderTimer;

    /////////////////////////////////////////////
    // ATTRIBUTES
    public bool glasses3D;
    public bool propBeanie;


    /////////////////////////////////////////////
    // NEIGHBORS
    public List<GameObject> _neighbors;

    /////////////////////////////////////////////
    // GAME LOGIC
    public bool Wedgied;

    /////////////////////////////////////////////
    // ANIMATION
    public string SpriteSheetName;
    Animator      _anim;
    public AudioClip WedgieSound;

    void Start()
    {
        // Initial Movement
        _intialVelocity      = new Vector2(Random.Range(-0.1F, 0.1F), Random.Range(-0.1F, 0.1F)).normalized;
        rigidbody2D.velocity = _intialVelocity * MaxSpeed;
        _wanderTimer         = 0f;

        // Initialize neighbor list
        _neighbors = new List<GameObject>();

        // Initialize animator
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        //UpdateInvincible();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // Check if in play
        if (Wedgied)
        {
            ResetAll();
            return;
        }

        if (_invincibleTimer > 0) return;
        // Check behaviour type
        if (Group)
            FlockMovement();
        else
            WanderMovement();
    }

    void LateUpdate()
    {
        // This particular piece of cade was taken from:
        // https://youtu.be/rMCLWt1DuqI
        var subSprites = Resources.LoadAll<Sprite>("Sprites/" + SpriteSheetName);
        var renderer = GetComponent<SpriteRenderer>();
        var spriteName = renderer.sprite.name;
        var newSprite = Array.Find(subSprites, item => item.name == spriteName);

        if (newSprite)
            renderer.sprite = newSprite;

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (Group && col.tag == "Kid" && !Group.Kids.Contains(col.gameObject) && !Wedgied)
        {
            var k = col.GetComponent<KidBehaviour>();
            
            // Get probablility based on Group size
            var probWedgie = 0.5f;
            if (k.Group.Kids.Count > Group.Kids.Count)
            {
                probWedgie = 1 - probWedgie;
            }

            var win = (probWedgie > Random.Range(0f,1f));

            if (win)
            {
                // Opponent loses
                k.Wedgied = true;
                if (k.Group.Kids.Contains(col.gameObject))
                {
                    k.Group.Kids.Remove(col.gameObject);
                }
                col.gameObject.layer = 8;
                AudioSource.PlayClipAtPoint(WedgieSound, transform.position);
                GameObject.Find("Controller").GetComponent<Controller>().Wedgies++;

            }
            else
            {
                // This kid loses
                Wedgied = true;
                if (Group.Kids.Contains(gameObject))
                {
                    Group.Kids.Remove(gameObject);
                }
                gameObject.layer = 8;
            }

            

        }
    }

    void UpdateAnimation()
    {
        // Update Animation
        if (Wedgied)
        {
            _anim.SetBool("Wedgied", Wedgied);
        }
        else
        {
            _anim.SetFloat("Direction", rigidbody2D.velocity.x);
        }

        renderer.sortingOrder = (int)(100 * -transform.position.y);
    }

    void ResetAll()
    {
        // Reset all variables
        _separation     = Vector2.zero;
        _alignment      = Vector2.zero;
        _cohesion       = Vector2.zero;
        _intialVelocity = Vector2.zero;

        // Reset actual velocity
        rigidbody2D.velocity = Vector2.zero;
    }

    void FlockMovement()
    {
        GetNeighbors();

        _cohesion   = Vector2.zero;
        _separation = Vector2.zero;
        _alignment  = Vector2.zero;
        _seek       = Vector2.zero;

        foreach (var n in _neighbors)
        {
            // Separation
            if ((transform.position - n.transform.position).magnitude < SeprationRadius)
                _separation = _separation + (Vector2)(transform.position - n.transform.position);;
        }

        // Cohesion
        _cohesion = (_cohesion - (Vector2)transform.position).normalized * CohesionWeight;

        // Separation
        _separation = _separation.normalized * SeparationWeight;

        // Alignment
        _seek = ((Vector2) Group.transform.position - (Vector2) transform.position);
        _seek = (_seek.magnitude < 0.2) ? Vector2.zero : _seek.normalized*_seek.magnitude*SeekWeight;

        var TotalForce = _cohesion + _separation + _seek;
        rigidbody2D.velocity = TotalForce;
    }

    void WanderMovement()
    {
        _wanderTimer = Mathf.Max(0, _wanderTimer - Time.deltaTime);
        if (_wanderTimer <= 0f)
        {
            _wanderTimer = Random.Range(3f, 6f);
            _alignment   = new Vector2(Random.Range(-1F, 1F), Random.Range(-1F, 1F)).normalized;
        }
        rigidbody2D.velocity = _alignment * MaxSpeed * 0.5f;
    }

    void GetNeighbors()
    {
        _neighbors.Clear();
        foreach (var kid in Group.Kids)
        {
            if ((kid.transform.position - transform.position).magnitude < NeighborRadius)
            {
                _neighbors.Add(kid);
            }
        }
    }
}