﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SabotageAction : Action {

    private static System.Random gen;

	public const int DURATION = 2;

	void Start () {
		IsTargeting = true;
        if (gen == null) gen = new System.Random();
	}
	
	public override List<Targetable> getPossibleTargets() {
		NodeData thisNode = getNode();
		List<NodeData> nodes = GraphUtility.getConnectedNodes(thisNode);
		
		List<Targetable> targets = new List<Targetable>();
		foreach (NodeData node in nodes) {
			targets.Add(node);
		}
		return targets;
	}
	
	public override string getAdditionalTextForTarget(Targetable target) {
		NodeData thisNode = getNode();
		
		int decrease = getDecrease(thisNode.getWorkingPower());
        if (decrease > 0) {
            return "-" + decrease + " (" + DURATION + " turns)";
        } else {
            return "--";
        }
	}
	
	protected override void doActivate(Targetable target) {
		NodeData otherNode = (NodeData) target;
		NodeData thisNode = getNode();
        doDecrease(thisNode, otherNode, 1.0f);
        putOnCooldown(DURATION);
	}

    public static void doDecrease(NodeData performer, NodeData target, float multiplier) {
        int decrease = getDecrease(performer.getWorkingPower());
        target.temporaryIncrease(-1 * (int) (decrease * multiplier), DURATION);
    }

	public static int getDecrease(int value) {
		return (int) (value / 4.0f);
	}

}
