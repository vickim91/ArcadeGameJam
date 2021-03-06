﻿using UnityEngine;
using System.Collections;

public class ScrollingTexture : MonoBehaviour
{
	public float scrollSpeed = 0.9f;
	public float scrollSpeed2 = 0.9f;
    public Texture2D[] textures;
    public float scaleX;
    public float scaleY;
    public Color setColor;


    Renderer rend;
    Vector2 newOffset;

	void Start()
	{
		rend = GetComponent<Renderer> ();
        rend.material.mainTextureScale = new Vector2(scaleX, scaleY);
        rend.material.color = setColor;
        


   
	}

    public void setTexture(int index)
    {
        if(index < textures.Length)
        rend.material.mainTexture = textures[index];
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