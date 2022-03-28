using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetDatabase : MonoBehaviour
{
    public List<AssetItem> items = new List<AssetItem>();

    private void Awake() { 
        BuildDatabase();
    }

    public AssetItem GetItem(int id) { 
        return items.Find(item => item.id == id);
    }

    public List<AssetItem> GetOwnerItems(string owner) { 
        return items.FindAll(item => item.owner == owner);
    }

    void BuildDatabase() { 
        
        items = new List<AssetItem>() { 
            new AssetItem(0, "Diamond Sword", "henry", "A sword made with diamond.", "Sprites/Items/Sword_diamond"),
            new AssetItem(0, "Diamond Pick", "henry", "A Pick made with diamond.", "Sprites/Items/Pick_diamond")
        };
    }
}
