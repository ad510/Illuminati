﻿using Assets.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Graph.Nodes
{
    //They're too clever to be caught in the act, but maybe we can scare them off...
    //Deactivates an edge for a while
    public class StakeoutAction : Action
    {
        public int NumTurnsTargetDeactivated;
        public float MinimumTargetVisibility;
        public float ProbabilityZeroThreshold;
        public float GuaranteedSuccessThreshold;

        void Reset()
        {
            NumTurnsTargetDeactivated = 5;
            MinimumTargetVisibility = 0.25f;
            ProbabilityZeroThreshold = 0.25f;
            GuaranteedSuccessThreshold = 0.75f;

            PathVisibilityIncreaseScaleParameter = 0.5f;
            CarryingEdgeVisibilityIncreaseScaleParameter = 1;
        }

        void Start()
        {
            IsTargeting = true;
        }

        public override List<Targetable> getPossibleTargets()
        {
            //can target any edge that does not belong to you with sufficiently high visibility
            List<EdgeData> allEdges = new List<EdgeData>(FindObjectsOfType<EdgeData>());

            PlayerData currentPlayer = TurnController.CurrentPlayer;

            IEnumerable<EdgeData> possibleTargetEdges = allEdges.Where<EdgeData>((x) =>
                {
                    if (x.Visibility < MinimumTargetVisibility) return false;
                    if (x.nodeOne.GetComponent<NodeData>().Owner == currentPlayer || x.nodeTwo.GetComponent<NodeData>().Owner == currentPlayer) return false;
                    return true;
                });

            return possibleTargetEdges.Select<EdgeData, Targetable>((x) => (Targetable)x).ToList<Targetable>();
        }

        public override string getAdditionalTextForTarget(Targetable target)
        {
            return (int)(100 * getSuccessProbability(target)) + "%";
        }

        protected override void doActivate(Targetable target)
        {
            EdgeData edge = (EdgeData) target;
            PlayerData currentPlayer = TurnController.CurrentPlayer;

            if (edge.getOwner() == currentPlayer) {
                return;
            }

            float randomValue = 1.0f;
            while(randomValue == 1.0f) randomValue = UnityEngine.Random.value;
            if(randomValue < getSuccessProbability(target))
            {
                //remove all ownership of the edge (and make it unusable)
                edge.direction = EdgeData.EdgeDirection.Unusable;

                //set it back to neutral in a few turns
                System.Action directionResetter = null;
                int numTurnsRemaining = NumTurnsTargetDeactivated;

                directionResetter = () =>
                    {
                        if (TurnController.CurrentPlayer == currentPlayer)
                        {
                            numTurnsRemaining--;
                        }

                        if (numTurnsRemaining == 0)
                        {
                            edge.direction = EdgeData.EdgeDirection.Neutral;
                            TurnController.OnTurnEnd -= directionResetter;
                        }
                    };

                TurnController.OnTurnEnd += directionResetter;

                //TODO possibly reveal the user of the edge, if any?
            }
        }

        public float getSuccessProbability(Targetable target)
        {
            float targetVisibility = ((EdgeData)target).Visibility;

            if (targetVisibility < ProbabilityZeroThreshold) return 0;
            if (targetVisibility >= GuaranteedSuccessThreshold) return 1;
            return (targetVisibility - ProbabilityZeroThreshold) / (GuaranteedSuccessThreshold - ProbabilityZeroThreshold);
        }
    }
}
