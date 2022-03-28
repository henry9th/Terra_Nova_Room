using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetItem : MonoBehaviour
{
    public int id; 
    public string title; 
    public string description;
    public string owner;
    public Sprite icon;

    public AssetItem(int id, string title, string owner, string description, string iconPath) { 
        this.id = id; 
        this.title = title;
        this.owner = owner;
        this.description = description;
        this.icon = Resources.Load<Sprite>(iconPath);
    }
}
