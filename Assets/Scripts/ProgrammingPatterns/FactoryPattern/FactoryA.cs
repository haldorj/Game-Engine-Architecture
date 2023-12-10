using UnityEngine;

public class ProductA : MonoBehaviour, IProduct
{
    [SerializeField] private string productName = "ProductA";
    public string ProductName { get => productName; set => productName
        = value ; }
    private ParticleSystem particleSystem;
    public void Initialize()
    {
        // any unique logic to this product
        gameObject.name = productName;
        particleSystem = GetComponentInChildren<ParticleSystem>();
        particleSystem?.Stop();
        particleSystem?.Play();
    }
}
public class ConcreteFactoryA : Factory
{
    [SerializeField] private ProductA productPrefab;
    public override IProduct GetProduct(Vector3 position)
    {
        // create a Prefab instance and get the product component
        GameObject instance = Instantiate(productPrefab.gameObject,
            position, Quaternion.identity);
        ProductA newProduct = instance.GetComponent<ProductA>();
        // each product contains its own logic
        newProduct.Initialize();
        return newProduct;
    }
}