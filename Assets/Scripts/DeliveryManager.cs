using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour {
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeListSO;
    [SerializeField] private float recipeExpiryTimeMax = 60f; // Tempo máximo para um pedido expirar
    [SerializeField] private int scorePerRecipe = 100;
    private float bonusMultiplier = 1f;
    [SerializeField] private float bonusMultiplierIncrease = 0.2f; // Quanto o multiplicador aumenta
    [SerializeField] private float bonusMultiplierResetTime = 5f; // Tempo para o multiplicador resetar se não houver entrega
    private float lastRecipeDeliveredTime;

    private List<WaitingRecipe> waitingRecipeList; // Usaremos uma classe para armazenar o RecipeSO e o tempo restante
    private float spawRecipeTimer;
    private float spawRecipeTimerMax = 2f;
    private int waitingRecipesMax = 5;
    private int playerScore;

    private class WaitingRecipe {
        public RecipeSO recipeSO;
        public float expiryTimer;

        public WaitingRecipe(RecipeSO recipeSO, float expiryTimer) {
            this.recipeSO = recipeSO;
            this.expiryTimer = expiryTimer;
        }
    }

    private void Awake() {
        Instance = this;
        waitingRecipeList = new List<WaitingRecipe>();
    }

    private void Update() {
        spawRecipeTimer -= Time.deltaTime;
        if (spawRecipeTimer <= 0f) {
            spawRecipeTimer = spawRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeList.Count < waitingRecipesMax) {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)];
                waitingRecipeList.Add(new WaitingRecipe(waitingRecipeSO, recipeExpiryTimeMax));
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }

        // Atualizar o timer dos pedidos existentes e remover os expirados
        for (int i = waitingRecipeList.Count - 1; i >= 0; i--) {
            waitingRecipeList[i].expiryTimer -= Time.deltaTime;
            if (waitingRecipeList[i].expiryTimer <= 0f) {
                // Pedido expirou!
                waitingRecipeList.RemoveAt(i);
                OnRecipeFailed?.Invoke(this, EventArgs.Empty); // Você pode querer um evento específico de expiração
                OnRecipeCompleted?.Invoke(this, EventArgs.Empty); // Para atualizar a UI
                bonusMultiplier = 1f; // Resetar o multiplicador se um pedido expirar
            }
        }

        // Resetar o multiplicador se não houver entrega por um certo tempo
        if (Time.time - lastRecipeDeliveredTime > bonusMultiplierResetTime) {
            bonusMultiplier = 1f;
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        bool recipeDelivered = false; // Flag para verificar se algum pedido foi entregue

        for (int i = 0; i < waitingRecipeList.Count; i++) {
            WaitingRecipe waitingRecipe = waitingRecipeList[i];
            RecipeSO waitingRecipeSO = waitingRecipe.recipeSO;

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                        if (plateKitchenObjectSO == recipeKitchenObjectSO) {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound) {
                        plateContentsMatchesRecipe = false;
                        break;
                    }
                }

                if (plateContentsMatchesRecipe) {
                    // Player delivered the correct recipe!
                    playerScore += Mathf.RoundToInt(scorePerRecipe * bonusMultiplier);
                    // Manter para outros usos, se necessário
                    waitingRecipeList.RemoveAt(i);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);

                    bonusMultiplier += bonusMultiplierIncrease;
                    lastRecipeDeliveredTime = Time.time;
                    recipeDelivered = true;
                    return;
                }
            }
        }
        // No matches found!
        //Player did not deliver a correct recipe
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);

        if (!recipeDelivered) {
            bonusMultiplier = 1f; // Resetar o multiplicador se a entrega falhar
        }
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        // Retorna apenas o RecipeSO para a UI (o timer fica interno)
        List<RecipeSO> recipeSOList = new List<RecipeSO>();
        foreach (WaitingRecipe waitingRecipe in waitingRecipeList) {
            recipeSOList.Add(waitingRecipe.recipeSO);
        }
        return recipeSOList;
    }

    public float GetRecipeExpiryTimerNormalized(RecipeSO recipeSO) {
        foreach (WaitingRecipe waitingRecipe in waitingRecipeList) {
            if (waitingRecipe.recipeSO == recipeSO) {
                return waitingRecipe.expiryTimer / recipeExpiryTimeMax;
            }
        }
        return 0f; // Se não encontrar, retorna 0
    }

    public int GetPlayerScore() {
        return playerScore;
    }
}