using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private string gameNameLevel;
    [SerializeField] private GameObject MenuInicial;
    [SerializeField] private GameObject Opcoes;
    [SerializeField] private GameObject Creditos;

    public void Play()
    {
        SceneManager.LoadScene("KitchenTutorial");
    }

    public void Options()
    {
        MenuInicial.SetActive(false);
        Opcoes.SetActive(true);

    }

    public void CloseOptions()
    {
        Opcoes.SetActive(false);
        MenuInicial.SetActive(true);
    }

    public void Credits()

    {
        MenuInicial.SetActive(false);
        Creditos.SetActive(true);
      
    }

    public void CloseCredits()

    {
        Creditos.SetActive(false);
        MenuInicial.SetActive(true);

    }

    public void LeaveGame()
    {
        Application.Quit();
    }
}