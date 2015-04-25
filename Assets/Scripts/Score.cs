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
using System.Collections;

namespace Assets.Scripts
{

    public class Score : MonoBehaviour
    {

        public TextMesh Value;

        public void SetLives(int value)
        {
            Value.text = "x " + value.ToString();
        }
    }

}