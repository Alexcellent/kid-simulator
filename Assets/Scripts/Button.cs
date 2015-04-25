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

namespace Assets.Scripts
{
    public class Button : MonoBehaviour
    {
        public Controller           Controller;
        public Controller.Attribute Choice;


        void Update()
        {
            if (Controller.Assigned) Destroy(gameObject);
        }

        void OnMouseDown()
        {
            Controller.Choice = Choice;
        }
    }
}
