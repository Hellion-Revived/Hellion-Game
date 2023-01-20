using System.Collections.Generic;
using Luminosity.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroGravity.UI
{
	public class ControlsRebinder : MonoBehaviour
	{
		public class ButtonListItem
		{
			public GameObject ButtonObject;

			public Button Button;

			public Text ButtonText;

			public bool IsAlt;

			public ControlItem ControlItem;
		}

		public static List<ControlItem> Controls;

		public static List<ControlItem> MovementControls;

		public static List<ControlItem> ShipControls;

		public static List<ControlItem> ActionControls;

		public static List<ControlItem> SuitControls;

		public static List<ControlItem> CommunicationControls;

		public static List<ControlItem> QuickActions;

		public Transform MovementHolder;

		public Transform ShipHolder;

		public Transform ActionHolder;

		public Transform SuitHolder;

		public Transform CommsHolder;

		public Transform QuickHolder;

		public Settings settings;

		public bool isScanning;

		public string WhoIsScanning = string.Empty;

		public List<ButtonListItem> buttonList = new List<ButtonListItem>();

		public GameObject ControlPref;

		public List<string> ControlsList = new List<string>();

		private string InputConfName = "KeyboardAndMouse";

		public InputAction actionsRev;

		private bool isPositiveRev;

		private bool isAltRev;

		public KeyCode oldKeyRev_p;

		public KeyCode oldKeyRev_n;

		public KeyCode oldKeyRev_ap;

		public KeyCode oldKeyRev_an;

		private ControlItem controlItemValRev;

		private InputAction actionsOld;

		private bool isPositiveOld;

		private bool isAltOld;

		public KeyCode oldKeyOld;

		private ControlItem controlItemValOld;

		private RebindInput rebinderRev;

		private RebindInput rebinderOld;

		public Transform ScrollListContent;

		private List<GameObject> AllElements = new List<GameObject>();

		public static void Initialize()
		{
			List<ControlItem> list = new List<ControlItem>();
			list.Add(new ControlItem
			{
				Name = Localization.Forward.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Forward
			});
			list.Add(new ControlItem
			{
				Name = Localization.Backward.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Forward,
				IsPositive = false
			});
			list.Add(new ControlItem
			{
				Name = Localization.Right.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Right
			});
			list.Add(new ControlItem
			{
				Name = Localization.Left.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Right,
				IsPositive = false
			});
			list.Add(new ControlItem
			{
				Name = Localization.RotationClockwise.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Lean
			});
			list.Add(new ControlItem
			{
				Name = Localization.RotationAnticlockwise.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Lean,
				IsPositive = false
			});
			list.Add(new ControlItem
			{
				Name = Localization.Jump.ToUpper() + " / <color='#A0D3F8'>" + Localization.Up.ToUpper() + "</color>",
				Axis = ZeroGravity.UI.InputManager.AxisNames.Space
			});
			list.Add(new ControlItem
			{
				Name = Localization.Crouch.ToUpper() + " / <color='#A0D3F8'>" + Localization.Down.ToUpper() + "</color>",
				Axis = ZeroGravity.UI.InputManager.AxisNames.LeftCtrl
			});
			list.Add(new ControlItem
			{
				Name = Localization.Sprint.ToUpper() + " / <color='#A0D3F8'>" + Localization.Grab.ToUpper() + "</color> / " + Localization.Stabilization.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.LeftShift
			});
			list.Add(new ControlItem
			{
				Name = Localization.FreeLook.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.LeftAlt
			});
			MovementControls = list;
			list = new List<ControlItem>();
			list.Add(new ControlItem
			{
				Name = Localization.PrimaryMouseButton.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Mouse1
			});
			list.Add(new ControlItem
			{
				Name = Localization.SecondaryMouseButton.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Mouse2
			});
			list.Add(new ControlItem
			{
				Name = Localization.ThirdMouseButton.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Mouse3
			});
			list.Add(new ControlItem
			{
				Name = Localization.Inventory.ToUpper() + " / <color='#A0D3F8'>" + Localization.ExitPanel.ToUpper() + "</color>",
				Axis = ZeroGravity.UI.InputManager.AxisNames.Tab
			});
			list.Add(new ControlItem
			{
				Name = Localization.Journal.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.O
			});
			list.Add(new ControlItem
			{
				Name = Localization.InteractTakeInHands.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.F
			});
			list.Add(new ControlItem
			{
				Name = Localization.DropThrow.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.G
			});
			list.Add(new ControlItem
			{
				Name = Localization.EquipItem.ToUpper() + " / <color='#A0D3F8'>" + Localization.Reload.ToUpper() + "</color> / " + Localization.ChangeDockingPort.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.R
			});
			list.Add(new ControlItem
			{
				Name = Localization.ChangeStance.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Z
			});
			list.Add(new ControlItem
			{
				Name = Localization.ToggleLights.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.L
			});
			list.Add(new ControlItem
			{
				Name = Localization.WeaponModKey.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.B
			});
			list.Add(new ControlItem
			{
				Name = Localization.Melee.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.V
			});
			ActionControls = list;
			list = new List<ControlItem>();
			list.Add(new ControlItem
			{
				Name = Localization.EngineToggle.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Enter
			});
			list.Add(new ControlItem
			{
				Name = Localization.EngineThrustUp.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.NumPlus
			});
			list.Add(new ControlItem
			{
				Name = Localization.EngineThrustDown.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.NumMinus
			});
			list.Add(new ControlItem
			{
				Name = Localization.MatchVelocityControl.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.M
			});
			ShipControls = list;
			list = new List<ControlItem>();
			list.Add(new ControlItem
			{
				Name = Localization.ToggleVisor.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.H
			});
			list.Add(new ControlItem
			{
				Name = Localization.ToggleJetpack.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.J
			});
			list.Add(new ControlItem
			{
				Name = Localization.HelmetRadar.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.X
			});
			list.Add(new ControlItem
			{
				Name = Localization.TargetUp.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.UpArrow
			});
			list.Add(new ControlItem
			{
				Name = Localization.TargetDown.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.DownArrow
			});
			list.Add(new ControlItem
			{
				Name = Localization.FilterLeft.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.LeftArrow
			});
			list.Add(new ControlItem
			{
				Name = Localization.FilterRight.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.RightArrow
			});
			SuitControls = list;
			list = new List<ControlItem>();
			list.Add(new ControlItem
			{
				Name = Localization.Chat.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Y
			});
			list.Add(new ControlItem
			{
				Name = Localization.Talk.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.CapsLock
			});
			list.Add(new ControlItem
			{
				Name = Localization.Radio.ToUpper(),
				Axis = ZeroGravity.UI.InputManager.AxisNames.Tilda
			});
			CommunicationControls = list;
			list = new List<ControlItem>();
			list.Add(new ControlItem
			{
				Name = Localization.Quick1,
				Axis = ZeroGravity.UI.InputManager.AxisNames.Alpha1
			});
			list.Add(new ControlItem
			{
				Name = Localization.Quick2,
				Axis = ZeroGravity.UI.InputManager.AxisNames.Alpha2
			});
			list.Add(new ControlItem
			{
				Name = Localization.Quick3,
				Axis = ZeroGravity.UI.InputManager.AxisNames.Alpha3
			});
			list.Add(new ControlItem
			{
				Name = Localization.Quick4,
				Axis = ZeroGravity.UI.InputManager.AxisNames.Alpha4
			});
			QuickActions = list;
		}

		private void Awake()
		{
			Initialize();
			UpdateUI();
		}

		private void Start()
		{
		}

		private void OnEnable()
		{
		}

		public void UpdateUI()
		{
			foreach (GameObject allElement in AllElements)
			{
				Object.Destroy(allElement);
			}
			AllElements.Clear();
			InstantiateKeyboardControls();
		}

		public void EnableAllButtons(bool val)
		{
			foreach (ButtonListItem button in buttonList)
			{
				if (button.ControlItem.CanBeChanged)
				{
					button.Button.interactable = val;
				}
			}
		}

		private void InstantiateKeyboardControls()
		{
			foreach (ControlItem movementControl in MovementControls)
			{
				InstantiateControlPref(movementControl, MovementHolder);
			}
			foreach (ControlItem actionControl in ActionControls)
			{
				InstantiateControlPref(actionControl, ActionHolder);
			}
			foreach (ControlItem shipControl in ShipControls)
			{
				InstantiateControlPref(shipControl, ShipHolder);
			}
			foreach (ControlItem suitControl in SuitControls)
			{
				InstantiateControlPref(suitControl, SuitHolder);
			}
			foreach (ControlItem communicationControl in CommunicationControls)
			{
				InstantiateControlPref(communicationControl, CommsHolder);
			}
			foreach (ControlItem quickAction in QuickActions)
			{
				InstantiateControlPref(quickAction, QuickHolder);
			}
		}

		public void InstantiateControlPref(ControlItem controlName, Transform holder)
		{
			GameObject gameObject = Object.Instantiate(ControlPref, holder);
			AllElements.Add(gameObject);
			gameObject.transform.Find("ControlNameText").GetComponent<Text>().text = controlName.Name;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			RebindInput component = gameObject.transform.Find("Button").GetComponent<RebindInput>();
			GameObject gameObject2 = gameObject.transform.Find("Button").gameObject;
			Button component2 = gameObject.transform.Find("Button").GetComponent<Button>();
			component2.interactable = controlName.CanBeChanged;
			component.SetRebinder(InputConfName, controlName, gameObject2.transform.Find("Text").GetComponent<Text>(), base.gameObject, isAlt: false);
			component2.onClick.AddListener(component.OnButtonPressed);
			buttonList.Add(new ButtonListItem
			{
				Button = component2,
				ButtonObject = gameObject2,
				ButtonText = gameObject2.transform.Find("Text").GetComponent<Text>(),
				IsAlt = false,
				ControlItem = component.ControlItem
			});
			gameObject.transform.name = controlName.Name;
			component = gameObject.transform.Find("AltButton").GetComponent<RebindInput>();
			gameObject2 = gameObject.transform.Find("AltButton").gameObject;
			component2 = gameObject.transform.Find("AltButton").GetComponent<Button>();
			component2.interactable = controlName.CanBeChanged;
			component.SetRebinder(InputConfName, controlName, gameObject2.transform.Find("Text").GetComponent<Text>(), base.gameObject, isAlt: true);
			component2.onClick.AddListener(component.OnButtonPressed);
			buttonList.Add(new ButtonListItem
			{
				Button = component2,
				ButtonObject = gameObject2,
				ButtonText = gameObject2.transform.Find("Text").GetComponent<Text>(),
				IsAlt = true,
				ControlItem = component.ControlItem
			});
		}

		private void SaveControlForRevert(InputAction actions, bool isPositive, bool isAlt, KeyCode oldKey, ControlItem controlItemVal, bool isPositiveR, bool isAltR)
		{
			actionsOld = actions;
			isPositiveOld = isPositive;
			isAltOld = isAlt;
			oldKeyOld = oldKey;
			controlItemValRev = controlItemVal;
			isPositiveRev = isPositiveR;
			isAltRev = isAltR;
		}

		public void DuplicatedControlsYes()
		{
			GameMenu.DisableGameMenu = false;
		}

		public void DuplicateControlsNo()
		{
			for (int i = 0; i < Luminosity.IO.InputManager.PlayerOneControlScheme.Actions.Count; i++)
			{
				if (Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i] == actionsRev)
				{
					if (isPositiveRev && !isAltRev)
					{
						Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[0].Positive = oldKeyRev_p;
					}
					else if (!isPositiveRev && !isAltRev)
					{
						Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[0].Negative = oldKeyRev_n;
					}
					else if (isPositiveRev && isAltRev)
					{
						Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[1].Positive = oldKeyRev_ap;
					}
					else if (!isPositiveRev && isAltRev)
					{
						Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[1].Negative = oldKeyRev_an;
					}
				}
				if (Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i] == actionsOld)
				{
					if (isPositiveOld && !isAltOld)
					{
						Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[0].Positive = oldKeyOld;
					}
					else if (!isPositiveOld && !isAltOld)
					{
						Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[0].Negative = oldKeyOld;
					}
					else if (isPositiveOld && isAltOld)
					{
						Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[1].Positive = oldKeyOld;
					}
					else if (!isPositiveOld && isAltOld)
					{
						Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[1].Negative = oldKeyOld;
					}
				}
			}
			foreach (ButtonListItem button in buttonList)
			{
				if (button.ControlItem == controlItemValRev)
				{
					if (!button.IsAlt && !isAltRev)
					{
						button.ButtonText.text = (!isPositiveRev) ? oldKeyRev_n.ToString() : oldKeyRev_p.ToString();
					}
					else if (button.IsAlt && isAltRev)
					{
						button.ButtonText.text = (!isPositiveRev) ? oldKeyRev_an.ToString() : oldKeyRev_ap.ToString();
					}
				}
				else if (button.ControlItem.Name == controlItemValOld.Name)
				{
					if (!isAltOld && !button.IsAlt)
					{
						button.ButtonText.text = oldKeyOld.ToString();
					}
					else if (isAltOld && button.IsAlt)
					{
						button.ButtonText.text = oldKeyOld.ToString();
					}
				}
			}
			GameMenu.DisableGameMenu = false;
		}

		public void OnKeyChange(KeyCode key, string AxisName, bool changePositive, bool changeAlt, ControlItem controlItemVal)
		{
			if (key == KeyCode.Escape)
			{
				return;
			}
			for (int i = 0; i < Luminosity.IO.InputManager.PlayerOneControlScheme.Actions.Count; i++)
			{
				if (Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[0].Positive == key)
				{
					SaveControlForRevert(Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i], isPositive: true, isAlt: false, key, controlItemVal, changePositive, changeAlt);
					Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[0].Positive = KeyCode.None;
				}
				else if (Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[0].Negative == key)
				{
					SaveControlForRevert(Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i], isPositive: false, isAlt: false, key, controlItemVal, changePositive, changeAlt);
					Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[0].Negative = KeyCode.None;
				}
				else if (Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[1].Positive == key)
				{
					SaveControlForRevert(Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i], isPositive: true, isAlt: true, key, controlItemVal, changePositive, changeAlt);
					Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[1].Positive = KeyCode.None;
				}
				else if (Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[1].Negative == key)
				{
					SaveControlForRevert(Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i], isPositive: false, isAlt: true, key, controlItemVal, changePositive, changeAlt);
					Luminosity.IO.InputManager.PlayerOneControlScheme.Actions[i].Bindings[1].Negative = KeyCode.None;
				}
			}
			ButtonListItem buttonListItem = buttonList.Find((ButtonListItem m) => m.ButtonText.text == key.ToString());
			if (buttonListItem != null)
			{
				controlItemValOld = buttonListItem.ControlItem;
				buttonListItem.ButtonText.text = string.Empty;
				if (buttonListItem.ControlItem.Name != controlItemVal.Name)
				{
					GameMenu.DisableGameMenu = true;
					Client.Instance.ShowConfirmMessageBox(Localization.DuplicatedControl, string.Format(Localization.DuplicateControlMessage, buttonListItem.ControlItem.Name), Localization.Yes, Localization.No, DuplicatedControlsYes, DuplicateControlsNo);
					return;
				}
			}
			if (!CheckIfEmpty())
			{
				Settings.Instance.SaveSettings(Settings.SettingsType.Controls);
			}
		}

		public bool CheckIfEmpty()
		{
			List<ButtonListItem> list = new List<ButtonListItem>();
			foreach (ButtonListItem button in buttonList)
			{
				if (!button.IsAlt && button.ButtonText.text == string.Empty)
				{
					list.Add(button);
				}
			}
			if (list.Count > 0)
			{
				string text = string.Empty;
				foreach (ButtonListItem item in list)
				{
					text = text + "- " + item.ControlItem.Name + "\n";
				}
				Client.Instance.ShowMessageBox(Localization.PleaseAssignAllControls, text);
				return true;
			}
			return false;
		}
	}
}
