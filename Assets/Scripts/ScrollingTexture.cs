﻿using UnityEngine;
using System.Collections;

public class ScrollingTexture : MonoBehaviour
{
	public float scrollSpeed = 0.9f;
	public float scrollSpeed2 = 0.9f;

	Renderer rend;
    Vector2 newOffset;

	void Start()
	{
		rend = GetComponent<Renderer> ();
	}

	void FixedUpdate ()
	{
		float offset = Time.time * scrollSpeed;
		float offset2 = Time.time * scrollSpeed2;

		rend.material.mainTextureOffset = new Vector2 (offset2, -offset);
        newOffset = rend.material.mainTextureOffset;
        rend.material.SetTextureOffset("_BumpMap", newOffset);
    }
}