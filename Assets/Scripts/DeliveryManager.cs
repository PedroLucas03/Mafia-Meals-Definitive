using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour {
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted; // Disparado para qualquer pedido concluído (certo ou errado)
    public event EventHandler OnRecipeSuccess;   // Disparado para pedidos corretos
    public event EventHandler OnRecipeFailed;    // Disparado para pedidos incorretos ou expirados
    public event EventHandler OnBonusMultiplierChanged;

    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeListSO;
    [SerializeField] private float recipeExpiryTimeMax = 60f;
    [SerializeField] private int scorePerRecipe = 100;
    [SerializeField] private int scorePenaltyPerFailedRecipe = 50;

    private float bonusMultiplier = 1f;
    [SerializeField] private float bonusMultiplierIncrease = 0.2f;
    [SerializeField] public float bonusMultiplierResetTime = 5f;
    private float lastRecipeDeliveredTime;

    private List<WaitingRecipe> waitingRecipeList;
    private float spawRecipeTimer;
    private float spawRecipeTimerMax = 2f;
    private int waitingRecipesMax = 5;
    private int playerScore;

    // NOVO: Contadores para receitas
    private int recipesCompletedSuccessfully;
    private int recipesFailed;

    private float currentBonusResetTimer;

    public bool IsBonusActive => bonusMultiplier > 1f;
    public float GetCurrentBonusResetTimer() => currentBonusResetTimer;
    public float GetCurrentBonusMultiplier() => bonusMultiplier;

    // NOVO: Métodos para obter as contagens
    public int GetRecipesCompletedSuccessfully() => recipesCompletedSuccessfully;
    public int GetRecipesFailed() => recipesFailed;


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
        lastRecipeDeliveredTime = Time.time;
        currentBonusResetTimer = bonusMultiplierResetTime;
        // Inicializa os contadores
        recipesCompletedSuccessfully = 0;
        recipesFailed = 0;
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

        for (int i = waitingRecipeList.Count - 1; i >= 0; i--) {
            waitingRecipeList[i].expiryTimer -= Time.deltaTime;
            if (waitingRecipeList[i].expiryTimer <= 0f) {
                // Pedido expirou!
                waitingRecipeList.RemoveAt(i);

                playerScore -= scorePenaltyPerFailedRecipe;
                if (playerScore < 0) playerScore = 0;

                recipesFailed++; // Incrementa o contador de receitas falhas

                OnRecipeFailed?.Invoke(this, EventArgs.Empty);
                OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                SetBonusMultiplier(1f);
            }
        }

        if (IsBonusActive) {
            currentBonusResetTimer -= Time.deltaTime;
            if (currentBonusResetTimer <= 0f) {
                SetBonusMultiplier(1f);
            }
            OnBonusMultiplierChanged?.Invoke(this, EventArgs.Empty);
        }
        else {
            currentBonusResetTimer = bonusMultiplierResetTime;
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        bool recipeFoundAndDelivered = false;

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
                    // Receita correta entregue!
                    playerScore += Mathf.RoundToInt(scorePerRecipe * bonusMultiplier);
                    recipesCompletedSuccessfully++; // Incrementa o contador de receitas bem-sucedidas

                    waitingRecipeList.RemoveAt(i);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);

                    SetBonusMultiplier(bonusMultiplier + bonusMultiplierIncrease);
                    lastRecipeDeliveredTime = Time.time;
                    currentBonusResetTimer = bonusMultiplierResetTime;

                    recipeFoundAndDelivered = true;
                    return;
                }
            }
        }

        // Se nenhuma receita correta foi encontrada
        playerScore -= scorePenaltyPerFailedRecipe;
        if (playerScore < 0) playerScore = 0;

        recipesFailed++; // Incrementa o contador de receitas falhas por entrega incorreta

        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
        SetBonusMultiplier(1f);
    }

    private void SetBonusMultiplier(float newMultiplier) {
        bool wasActive = bonusMultiplier > 1f;

        if (newMultiplier < 1f) newMultiplier = 1f;
        if (bonusMultiplier != newMultiplier) {
            bonusMultiplier = newMultiplier;
            OnBonusMultiplierChanged?.Invoke(this, EventArgs.Empty);
            Debug.Log($"Multiplicador de Bônus: {bonusMultiplier:F1}x");
        }
        else if (wasActive && newMultiplier == 1f) {
            OnBonusMultiplierChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
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
        return 0f;
    }

    public int GetPlayerScore() {
        return playerScore;
    }
}