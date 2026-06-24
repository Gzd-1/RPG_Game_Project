using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PickableObject : InteractableObject
{
    public ItemSO itemSO;

    protected override void Interact()
    {
        Destroy(this.gameObject);
        InventoryManager.Instance.AddItem(itemSO);
    }
}
