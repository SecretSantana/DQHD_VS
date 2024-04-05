using UnityEngine;

public class Main : MonoBehaviour
{
    public DataSource DataSource;
    public StaticResources StaticResources;
    public DynamicResources DynamicResources;
    public AppController AppController;

    private void Start()
    {
        AppController.Initialize(DataSource, StaticResources, DynamicResources);
    }
}
