﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BribeAttackAction : Action {

	// Use this for initialization
	void Start () {
		isTargeting = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	override public List<Highlightable> getPossibleTargets() {
		List<NodeData> nodes = Graph.instance.getNeutralConnectedNodes(this.gameObject.GetComponent<NodeData>());
		List<Highlightable> ret = new List<Highlightable>();
		foreach (NodeData node in nodes) {
			if (this.gameObject.GetComponent<NodeData>().owner != node.owner) {
				ret.Add(node);
			}
		}
		return ret;
	}
	
	override public void Activate() {

	}
}
