using UnityEngine;
public class RedState : IState
{
    private readonly GameObject _light;
    public RedState(GameObject gameObject)
    {
        _light = gameObject;
    }
    public void Enter()
    {
        _light.GetComponent<Renderer>().material.color = Color.red;
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