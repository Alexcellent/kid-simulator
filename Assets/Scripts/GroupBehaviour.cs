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

public class GroupBehaviour : MonoBehaviour
{
    //////////////////////////////////////////////
    // GROUP DATA
    public int              LayerID; 
    public List<GameObject> Kids;
    public float            TargetRadius;
    public float            SeekSpeed;
    public float            FleeSpeed;
    public float            WndrSpeed;
    
    /////////////////////////////////////////////
    // MOVEMENT
    GroupBehaviour _target;
    Vector2        _direction;
    float          _wanderTimer;

    void Start()
    {
    }

    void Update()
    {
        UpdateTarget();
        UpdateMovement();
    }

    void UpdateTarget()
    {
        var Groups  = GameObject.Find("Controller").GetComponent<Controller>().Groups;
        var minDist = float.MaxValue;
        _target     = null;
        foreach (var g in Groups)
        {
            if (g != this)
            {
                if ((g.Kids.Count > 0) && 
                    (g.Kids.Count != Kids.Count) && 
                    (g.transform.position - transform.position).magnitude < minDist)
                {
                    _target = g;
                } 
            }
        }
    }

    void UpdateMovement()
    {
        if (!GameObject.Find("Controller").GetComponent<Controller>().GameStarted) return;

        if (_target && (_target.transform.position - transform.position).magnitude < TargetRadius)
        {
            if (_target.Kids.Count > Kids.Count)
            {
                Flee();
            }
            else
            { 
                Seek();
            }
        }
        else
        {
            Wander();
        }
    }

    void Seek()
    {
        var agro   = Mathf.Abs(Kids.Count - _target.Kids.Count) / 2f; // TODO: Play around with this
        _direction = _target.transform.position - transform.position;
        rigidbody2D.velocity = (_direction.normalized) * agro *  SeekSpeed;
        Debug.Log("Seek" + LayerID);
    }

    void Flee()
    {
        var scare =  Mathf.Abs(Kids.Count - _target.Kids.Count) / 2f; // TODO: Play around with this
        _direction = transform.position - _target.transform.position;
        rigidbody2D.velocity = (_direction.normalized) * scare * FleeSpeed;
        Debug.Log("Flee" + LayerID);

    }

    void Wander()
    {
        _wanderTimer = Mathf.Max(0, _wanderTimer - Time.deltaTime);
        if (_wanderTimer <= 0f)
        {
            _wanderTimer = Random.Range(3f, 6f);
            _direction   = (new Vector2(Random.Range(-2F, 2F), Random.Range(-1F, 0.25F)).normalized) - (Vector2)transform.position;
        }
        rigidbody2D.velocity = (_direction.normalized) * WndrSpeed;
        Debug.Log("Wander" + LayerID);

    }

    public void AssignCollisionLayers(bool ignore = false)
    {
        if (!ignore) // Assign each to its group's layer
        {
            foreach (var k in Kids)
            {
                k.layer = LayerID;
            }
        }
        else // Assign each back to the ignore layer
        {
            foreach (var k in Kids)
            {
                k.layer = 8;
            }
        }
    }
}
