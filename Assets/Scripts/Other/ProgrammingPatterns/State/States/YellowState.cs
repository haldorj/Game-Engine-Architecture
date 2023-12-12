using UnityEngine;
public class YellowState : IState
{
    private readonly GameObject _light;
    
    public YellowState(GameObject gameObject)
    {
        _light = gameObject;
    }
    public void Enter()
    {
        _light.GetComponent<Renderer>().material.color = Color.yellow;
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