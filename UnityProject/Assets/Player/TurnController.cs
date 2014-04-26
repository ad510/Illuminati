﻿using UnityEngine;
using System.Collections;
using System;

namespace Assets.Player
{
    public class TurnController : MonoBehaviour
    {

        public PlayerData CurrentPlayer;
        public PlayerData OtherPlayer;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //if the end turn key has been pressed
            //check whether the turn can be ended
            //and end the turn if so
        }

        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 100, 90), "End Turn"))
            {
                NextTurn();
            }

            GUI.TextArea(new Rect(10, 110, 100, 90), CurrentPlayer.Name);
        }

        private void NextTurn()
        {
            //end the current player's turn
            CurrentPlayer.endTurn();

            //set visibility ot public information only
            this.GetComponent<VisibilityController>().setVisibility(VisibilityController.Visibility.Public);

            //start the next player's turn
            OtherPlayer.startTurn();

            //announce the turn change ('Player 2's turn' pops up on the screen or something)

            PlayerData temp = OtherPlayer;
            OtherPlayer = CurrentPlayer;
            CurrentPlayer = temp;
        }
    }
}
    