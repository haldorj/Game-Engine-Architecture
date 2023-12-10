

using UnityEngine;

public class GreenState : IState
{
    private readonly GameObject _light;
    public GreenState(GameObject gameObject)
    {
        _light.GetComponent<Renderer>().material.color = Color.green;
    }
    public void Enter()
    {
        // code that runs when we first enter the state
    }
    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
    }
    public void Exit()
    {
        // code that runs when we exit the state
    }
}