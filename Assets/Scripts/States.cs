using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class States : MonoBehaviour
{
    public abstract States RunCurrentState();
}