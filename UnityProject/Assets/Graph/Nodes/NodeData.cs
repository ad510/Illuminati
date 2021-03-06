﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Player;
using System;

public class NodeData : Targetable {

    private static System.Random gen;

    public NodeType type;
    public string archetype;
    public bool isScheduled = false;
    public int power;
	public PlayerData startingOwner;
	public bool isStartNode = false;
    public bool isSecondaryStartNode = false;

    public PlayerData Owner { get; set; }

    private List<TemporaryIncrease> increases;

	// Used for freezing the node for a certain number of turns after performing an action
	public int nTurnsUntilAvailable = 0;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		Owner = startingOwner;
		TurnController.OnTurnStart += onTurnStart;

        // Randomize power
        if (gen == null) gen = new System.Random();
        double range = power / 6;
        double diff = (gen.NextDouble() * range) - (range / 2.0);
        power += (int)Math.Round(diff);

        // Init temporary increases
        increases = new List<TemporaryIncrease>();
	}

    public void temporaryIncrease(int amount, int nTurns) {
        if (nTurns <= 0) {
            return;
        }
        TemporaryIncrease inc = new TemporaryIncrease();
        inc.amount = amount;
        inc.nTurns = nTurns;
        increases.Add(inc);
    }

    public int getWorkingPower() {
        int val = power;
        foreach (TemporaryIncrease inc in increases) {
            val += inc.amount;
        }
        return val;
    }

	public void onTurnStart() {
        if (TurnController.CurrentPlayer != Owner) {
			// Only decrement on your own turn
			return;
		}

        // Decrement node freeze
		if (nTurnsUntilAvailable > 0) {
			nTurnsUntilAvailable -= 1;
		}

        // Decrement temporary power increases
        List<TemporaryIncrease> toRemove = new List<TemporaryIncrease>();
        foreach (TemporaryIncrease inc in increases) {
            inc.nTurns -= 1;
            if (inc.nTurns <= 0) {
                toRemove.Add(inc);
            }
        }

        // Remove finished temporary power increases
        foreach (TemporaryIncrease inc in toRemove) {
            increases.Remove(inc);
        }
	}

    private class TemporaryIncrease {
        public int amount;
        public int nTurns;
    }
}
