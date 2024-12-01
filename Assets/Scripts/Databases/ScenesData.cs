using UnityEngine;

[CreateAssetMenu(fileName = "NewScenesData", menuName = "Databases/Scene")]
public class SceneData : ScriptableObject 
{
    [Header("Scenario settings")] 
    [SerializeField] private string _sceneName = "Scene";
    [SerializeField] private string _sceneFilename = "scene";

    public string SceneNameToUser { get { return _sceneName;} }
    public string SceneName { get { return _sceneFilename;} }
}
