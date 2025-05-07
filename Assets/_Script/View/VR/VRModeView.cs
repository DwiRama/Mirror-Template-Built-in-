using UnityEngine;
using UnityEngine.UI;

public class VRModeView : MonoBehaviour
{
    [SerializeField] Toggle vrModeToggle;

	private void Start()
	{
		PlayerDataHandler.Instance.vrMode = vrModeToggle.isOn;
		vrModeToggle.onValueChanged.AddListener(OnValueChanged);
	}

	private void OnDestroy()
	{
		vrModeToggle.onValueChanged.RemoveListener(OnValueChanged);
	}

	public void OnValueChanged(bool value)
    {
		PlayerDataHandler.Instance.vrMode = value;
    }
}
