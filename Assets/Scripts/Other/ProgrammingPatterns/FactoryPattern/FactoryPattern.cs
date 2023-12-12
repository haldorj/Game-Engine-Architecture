using UnityEngine;

public interface IProduct
{
    public string ProductName { get; set; }
    public void Initialize();
}
public abstract class Factory : MonoBehaviour
{
    public abstract IProduct GetProduct(Vector3 position);
    
    // shared method with all factories
}