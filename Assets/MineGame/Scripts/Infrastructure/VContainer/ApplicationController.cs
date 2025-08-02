using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

/// <summary>
/// контролер VContainer
/// </summary>
public class ApplicationController : LifetimeScope
{
    [HideInInspector] public static InputPlayer inputPlayer;

    protected override void Configure(IContainerBuilder builder)
    {
        inputPlayer = new();

        Application.targetFrameRate = 60;
        base.Configure(builder);

        RegisterControllers(builder);
        RegisterDatabases(builder);
        RegisterViews(builder);
    }
    private void OnEnable()
    {
        inputPlayer.Enable();
    }
    private void OnDisable()
    {
        inputPlayer.Disable();
    }
    private static void RegisterControllers(IContainerBuilder builder)
    {
    }

    private void RegisterDatabases(IContainerBuilder builder)
    {
    }

    private void RegisterViews(IContainerBuilder builder)
    {
    }
}
