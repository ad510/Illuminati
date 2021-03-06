﻿using UnityEngine;
using System.Collections;
using System;
using Assets.Player;
using Assets;

public abstract class Highlightable : DependencyResolvingComponent {
	
	public Sprite normalSprite, highlightSprite, ownedNormalSprite, ownedHighlightSprite;

	private SpriteRenderer spriteRenderer;
	private int highlightRequests = 0;

    private Sprite curNormal, curHighlight;

	// Use this for initialization
	protected virtual void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		VisibilityController.instance.VisibilityChanged += new VisibilityController.VisibilityChangeHandler(updateVisibility);
		updateSprites();
	}

    public event System.Action OnClicked;
    public event System.Action OnHover;
    public event System.Action OnEndHover;

    void OnMouseUpAsButton() {
        if (OnClicked != null) OnClicked();
    }

    void OnMouseEnter() {
        if (OnHover != null) OnHover();
    }

    void OnMouseExit() {
        if (OnEndHover != null) OnEndHover();
    }

	public void setNormalSprite(Sprite newNormal) {
        curNormal = newNormal;
        updateHighlight();
	}

	public void setHighlightedSprite(Sprite newHighlight) {
		curHighlight = newHighlight;
        updateHighlight();
	}
	
	public void setHighlighted(bool isHighlighted) {
        if (isHighlighted) highlightRequests++; else highlightRequests--;
        if (highlightRequests < 0) highlightRequests = 0;
        updateHighlight();
	}

    private void updateHighlight() {
        spriteRenderer.sprite = highlightRequests > 0 ? curHighlight : curNormal;
    }
	
	public void updateSprites() {
		updateVisibility(VisibilityController.instance.visibility);
	}
	
	private void updateVisibility(VisibilityController.Visibility vis) {
		bool owned = viewAsOwned(vis);
		setNormalSprite(owned ? ownedNormalSprite : normalSprite);
		setHighlightedSprite(owned ? ownedHighlightSprite : highlightSprite);
    }

    public abstract bool viewAsOwned(VisibilityController.Visibility visibility);

}
