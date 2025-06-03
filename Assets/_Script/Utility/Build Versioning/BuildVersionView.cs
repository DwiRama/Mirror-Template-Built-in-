using UnityEngine;
using TMPro;

public class BuildVersionView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buildVersionText;

    private void Awake()
    {
        SetBuildVersion();
    }

    public void SetBuildVersion()
    {
        buildVersionText.text = $"Version: {Application.version}";
        Debug.Log($"Build Version: {Application.version}");
    }
}
