using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ContainerCounter : BaseCounter {
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player) {
        if (!player.HasKitchenObject() && !player.IsHoldingAndAnimated()) {
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            player.SetKitchenObject(player.GetKitchenObject()); // Chama SetKitchenObject no Player para iniciar a animação e o travamento
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }
}
