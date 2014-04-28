﻿using UnityEngine;
using System.Collections;
using Assets.Player;

public class EdgeData : Targetable {

    public float minVisibilityIncrease, maxVisibilityIncrease;

	public GameObject nodeOne;
	public GameObject nodeTwo;

	public GameObject arrowHead;
	private GameObject realArrowHead;

	public DominationType type {get; set;}
	public EdgeDirection direction { get; set; }
	private EdgeDirection prevDirection;

	private GUIStyle visibilityStyle;

    public float Visibility { get; set; }
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
		direction = EdgeDirection.Neutral;
		prevDirection = EdgeDirection.Neutral;
		type = DominationType.Bribe;
        Visibility = 0;

        TurnController.instance.OnTurnEnd += () => Visibility *= 0.9f;
		TurnController.instance.OnTurnEnd += () => triggerEdge(0.1f);
		TurnController.instance.OnTurnEnd += new TurnController.OnTurnEndHandler(updateVisibilityRendering);
		VisibilityController.instance.VisibilityChanged += new VisibilityController.VisibilityChangeHandler(updateArrowHead);

		visibilityStyle = new GUIStyle();
		visibilityStyle.normal.textColor = Color.black;
		visibilityStyle.fontSize = 14;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public enum EdgeDirection {
		OneToTwo, TwoToOne, Neutral, Unusable
	};

    public void triggerEdge(float baseTriggerProbability)
    {
        float rand = 1.0f;
        while (rand == 1.0f)
        {
            rand = Random.value;
        }

        if (rand < baseTriggerProbability)
            triggerEdge();
    }

    private void triggerEdge()
    {
        float rand = Random.value;
        float visibilityDelta = rand * (maxVisibilityIncrease - minVisibilityIncrease) + minVisibilityIncrease;

        Visibility += visibilityDelta;
    }

    private bool displayVisibility = false;

    void OnMouseEnter()
    {
        displayVisibility = true;
    }

    void OnMouseExit()
    {
        displayVisibility = false;
    }

    void OnGUI()
    {
        if (displayVisibility) {
			float margin = 0.007f * Screen.height;
			GUI.Label(new Rect(margin, margin, 100, 20), "Visibility: " + (int)(Visibility * 100) + "%", visibilityStyle);
		}
    }
	
	override public bool viewAsOwned(VisibilityController.Visibility vis) {
		bool isPrivate = vis == VisibilityController.Visibility.Private;
		bool isOwned = direction != EdgeDirection.Neutral;

		if (isPrivate && isOwned) {
			return nodeOne.GetComponent<NodeData>().Owner == TurnController.instance.CurrentPlayer;
		}

		return false;
	}

	private void updateArrowHead(VisibilityController.Visibility vis) {
		if (realArrowHead == null) {
			realArrowHead = (GameObject) Instantiate(arrowHead, transform.position, Quaternion.identity);

			Vector2 nodeOnePosition = new Vector2(nodeOne.transform.position.x, nodeOne.transform.position.y);
			Vector2 nodeTwoPosition = new Vector2(nodeTwo.transform.position.x, nodeTwo.transform.position.y);
			Vector2 edgeVector = nodeTwoPosition - nodeOnePosition;
			float edgeAngle = Vector2.Angle(Vector2.right, edgeVector);
			if (Vector3.Cross(Vector2.right, edgeVector).z < 0)
			{
				edgeAngle *= -1;
			}
			realArrowHead.transform.Rotate(Vector3.forward, edgeAngle);
		}

		realArrowHead.SetActive(viewAsOwned(vis));

		EdgeDirection simplePrev = (prevDirection == EdgeDirection.TwoToOne ? EdgeDirection.TwoToOne : EdgeDirection.OneToTwo);
		EdgeDirection simpleDir = (direction == EdgeDirection.TwoToOne ? EdgeDirection.TwoToOne : EdgeDirection.OneToTwo);
		if (simplePrev != simpleDir) {
			realArrowHead.transform.Rotate(Vector3.forward, 180);
			prevDirection = direction;
		}

		updateVisibilityRendering();
	}

	private void updateVisibilityRendering() {
		if (VisibilityController.instance.visibility == VisibilityController.Visibility.Public) {
			this.GetComponent<SpriteRenderer>().color = new Color(1, 1.0f - Visibility, 1.0f - Visibility);
		} else {
			this.GetComponent<SpriteRenderer>().color = Color.white;
		}
	}
}
