using UnityEngine;
using UnityEngine.UI;
using ZeroGravity.Data;

namespace ZeroGravity.UI
{
	public class ControlsSettings : MonoBehaviour
	{
		public ControlsRebinder ControlsRebind;

		public ControlsSettingsData ControlsData = new ControlsSettingsData();

		public Text mouseSensitivityLevel;

		public Slider mouseSensitivitySlider;

		public Toggle InvertMouseToggle;

		public Toggle InvertMouseWhileDrivingToggle;

		private void Start()
		{
			mouseSensitivitySlider.minValue = 0.1f;
			mouseSensitivitySlider.maxValue = 10f;
			mouseSensitivitySlider.value = InputManager.MouseSensitivity;
			mouseSensitivityLevel.text = mouseSensitivitySlider.value.ToString("0.0");
			mouseSensitivitySlider.onValueChanged.AddListener(MouseSensitivity);
			InvertMouseToggle.onValueChanged.AddListener(delegate
			{
				InvertMouseSetter();
			});
			InvertMouseWhileDrivingToggle.onValueChanged.AddListener(delegate
			{
				InvertMouseWhileDrivingSetter();
			});
		}

		public void MouseSensitivity(float val)
		{
			mouseSensitivityLevel.text = val.ToString("0.0");
			InputManager.MouseSensitivity = val;
			ControlsData.MouseSensitivity = val;
			mouseSensitivitySlider.value = val;
		}

		public void InvertMouseSetter()
		{
			Client.Instance.InvertedMouse = InvertMouseToggle.isOn;
			ControlsData.InvertMouse = Client.Instance.InvertedMouse;
		}

		public void InvertMouseWhileDrivingSetter()
		{
			Client.Instance.InvertMouseWhileDriving = InvertMouseWhileDrivingToggle.isOn;
			ControlsData.InverMouseWhileDriving = Client.Instance.InvertMouseWhileDriving;
		}

		public void SetDefault()
		{
			ControlsData.MouseSensitivity = 1f;
			MouseSensitivity(ControlsData.MouseSensitivity);
			ControlsSettingsData controlsData = ControlsData;
			bool flag = false;
			InvertMouseToggle.isOn = flag;
			controlsData.InvertMouse = flag;
			ControlsSettingsData controlsData2 = ControlsData;
			flag = false;
			InvertMouseWhileDrivingToggle.isOn = flag;
			controlsData2.InverMouseWhileDriving = flag;
			InputManager.LoadDefaultJSON();
		}

		public void SaveControlsSettings()
		{
			ControlsData.MouseSensitivity = InputManager.MouseSensitivity;
			ControlsData.InvertMouse = Client.Instance.InvertedMouse;
			ControlsData.InverMouseWhileDriving = Client.Instance.InvertMouseWhileDriving;
			if (!ControlsRebind.CheckIfEmpty())
			{
				ControlsData.InputConfigurations = Client.Instance.InputManager.GetSaveData();
			}
		}

		public void Load(ControlsSettingsData controlsSettings)
		{
			if (controlsSettings == null)
			{
				SetDefault();
				return;
			}
			ControlsData = controlsSettings;
			MouseSensitivity(ControlsData.MouseSensitivity);
			InvertMouseToggle.isOn = ControlsData.InvertMouse;
			InvertMouseWhileDrivingToggle.isOn = ControlsData.InverMouseWhileDriving;
			InputManager.MouseSensitivity = ControlsData.MouseSensitivity;
			Client.Instance.InvertedMouse = ControlsData.InvertMouse;
			Client.Instance.InvertMouseWhileDriving = ControlsData.InverMouseWhileDriving;
			Client.Instance.InputManager.SetSaveData(ControlsData.InputConfigurations);
		}
	}
}
