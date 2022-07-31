using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZeroGravity.Data;
using ZeroGravity.LevelDesign;
using ZeroGravity.Network;
using ZeroGravity.Objects;
using ZeroGravity.UI;

namespace ZeroGravity.ShipComponents
{
	public class NavigationPanel : MonoBehaviour
	{
		public struct POINT
		{
			public int x;

			public int y;

			public static implicit operator Point(POINT point)
			{
				return new Point(point.x, point.y);
			}
		}

		[CompilerGenerated]
		private sealed class _003CAuthorizedVesselsResponseListener_003Ec__AnonStorey0
		{
			internal AuthorizedVesselsResponse avr;

			internal bool _003C_003Em__0(long m)
			{
				return avr.GUIDs.Contains(m);
			}
		}

		[CompilerGenerated]
		private sealed class _003CDoScan_003Ec__AnonStorey1
		{
			internal MapObjectShip mos;

			internal NavigationPanel _0024this;

			internal void _003C_003Em__0()
			{
				_0024this.ParentVessel.RadarSystem.ActiveScan(mos.ScanningConeAngle, mos.ScanningCone.transform.forward);
			}
		}

		[Header("Navigation")]
		public Map Map;

		public Ship ParentVessel;

		public MapObject PlayerShip;

		public GameObject RightHolder;

		[Header("Selected object info")]
		public GameObject SelectedMapObject;

		public Text NameOfSelectedObjectText;

		public UnityEngine.UI.Image SelectedMapObjectIcon;

		public Text SelectedMapObjectDescription;

		public GameObject HoverObjectUi;

		public Animator HoverObjectUiAnimator;

		public Text HoverObjectName;

		public Transform GroupObjectsRoot;

		public Text GroupObject;

		public GameObject RadiationInfo;

		public Text DegradationRateValue;

		public Text SignatureValue;

		[Header("Orbit parameters")]
		public GameObject OrbitParametersHolder;

		public InputField Inclination;

		public InputField ArgumentOfPeriapsis;

		public InputField LongitudeOfAscendingNode;

		public InputField PeriapsisHeight;

		public InputField ApoapsisHeight;

		public InputField PositionOnOrbit;

		public Text OrbitalPeriod;

		[Header("Buttons")]
		public Button HomeStationButton;

		public Button MyShipButton;

		public Button WarpToButton;

		public Button AddCustomOrbitButton;

		public Button RemoveCustomOrbitButton;

		public Button WarpButton;

		[Header("Actions")]
		public GameObject CurrentItemScreen;

		public GameObject CurrentItemActive;

		public GameObject AuthorizedVesselsScreen;

		public GameObject AuthorizedVesselsActive;

		public Transform AuthorizedItemsHolder;

		public GameObject DistressSignalsScreen;

		public GameObject DistressSignalsActive;

		public Transform DistressItemsHolder;

		public GameObject GroupItemsScreen;

		public GameObject GroupItemsActive;

		public Transform GroupItemsHolder;

		public MapObjectUI MapObjectPrefab;

		public GameObject WarpScreen;

		public GameObject WarpScreenActive;

		public GameObject ScanScreen;

		public GameObject ScanScreenActive;

		public GameObject Warnings;

		public Text WarningText;

		[Header("Lists values")]
		public Text GroupObjectsValue;

		public Text DistressSignalsValue;

		public Text AuthorizedVesselsValue;

		[Header("FTL")]
		public GameObject ManeuverDetails;

		public GameObject ManeuverInProgress;

		public GameObject AlignShipInfo;

		public GameObject NoManeuverSelected;

		public UnityEngine.UI.Image PowerFiller;

		public UnityEngine.UI.Image PowerRequired;

		public Text PowerRequiredValue;

		public Text DistanceValue;

		public Text DockedVessels;

		public int SelectedWarpStage;

		public Transform WarpStageHolder;

		public WarpStageUI WarpStage;

		public Transform WarpCellsHolder;

		public List<WarpCellUI> AllWarpCells = new List<WarpCellUI>();

		public WarpCellUI WarpCell;

		public bool StartTimeEdit;

		public bool EndTimeEdit;

		public Text WarpStartTime;

		public Text WarpEndTime;

		public Text WarpStatus;

		public Text ManeuverTimeLeft;

		public GameObject TimerStatus;

		public GameObject DockedStructureStatus;

		public GameObject WarpCellStatus;

		public GameObject PowerStatus;

		public GameObject VesselBaseSystemsStatus;

		public UnityEngine.UI.Image MassFiller;

		public Text MassValue;

		public Text DockedVesselsMass;

		[Header("Warp Summary")]
		public Text WarpConsumptionSummary;

		public Text PowerConsumptionSummary;

		[Header("SCAN")]
		public GameObject SelectedDetails;

		public Text ActiveSens;

		public Text PassiveSens;

		public Text WarpSens;

		public Slider ScaningSlider;

		public Text ScaningAngle;

		public Text ScanPowerConsumption;

		private float scanAngle = 30f;

		public float PitchAngle;

		public float YawAngle;

		public UnityEngine.UI.Image ScanPowerFiller;

		public Text ScanPowerValue;

		public Text ScanStatus;

		public Button ScanButton;

		public Text SignalAmplificationValue;

		[Header("Pitch & Yaw")]
		public Text PitchValue;

		public Slider PitchSlider;

		public Text YawValue;

		public Slider YawSlider;

		public UnityEngine.UI.Image CapacitorFiller;

		[CompilerGenerated]
		private static Func<SubSystem, bool> _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static Func<SubSystem, bool> _003C_003Ef__am_0024cache1;

		[CompilerGenerated]
		private static Func<SpaceObjectVessel, double> _003C_003Ef__am_0024cache2;

		[CompilerGenerated]
		private static Func<SpaceObjectVessel, double> _003C_003Ef__am_0024cache3;

		[CompilerGenerated]
		private static Func<MapObject, bool> _003C_003Ef__am_0024cache4;

		[CompilerGenerated]
		private static Func<DockedVesselData, long> _003C_003Ef__am_0024cache5;

		[CompilerGenerated]
		private static Func<SpaceObjectVessel, long> _003C_003Ef__am_0024cache6;

		[CompilerGenerated]
		private static Func<SceneMachineryPartSlot, bool> _003C_003Ef__am_0024cache7;

		[CompilerGenerated]
		private static Func<SceneMachineryPartSlot, int> _003C_003Ef__am_0024cache8;

		[CompilerGenerated]
		private static Func<WarpCellUI, bool> _003C_003Ef__am_0024cache9;

		[CompilerGenerated]
		private static Func<WarpCellUI, bool> _003C_003Ef__am_0024cacheA;

		[CompilerGenerated]
		private static Func<WarpCellUI, int> _003C_003Ef__am_0024cacheB;

		[CompilerGenerated]
		private static Func<SpaceObjectVessel, double> _003C_003Ef__am_0024cacheC;

		[CompilerGenerated]
		private static Func<SpaceObjectVessel, double> _003C_003Ef__am_0024cacheD;

		public bool InputFocused
		{
			get
			{
				if (Inclination.isFocused || ArgumentOfPeriapsis.isFocused || LongitudeOfAscendingNode.isFocused || PeriapsisHeight.isFocused || ApoapsisHeight.isFocused || PositionOnOrbit.isFocused)
				{
					return true;
				}
				return false;
			}
		}

		private float currentCapacity
		{
			get
			{
				if (ParentVessel != null && ParentVessel.Capacitor != null)
				{
					return ParentVessel.Capacitor.Capacity;
				}
				return 0f;
			}
		}

		private float maximumCapacity
		{
			get
			{
				if (ParentVessel != null && ParentVessel.Capacitor != null)
				{
					return ParentVessel.Capacitor.MaxCapacity;
				}
				return 0f;
			}
		}

		[DllImport("user32.dll")]
		private static extern bool SetCursorPos(int x, int y);

		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out POINT lpPoint);

		private void Start()
		{
			if (HoverObjectUiAnimator == null)
			{
				HoverObjectUiAnimator = HoverObjectUi.GetComponent<Animator>();
			}
			ScaningSlider.onValueChanged.AddListener(ScaningAngleSlider);
			PitchSlider.onValueChanged.AddListener(ScaningConePitch);
			YawSlider.onValueChanged.AddListener(ScanningConeYaw);
		}

		public void ShowWarning(string warningText)
		{
			WarningText.text = warningText;
			Warnings.SetActive(true);
		}

		public void HideWarning()
		{
			Warnings.GetComponent<Animator>().SetTrigger("Stop");
		}

		public void OnInteract(Ship parent)
		{
			base.gameObject.Activate(true);
			Map = Client.Instance.Map;
			if (parent != ParentVessel)
			{
				Map.RemoveManeuverCourse();
			}
			ParentVessel = parent;
			ParentVessel.NavPanel = this;
			Inclination.onEndEdit.AddListener(SetInclination);
			ArgumentOfPeriapsis.onEndEdit.AddListener(SetAop);
			PeriapsisHeight.onEndEdit.AddListener(SetPeriapsis);
			ApoapsisHeight.onEndEdit.AddListener(SetApoapsis);
			LongitudeOfAscendingNode.onEndEdit.AddListener(SetLoan);
			PositionOnOrbit.onEndEdit.AddListener(SetOrbitPosition);
			Client.Instance.NetworkController.EventSystem.AddListener(typeof(AuthorizedVesselsResponse), AuthorizedVesselsResponseListener);
			Client.Instance.NetworkController.SendToGameServer(new AuthorizedVesselsRequest());
			GameObject go = WarpButton.gameObject;
			Dictionary<int, SubSystem>.ValueCollection values = ParentVessel.SubSystems.Values;
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003COnInteract_003Em__0;
			}
			go.Activate(values.FirstOrDefault(_003C_003Ef__am_0024cache0) != null);
		}

		public void OnDetach()
		{
			base.gameObject.SetActive(false);
			Inclination.onEndEdit.RemoveAllListeners();
			ArgumentOfPeriapsis.onEndEdit.RemoveAllListeners();
			PeriapsisHeight.onEndEdit.RemoveAllListeners();
			ApoapsisHeight.onEndEdit.RemoveAllListeners();
			LongitudeOfAscendingNode.onEndEdit.RemoveAllListeners();
			PositionOnOrbit.onEndEdit.RemoveAllListeners();
			Client.Instance.NetworkController.EventSystem.RemoveListener(typeof(AuthorizedVesselsResponse), AuthorizedVesselsResponseListener);
		}

		private void Update()
		{
			if (Map == null)
			{
				return;
			}
			UpdateUIElements();
			if (Map.SelectedObject != null && (!(Map.SelectedObject is MapObjectCustomOrbit) || Map.Dragging))
			{
				UpdateParameters();
			}
			if (ManeuverDetails.activeInHierarchy)
			{
				UpdateManeuver();
			}
			else if (ParentVessel.EndWarpTime > Client.Instance.SolarSystem.CurrentTime && ParentVessel.IsWarpOnline && ParentVessel.FTLEngine.IsSwitchedOn())
			{
				ManeuverInProgress.SetActive(true);
				ManeuverTimeLeft.text = Localization.ETA + " " + FormatHelper.PeriodFormat(ParentVessel.EndWarpTime - Client.Instance.SolarSystem.CurrentTime);
				AlignShipInfo.SetActive(false);
				NoManeuverSelected.SetActive(false);
			}
			else if (ManeuverInProgress.activeInHierarchy && Map.WarpManeuver == null)
			{
				ManeuverInProgress.SetActive(false);
				ManeuverTimeLeft.text = string.Empty;
				NoManeuverSelected.SetActive(true);
			}
			if (InputManager.GetAxis(InputManager.AxisNames.MouseWheel).IsNotEpsilonZero() && (StartTimeEdit || EndTimeEdit) && Map.WarpManeuver != null)
			{
				float axis = InputManager.GetAxis(InputManager.AxisNames.MouseWheel);
				if (axis > 0f)
				{
					if (EndTimeEdit)
					{
						UpdateEndTime(10f);
					}
					else if (StartTimeEdit)
					{
						UpdateStartTime(10f);
					}
				}
				else if (axis < 0f)
				{
					if (EndTimeEdit)
					{
						UpdateEndTime(-10f);
					}
					else if (StartTimeEdit)
					{
						UpdateStartTime(-10f);
					}
				}
			}
			if (HoverObjectUi.activeInHierarchy)
			{
				HoverObjectUi.transform.position = Input.mousePosition;
			}
		}

		private void UpdateUIElements()
		{
			if (!HomeStationButton.interactable && Map.Home != null)
			{
				HomeStationButton.interactable = true;
			}
			else if (Map.Home == null)
			{
				HomeStationButton.interactable = false;
			}
			OrbitParametersHolder.Activate(Map.SelectedObject != null);
			RemoveCustomOrbitButton.gameObject.Activate(Map.SelectedObject is MapObjectCustomOrbit);
			if (ManeuverDetails.activeInHierarchy)
			{
				CapacitorFiller.fillAmount = currentCapacity / maximumCapacity;
			}
			if (ScanScreenActive.activeInHierarchy)
			{
				UpdateScanDetails();
			}
		}

		public void ShowObjectInfo(MapObject obj)
		{
			Vector3 vector = Map.MapCamera.WorldToScreenPoint(obj.ObjectPosition);
			vector.z = 0f;
			HoverObjectName.text = obj.Name;
		}

		public void SetInclination(string value)
		{
			double result;
			double.TryParse(value, out result);
			result = CheckValue(0.0, 360.0, result);
			UpdateOrbit(1, result);
		}

		public void SetAop(string value)
		{
			double result;
			double.TryParse(value, out result);
			result = CheckValue(0.0, 360.0, result);
			UpdateOrbit(2, result);
		}

		public void SetPeriapsis(string value)
		{
			double result;
			double.TryParse(value, out result);
			result = CheckValue(0.0, Map.SelectedObject.Orbit.ApoapsisDistance / 1000.0, result);
			UpdateOrbit(3, result * 1000.0);
		}

		public void SetApoapsis(string value)
		{
			double result;
			double.TryParse(value, out result);
			result = CheckValue(Map.SelectedObject.Orbit.PeriapsisDistance / 1000.0, Map.SelectedObject.Orbit.Parent.GravityInfluenceRadius / 1000.0, result);
			UpdateOrbit(4, result * 1000.0);
		}

		public void SetLoan(string value)
		{
			double result;
			double.TryParse(value, out result);
			result = CheckValue(0.0, 360.0, result);
			UpdateOrbit(5, result);
		}

		public void SetOrbitPosition(string value)
		{
			double result;
			double.TryParse(value, out result);
			result = CheckValue(0.0, 360.0, result);
			UpdateOrbit(6, result);
		}

		private double CheckValue(double min, double max, double value)
		{
			if (value > max)
			{
				return max;
			}
			if (value < min)
			{
				return min;
			}
			return value;
		}

		private void UpdateOrbit(int arg, double value)
		{
			OrbitParameters orbit = Map.SelectedObject.Orbit;
			double inclination = ((arg != 1) ? orbit.Inclination : value);
			double argumentOfPeriapsis = ((arg != 2) ? orbit.ArgumentOfPeriapsis : value);
			double periapsisDistance = ((arg != 3) ? orbit.PeriapsisDistance : value);
			double apoapsisDistance = ((arg != 4) ? orbit.ApoapsisDistance : value);
			double longitudeOfAscendingNode = ((arg != 5) ? orbit.LongitudeOfAscendingNode : value);
			double trueAnomalyAngleDeg = ((arg != 6) ? (orbit.TrueAnomalyAtZeroTime() * (180.0 / System.Math.PI)) : value);
			Map.SelectedObject.Orbit.InitFromPeriapisAndApoapsis(orbit.Parent, periapsisDistance, apoapsisDistance, inclination, argumentOfPeriapsis, longitudeOfAscendingNode, trueAnomalyAngleDeg, 0.0);
			Map.SelectedObject.Orbit.ResetOrbit(Client.Instance.SolarSystem.CurrentTime);
			Map.SelectedObject.SetOrbit();
			Map.SelectedObject.UpdateOrbitPlane();
			UpdateParameters();
		}

		public void UpdateParameters()
		{
			if (Map.SelectedObject != null)
			{
				OrbitParametersHolder.Activate(Map.SelectedObject.Orbit != null);
				if (Map.SelectedObject.Orbit != null)
				{
					Inclination.text = Map.SelectedObject.Orbit.Inclination.ToString("0.00", NumberFormatInfo.InvariantInfo);
					ArgumentOfPeriapsis.text = Map.SelectedObject.Orbit.ArgumentOfPeriapsis.ToString("0.00", NumberFormatInfo.InvariantInfo);
					LongitudeOfAscendingNode.text = Map.SelectedObject.Orbit.LongitudeOfAscendingNode.ToString("0.00", NumberFormatInfo.InvariantInfo);
					PeriapsisHeight.text = (Map.SelectedObject.Orbit.PeriapsisDistance / 1000.0).ToString("0.00", NumberFormatInfo.InvariantInfo);
					ApoapsisHeight.text = (Map.SelectedObject.Orbit.ApoapsisDistance / 1000.0).ToString("0.00", NumberFormatInfo.InvariantInfo);
					PositionOnOrbit.text = (Map.SelectedObject.Orbit.TrueAnomalyAtZeroTime() * (180.0 / System.Math.PI)).ToString("0.00", NumberFormatInfo.InvariantInfo);
					OrbitalPeriod.text = FormatHelper.PeriodFormat(Map.SelectedObject.Orbit.OrbitalPeriod);
					Inclination.interactable = Map.SelectedObject is MapObjectCustomOrbit;
					ArgumentOfPeriapsis.interactable = Map.SelectedObject is MapObjectCustomOrbit;
					PeriapsisHeight.interactable = Map.SelectedObject is MapObjectCustomOrbit;
					ApoapsisHeight.interactable = Map.SelectedObject is MapObjectCustomOrbit;
					LongitudeOfAscendingNode.interactable = Map.SelectedObject is MapObjectCustomOrbit;
					PositionOnOrbit.interactable = Map.SelectedObject is MapObjectCustomOrbit;
				}
			}
		}

		public void UpdateObjectData()
		{
			if (Map.SelectedObject != null)
			{
				UpdateSelectedObjectInfo();
				UpdateParameters();
				AddCustomOrbitButton.gameObject.SetActive(Map.SelectedObject is MapObjectCelestial);
				if (Map.SelectedObject == Map.MyShip)
				{
					WarpToButton.gameObject.Activate(false);
				}
				else if (!(Map.SelectedObject is MapObjectCelestial) && Map.SelectedObject.Orbit != null)
				{
					GameObject go = WarpToButton.gameObject;
					int value;
					if (!ParentVessel.IsWarpOnline)
					{
						Dictionary<int, SubSystem>.ValueCollection values = ParentVessel.SubSystems.Values;
						if (_003C_003Ef__am_0024cache1 == null)
						{
							_003C_003Ef__am_0024cache1 = _003CUpdateObjectData_003Em__1;
						}
						value = ((values.FirstOrDefault(_003C_003Ef__am_0024cache1) != null) ? 1 : 0);
					}
					else
					{
						value = 0;
					}
					go.Activate((byte)value != 0);
				}
				else
				{
					WarpToButton.gameObject.Activate(false);
				}
				RightHolder.Activate(true);
			}
			else
			{
				RightHolder.Activate(false);
				ActivateOther(-1);
			}
		}

		private void UpdateSelectedObjectInfo(bool ToScan = false)
		{
			string text = Map.SelectedObject.Name.ToUpper();
			Sprite icon = Map.SelectedObject.Icon;
			string text2 = Map.SelectedObject.Description;
			string empty = string.Empty;
			string text3 = ((!(Map.SelectedObject is MapObjectVessel)) ? "-" : (Map.SelectedObject as MapObjectVessel).RadarSignature.ToString("0"));
			RadiationInfo.Activate(true);
			if (Map.SelectedObject is MapObjectCustomOrbit)
			{
				float vesselExposureDamage = Client.Instance.GetVesselExposureDamage((Map.SelectedObject as MapObjectCustomOrbit).Orbit.Position.Magnitude);
				empty = ((double)vesselExposureDamage * SpaceObjectVessel.VesselDecayRateMultiplier).ToString("0.0");
			}
			else if (Map.SelectedObject is MapObjectFuzzyScan)
			{
				MapObjectFuzzyScan mapObjectFuzzyScan = Map.SelectedObject as MapObjectFuzzyScan;
				string text5;
				if (mapObjectFuzzyScan.Vessels.Count > 1)
				{
					List<SpaceObjectVessel> vessels = mapObjectFuzzyScan.Vessels;
					if (_003C_003Ef__am_0024cache2 == null)
					{
						_003C_003Ef__am_0024cache2 = _003CUpdateSelectedObjectInfo_003Em__2;
					}
					string text4 = vessels.Min(_003C_003Ef__am_0024cache2).ToString("0");
					List<SpaceObjectVessel> vessels2 = mapObjectFuzzyScan.Vessels;
					if (_003C_003Ef__am_0024cache3 == null)
					{
						_003C_003Ef__am_0024cache3 = _003CUpdateSelectedObjectInfo_003Em__3;
					}
					text5 = text4 + " to " + vessels2.Max(_003C_003Ef__am_0024cache3).ToString("0");
				}
				else
				{
					text5 = mapObjectFuzzyScan.Vessels[0].RadarSignature.ToString("0");
				}
				text3 = text5;
				text2 = Localization.PossibleContacts + ": " + mapObjectFuzzyScan.Vessels.Count;
				empty = "-";
				RadiationInfo.Activate(false);
			}
			else if (Map.SelectedObject is MapObjectFixedPosition)
			{
				empty = "-";
				RadiationInfo.Activate(false);
			}
			else
			{
				empty = ((double)Map.SelectedObject.VesselExposureDamage * SpaceObjectVessel.VesselDecayRateMultiplier).ToString("0.0");
			}
			if (ToScan)
			{
				SelectedDetails.Activate(true);
				double sensitivityMultiplier = ParentVessel.RadarSystem.GetSensitivityMultiplier();
				ActiveSens.text = (ParentVessel.RadarSystem.ActiveScanSensitivity * sensitivityMultiplier).ToString("0.0");
				PassiveSens.text = (ParentVessel.RadarSystem.PassiveScanSensitivity * sensitivityMultiplier).ToString("0.0");
				WarpSens.text = (ParentVessel.RadarSystem.WarpDetectionSensitivity * sensitivityMultiplier).ToString("0.0");
				ScanPowerConsumption.text = FormatHelper.FormatValue(ParentVessel.RadarSystem.GetPowerConsumption(true) * ParentVessel.RadarSystem.ActiveScanDuration);
				SignalAmplificationValue.text = ((!(ParentVessel.RadarSystem.SignalAmplification > 1f)) ? Localization.NoPart.ToUpper() : (FormatHelper.FormatValue(ParentVessel.RadarSystem.SignalAmplification) + "x"));
			}
			else
			{
				NameOfSelectedObjectText.GetComponentInChildren<Text>().text = text;
				SelectedMapObjectIcon.sprite = icon;
				SelectedMapObjectDescription.text = text2;
				DegradationRateValue.text = empty;
				SignatureValue.text = text3;
			}
		}

		public void ShowMeHome()
		{
			Map.FocusToHome();
			ActivateOther(0);
			UpdateObjectData();
		}

		public void ShowMeMyShip()
		{
			Map.FocusToParentVessel();
			ActivateOther(0);
			UpdateObjectData();
		}

		public void GroupItemsList()
		{
			if (Map.SelectedObject == null)
			{
				RightHolder.SetActive(false);
				ActivateOther(-1);
				return;
			}
			RightHolder.SetActive(true);
			int num = 0;
			GroupItemsHolder.DestroyAll<MapObjectUI>();
			foreach (MapObject item in Map.SelectedVesselsGroup)
			{
				MapObjectUI mapObjectUI = UnityEngine.Object.Instantiate(MapObjectPrefab, GroupItemsHolder);
				mapObjectUI.gameObject.SetActive(true);
				mapObjectUI.gameObject.transform.Reset();
				mapObjectUI.gameObject.transform.localScale = Vector3.one;
				mapObjectUI.MapObj = item;
				mapObjectUI.UpdateUI();
				num++;
			}
			GroupObjectsValue.text = num.ToString("0");
		}

		public void DistressSignals()
		{
			RightHolder.SetActive(true);
			int num = 0;
			DistressItemsHolder.DestroyAll<MapObjectUI>();
			Dictionary<IMapMainObject, MapObject>.ValueCollection values = Map.AllMapObjects.Values;
			if (_003C_003Ef__am_0024cache4 == null)
			{
				_003C_003Ef__am_0024cache4 = _003CDistressSignals_003Em__4;
			}
			foreach (MapObject item in values.Where(_003C_003Ef__am_0024cache4))
			{
				MapObjectUI mapObjectUI = UnityEngine.Object.Instantiate(MapObjectPrefab, DistressItemsHolder);
				mapObjectUI.gameObject.SetActive(true);
				mapObjectUI.gameObject.transform.Reset();
				mapObjectUI.gameObject.transform.localScale = Vector3.one;
				mapObjectUI.MapObj = item;
				mapObjectUI.UpdateUI();
				num++;
			}
			DistressSignalsValue.text = num.ToString("0");
		}

		private void AuthorizedVesselsResponseListener(NetworkData data)
		{
			_003CAuthorizedVesselsResponseListener_003Ec__AnonStorey0 _003CAuthorizedVesselsResponseListener_003Ec__AnonStorey = new _003CAuthorizedVesselsResponseListener_003Ec__AnonStorey0();
			_003CAuthorizedVesselsResponseListener_003Ec__AnonStorey.avr = data as AuthorizedVesselsResponse;
			AuthorizedItemsHolder.DestroyAll<MapObjectUI>();
			int num = 0;
			if (_003CAuthorizedVesselsResponseListener_003Ec__AnonStorey.avr.GUIDs == null)
			{
				return;
			}
			foreach (MapObject value in Map.AllMapObjects.Values)
			{
				SpaceObjectVessel spaceObjectVessel = value.MainObject as SpaceObjectVessel;
				if (!(spaceObjectVessel != null) || !spaceObjectVessel.IsMainVessel)
				{
					continue;
				}
				List<long> list = new List<long>();
				list.Add(spaceObjectVessel.GUID);
				List<long> list2 = list;
				if (spaceObjectVessel.IsDummyObject && spaceObjectVessel.DummyDockedVessels.Count > 0)
				{
					List<DockedVesselData> dummyDockedVessels = spaceObjectVessel.DummyDockedVessels;
					if (_003C_003Ef__am_0024cache5 == null)
					{
						_003C_003Ef__am_0024cache5 = _003CAuthorizedVesselsResponseListener_003Em__5;
					}
					list2.AddRange(dummyDockedVessels.Select(_003C_003Ef__am_0024cache5));
				}
				else
				{
					List<SpaceObjectVessel> allDockedVessels = spaceObjectVessel.AllDockedVessels;
					if (_003C_003Ef__am_0024cache6 == null)
					{
						_003C_003Ef__am_0024cache6 = _003CAuthorizedVesselsResponseListener_003Em__6;
					}
					list2.AddRange(allDockedVessels.Select(_003C_003Ef__am_0024cache6));
				}
				if (list2.FirstOrDefault(_003CAuthorizedVesselsResponseListener_003Ec__AnonStorey._003C_003Em__0) != 0)
				{
					MapObjectUI mapObjectUI = UnityEngine.Object.Instantiate(MapObjectPrefab, AuthorizedItemsHolder);
					mapObjectUI.gameObject.SetActive(true);
					mapObjectUI.gameObject.transform.Reset();
					mapObjectUI.gameObject.transform.localScale = Vector3.one;
					mapObjectUI.MapObj = value;
					spaceObjectVessel.RadarVisibilityType = RadarVisibilityType.AlwaysVisible;
					mapObjectUI.UpdateUI();
					num++;
				}
			}
			AuthorizedVesselsValue.text = num.ToString("0");
			Client.Instance.CanvasManager.CanvasUI.QuestIndicators.AddMarkersOnMap();
		}

		public void Scan()
		{
			if (Map.SelectedObject == null)
			{
				Map.SelectMapObject(Map.MyShip);
			}
			UpdateSelectedObjectInfo(true);
			MapObjectShip mapObjectShip = Map.MyShip as MapObjectShip;
			if (Map.SelectedObject != null && Map.SelectedObject != mapObjectShip)
			{
				Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, (Map.SelectedObject.TruePosition - mapObjectShip.MainObject.Position).ToVector3());
				PitchAngle = quaternion.eulerAngles.x;
				YawAngle = quaternion.eulerAngles.y;
			}
			Map.FocusToParentVessel();
			mapObjectShip.ToggleCone(true);
			RightHolder.Activate(true);
			ScaningAngleSlider(scanAngle);
			SetScanningConePitch(PitchAngle);
			ScanningConeYaw(YawAngle);
		}

		public void ScaningAngleSlider(float val)
		{
			ScaningAngle.text = val.ToString("0.00");
			ScaningSlider.value = val;
			scanAngle = val;
			(Map.MyShip as MapObjectShip).ScanningConeAngle = scanAngle;
			(Map.MyShip as MapObjectShip).UpdateObject();
		}

		private void ScaningConePitch(float val)
		{
			if (val == -1f)
			{
				int num = (int)PitchSlider.handleRect.position.x;
				SetScanningConePitch(359f);
				int num2 = (int)PitchSlider.handleRect.position.x;
				POINT lpPoint;
				GetCursorPos(out lpPoint);
				SetCursorPos(lpPoint.x + (num2 - num), lpPoint.y);
			}
			else if (val == 360f)
			{
				int num3 = (int)PitchSlider.handleRect.position.x;
				SetScanningConePitch(0f);
				int num4 = (int)PitchSlider.handleRect.position.x;
				POINT lpPoint2;
				GetCursorPos(out lpPoint2);
				SetCursorPos(lpPoint2.x + (num4 - num3), lpPoint2.y);
			}
			else
			{
				SetScanningConePitch(val);
			}
		}

		public void SetScanningConePitch(float val)
		{
			PitchAngle = val;
			PitchSlider.value = val;
			MapObjectShip mapObjectShip = Map.SelectedObject as MapObjectShip;
			bool? obj;
			if ((object)mapObjectShip == null)
			{
				obj = null;
			}
			else
			{
				GameObject scanningCone = mapObjectShip.ScanningCone;
				obj = (((object)scanningCone != null) ? new bool?(scanningCone.activeInHierarchy) : null);
			}
			if (obj == true)
			{
				mapObjectShip.ScanningCone.transform.localRotation = Quaternion.Euler(val, YawAngle, 0f);
				mapObjectShip.Pitch.transform.localRotation = Quaternion.Euler(val, 0f, 0f);
			}
			PitchValue.text = val.ToString("0.00");
		}

		private void ScanningConeYaw(float val)
		{
			if (val == -1f)
			{
				int num = (int)YawSlider.handleRect.position.x;
				SetScanningConeYaw(359f);
				int num2 = (int)YawSlider.handleRect.position.x;
				POINT lpPoint;
				GetCursorPos(out lpPoint);
				SetCursorPos(lpPoint.x + (num2 - num), lpPoint.y);
			}
			else if (val == 360f)
			{
				int num3 = (int)YawSlider.handleRect.position.x;
				SetScanningConeYaw(0f);
				int num4 = (int)YawSlider.handleRect.position.x;
				POINT lpPoint2;
				GetCursorPos(out lpPoint2);
				SetCursorPos(lpPoint2.x + (num4 - num3), lpPoint2.y);
			}
			else
			{
				SetScanningConeYaw(val);
			}
		}

		public void SetScanningConeYaw(float val)
		{
			YawAngle = val;
			YawSlider.value = val;
			MapObjectShip mapObjectShip = Map.SelectedObject as MapObjectShip;
			bool? obj;
			if ((object)mapObjectShip == null)
			{
				obj = null;
			}
			else
			{
				GameObject scanningCone = mapObjectShip.ScanningCone;
				obj = (((object)scanningCone != null) ? new bool?(scanningCone.activeInHierarchy) : null);
			}
			if (obj == true)
			{
				mapObjectShip.ScanningCone.transform.localRotation = Quaternion.Euler(PitchAngle, val, 0f);
				mapObjectShip.Yaw.transform.localRotation = Quaternion.Euler(0f, val, 0f);
			}
			YawValue.text = val.ToString("0.00");
		}

		public void DoScan()
		{
			_003CDoScan_003Ec__AnonStorey1 _003CDoScan_003Ec__AnonStorey = new _003CDoScan_003Ec__AnonStorey1();
			_003CDoScan_003Ec__AnonStorey._0024this = this;
			_003CDoScan_003Ec__AnonStorey.mos = Map.MyShip as MapObjectShip;
			if (ParentVessel.RadarSystem != null && (ParentVessel.RadarSystem.Status == SystemStatus.OffLine || ParentVessel.RadarSystem.Status == SystemStatus.CoolDown))
			{
				ParentVessel.RadarSystem.ActiveScanTask = new Task(_003CDoScan_003Ec__AnonStorey._003C_003Em__0);
				ParentVessel.RadarSystem.SwitchOn();
			}
		}

		private void UpdateScanDetails()
		{
			if (ParentVessel.RadarSystem.Status == SystemStatus.OffLine && ParentVessel.VesselBaseSystem.Status == SystemStatus.OnLine)
			{
				ScanButton.interactable = true;
			}
			else
			{
				ScanButton.interactable = false;
			}
			UnityEngine.Color color;
			ScanStatus.text = ParentVessel.RadarSystem.GetStatus(out color);
			ScanStatus.color = color;
			ScanPowerFiller.fillAmount = currentCapacity / maximumCapacity;
			ScanPowerValue.text = FormatHelper.CurrentMax(currentCapacity, maximumCapacity);
		}

		public void ActivateOther(int oth)
		{
			CurrentItemScreen.SetActive(oth == 0);
			CurrentItemActive.SetActive(oth == 0);
			AuthorizedVesselsScreen.SetActive(oth == 1);
			AuthorizedVesselsActive.SetActive(oth == 1);
			DistressSignalsScreen.SetActive(oth == 2);
			DistressSignalsActive.SetActive(oth == 2);
			GroupItemsScreen.SetActive(oth == 3);
			GroupItemsActive.SetActive(oth == 3);
			WarpScreen.SetActive(oth == 4);
			WarpScreenActive.SetActive(oth == 4);
			ScanScreen.Activate(oth == 5);
			ScanScreenActive.Activate(oth == 5);
			switch (oth)
			{
			case 0:
				UpdateObjectData();
				break;
			case 1:
				RightHolder.Activate(true);
				break;
			case 2:
				DistressSignals();
				break;
			case 3:
				GroupItemsList();
				break;
			case 4:
				Warp();
				break;
			case 5:
				Scan();
				break;
			}
		}

		public void AddCustomOrbit()
		{
			Map.CreateCustomOrbit();
		}

		public void RemoveCustomOrbit()
		{
			Map.RemoveCustomOrbit();
			UpdateObjectData();
		}

		public void WarpTo()
		{
			Map.CreateManeuverCourse();
			ActivateOther(4);
		}

		public void Warp()
		{
			RightHolder.SetActive(true);
			if (Map.WarpManeuver != null)
			{
				NoManeuverSelected.SetActive(false);
				if (Map.WarpManeuver.Initialized && ParentVessel.FTLEngine.IsSwitchedOn())
				{
					ManeuverDetails.SetActive(false);
					ManeuverInProgress.SetActive(true);
					AlignShipInfo.SetActive(true);
					ManeuverTimeLeft.text = string.Empty;
				}
				else
				{
					AlignShipInfo.SetActive(false);
					ManeuverInProgress.SetActive(false);
					ManeuverDetails.SetActive(true);
					DockedVessels.text = ParentVessel.MainVessel.AllDockedVessels.Count.ToString();
					SetWarpStages();
					SetWarpCells();
					UpdateManeuver();
				}
				CalculateMass();
			}
			else
			{
				ManeuverDetails.SetActive(false);
				ManeuverInProgress.SetActive(false);
				NoManeuverSelected.SetActive(true);
				ManeuverTimeLeft.text = string.Empty;
			}
		}

		public void SetWarpStages()
		{
			WarpStageHolder.DestroyAll<WarpStageUI>();
			for (int i = 0; i <= ParentVessel.FTLEngine.MaxWarp; i++)
			{
				WarpStageUI warpStageUI = UnityEngine.Object.Instantiate(WarpStage, WarpStageHolder);
				warpStageUI.gameObject.SetActive(true);
				warpStageUI.gameObject.transform.Reset();
				warpStageUI.gameObject.transform.localScale = Vector3.one;
				warpStageUI.Value = i;
				warpStageUI.Panel = this;
			}
			if (SelectedWarpStage >= 0 && SelectedWarpStage <= ParentVessel.FTLEngine.MaxWarp)
			{
				SelectWarpStage(SelectedWarpStage);
			}
			else
			{
				SelectWarpStage(ParentVessel.FTLEngine.MaxWarp);
			}
		}

		public void SelectWarpStage(int i)
		{
			SelectedWarpStage = i;
			WarpStageUI[] componentsInChildren = WarpStageHolder.GetComponentsInChildren<WarpStageUI>();
			foreach (WarpStageUI warpStageUI in componentsInChildren)
			{
				warpStageUI.Selected.gameObject.SetActive(SelectedWarpStage == warpStageUI.Value);
			}
			PowerRequired.fillAmount = ParentVessel.FTLEngine.WarpsData[SelectedWarpStage].PowerConsumption / maximumCapacity;
			PowerRequiredValue.text = FormatHelper.CurrentMax(ParentVessel.FTLEngine.WarpsData[SelectedWarpStage].PowerConsumption, maximumCapacity);
			Map.WarpManeuver.Transfer.WarpIndex = SelectedWarpStage;
			PowerConsumptionSummary.text = FormatHelper.FormatValue(ParentVessel.FTLEngine.WarpsData[SelectedWarpStage].PowerConsumption);
			CalculateMass();
		}

		public void SetWarpCells()
		{
			float num = 0f;
			WarpCellsHolder.DestroyAll<WarpCellUI>();
			AllWarpCells.Clear();
			SceneMachineryPartSlot[] machineryPartSlots = ParentVessel.FTLEngine.MachineryPartSlots;
			if (_003C_003Ef__am_0024cache7 == null)
			{
				_003C_003Ef__am_0024cache7 = _003CSetWarpCells_003Em__7;
			}
			IEnumerable<SceneMachineryPartSlot> source = machineryPartSlots.Where(_003C_003Ef__am_0024cache7);
			if (_003C_003Ef__am_0024cache8 == null)
			{
				_003C_003Ef__am_0024cache8 = _003CSetWarpCells_003Em__8;
			}
			foreach (SceneMachineryPartSlot item in source.OrderBy(_003C_003Ef__am_0024cache8))
			{
				WarpCellUI warpCellUI = UnityEngine.Object.Instantiate(WarpCell, WarpCellsHolder);
				warpCellUI.gameObject.SetActive(true);
				warpCellUI.gameObject.transform.Reset();
				warpCellUI.gameObject.transform.localScale = Vector3.one;
				warpCellUI.Panel = this;
				warpCellUI.Slot = item;
				if (item.Item != null)
				{
					num += item.Item.Health;
					warpCellUI.IsSelected = true;
				}
				AllWarpCells.Add(warpCellUI);
				warpCellUI.UpdateUI();
			}
		}

		public void UpdateManeuver()
		{
			if (!(Map.WarpManeuver != null))
			{
				return;
			}
			float num = 0f;
			float num2 = Map.WarpManeuver.FuelAmountRequired;
			WarpConsumptionSummary.text = FormatHelper.FormatValue(num2);
			DistanceValue.text = (Map.WarpManeuver.ManeuverDistance / 1000.0).ToString("0.0") + " km";
			WarpStartTime.text = FormatHelper.PeriodFormat(Map.WarpManeuver.Transfer.StartEta);
			WarpEndTime.text = FormatHelper.PeriodFormat(Map.WarpManeuver.Transfer.EndEta);
			List<WarpCellUI> allWarpCells = AllWarpCells;
			if (_003C_003Ef__am_0024cache9 == null)
			{
				_003C_003Ef__am_0024cache9 = _003CUpdateManeuver_003Em__9;
			}
			foreach (WarpCellUI item in allWarpCells.Where(_003C_003Ef__am_0024cache9))
			{
				num += item.Slot.Item.Health;
				if (item.Slot.Item.Health > num2)
				{
					item.FillerRequired.fillAmount = num2 / item.Slot.Item.MaxHealth;
					num2 = 0f;
				}
				else
				{
					item.FillerRequired.fillAmount = item.Slot.Item.Health / item.Slot.Item.MaxHealth;
					num2 -= item.Slot.Item.Health;
				}
			}
			ManeuverTransfer transfer = Map.WarpManeuver.Transfer;
			List<WarpCellUI> allWarpCells2 = AllWarpCells;
			if (_003C_003Ef__am_0024cacheA == null)
			{
				_003C_003Ef__am_0024cacheA = _003CUpdateManeuver_003Em__A;
			}
			IEnumerable<WarpCellUI> source = allWarpCells2.Where(_003C_003Ef__am_0024cacheA);
			if (_003C_003Ef__am_0024cacheB == null)
			{
				_003C_003Ef__am_0024cacheB = _003CUpdateManeuver_003Em__B;
			}
			transfer.WarpCells = source.Select(_003C_003Ef__am_0024cacheB).ToList();
			Map.WarpManeuver.Transfer.WarpIndex = SelectedWarpStage;
			if (ParentVessel.VesselBaseSystem.IsSwitchedOn())
			{
				if (Map.WarpManeuver.FeasibilityError != 0)
				{
					WarpStatus.color = Colors.Red;
					WarpStatus.text = Map.WarpManeuver.FeasibilityError.ToLocalizedString().ToUpper();
				}
				else
				{
					WarpStatus.color = Colors.Green;
					WarpStatus.text = Localization.Ready.ToUpper();
				}
			}
			else
			{
				WarpStatus.color = Colors.Red;
				WarpStatus.text = Localization.VesselPowerOffline.ToUpper();
			}
			ManeuverChecklist();
		}

		public void ManeuverChecklist()
		{
			VesselBaseSystemsStatus.SetActive(!ParentVessel.VesselBaseSystem.IsSwitchedOn());
			if (Map.WarpManeuver.FeasibilityError == ManeuverCourse.FeasibilityErrorType.Course_Impossible)
			{
				TimerStatus.SetActive(true);
				PowerStatus.SetActive(true);
				WarpCellStatus.SetActive(true);
				DockedStructureStatus.SetActive(true);
			}
			else
			{
				TimerStatus.SetActive(Map.WarpManeuver.ManeuverError);
				PowerStatus.SetActive(Map.WarpManeuver.PowerError);
				WarpCellStatus.SetActive(Map.WarpManeuver.WarpCellError);
				DockedStructureStatus.SetActive(Map.WarpManeuver.DockingStructureError);
			}
		}

		public void InitializeManeuver()
		{
			if (Map.WarpManeuver.FeasibilityError == ManeuverCourse.FeasibilityErrorType.None && ParentVessel.VesselBaseSystem.IsSwitchedOn())
			{
				ParentVessel.FTLEngine.SwitchOn();
				Map.InitializeManeuverCourse();
				NoManeuverSelected.SetActive(false);
				ManeuverDetails.SetActive(false);
				ManeuverInProgress.SetActive(true);
				AlignShipInfo.SetActive(true);
				ManeuverTimeLeft.text = string.Empty;
			}
		}

		public void CancelManeuver()
		{
			Map.RemoveManeuverCourse();
			ManeuverInProgress.SetActive(false);
			ActivateOther(0);
			(Map.MyShip.MainObject as Ship).CancelManeuver();
		}

		public void CancelWarp()
		{
			if (ManeuverInProgress.activeInHierarchy)
			{
				ParentVessel.FTLEngine.SwitchOff();
				ManeuverDetails.SetActive(true);
			}
			CancelManeuver();
		}

		public void UpdateStartTime(float time)
		{
			Map.WarpManeuver.AddStartTime(time);
		}

		public void UpdateEndTime(float time)
		{
			Map.WarpManeuver.AddEndTime(time);
		}

		public void ToggleFTL()
		{
			if (ParentVessel.FTLEngine.IsSwitchedOn())
			{
				ParentVessel.FTLEngine.SwitchOff();
			}
			else
			{
				ParentVessel.FTLEngine.SwitchOn();
			}
		}

		public void ShowHoverInfo(List<MapObject> mapObjects)
		{
			HoverObjectUi.SetActive(true);
			if (InputManager.GetKey(KeyCode.LeftControl) && Map.SelectedObject != null)
			{
				HoverObjectName.text = Map.SelectedObject.Name.ToUpper() + " - " + mapObjects[0].Name.ToUpper() + "\n" + Localization.Distance + ": " + FormatHelper.DistanceFormat((Map.SelectedObject.TruePosition - mapObjects[0].TruePosition).Magnitude);
				HoverObjectUiAnimator.SetBool("cluster", false);
				HoverObjectUiAnimator.SetBool("object", true);
				return;
			}
			if (mapObjects.Count > 1)
			{
				GroupObject.text = string.Empty;
				foreach (MapObject mapObject in mapObjects)
				{
					Text groupObject = GroupObject;
					groupObject.text = groupObject.text + mapObject.Name.ToUpper() + "\n";
				}
				HoverObjectUiAnimator.SetBool("object", false);
				HoverObjectUiAnimator.SetBool("cluster", true);
				return;
			}
			if (mapObjects[0] is MapObjectFuzzyScan)
			{
				MapObjectFuzzyScan mapObjectFuzzyScan = mapObjects[0] as MapObjectFuzzyScan;
				HoverObjectName.text = Localization.PossibleContacts + ": " + mapObjectFuzzyScan.Vessels.Count + "\n" + Localization.RadarSignature + ": ";
				if (mapObjectFuzzyScan.Vessels.Count > 1)
				{
					Text hoverObjectName = HoverObjectName;
					string text = hoverObjectName.text;
					string[] obj = new string[6] { text, null, null, null, null, null };
					List<SpaceObjectVessel> vessels = mapObjectFuzzyScan.Vessels;
					if (_003C_003Ef__am_0024cacheC == null)
					{
						_003C_003Ef__am_0024cacheC = _003CShowHoverInfo_003Em__C;
					}
					obj[1] = vessels.Min(_003C_003Ef__am_0024cacheC).ToString("0");
					obj[2] = " ";
					obj[3] = Localization.To.ToLower();
					obj[4] = " ";
					List<SpaceObjectVessel> vessels2 = mapObjectFuzzyScan.Vessels;
					if (_003C_003Ef__am_0024cacheD == null)
					{
						_003C_003Ef__am_0024cacheD = _003CShowHoverInfo_003Em__D;
					}
					obj[5] = vessels2.Max(_003C_003Ef__am_0024cacheD).ToString("0");
					hoverObjectName.text = string.Concat(obj);
				}
				else
				{
					HoverObjectName.text += mapObjectFuzzyScan.Vessels[0].RadarSignature.ToString("0");
				}
			}
			else
			{
				HoverObjectName.text = mapObjects[0].Name.ToUpper();
			}
			HoverObjectUiAnimator.SetBool("cluster", false);
			HoverObjectUiAnimator.SetBool("object", true);
		}

		public void HideHoverInfo()
		{
			HoverObjectUiAnimator.SetBool("object", false);
			HoverObjectUiAnimator.SetBool("cluster", false);
		}

		public void CalculateMass()
		{
			float num = ((SelectedWarpStage != 0) ? ParentVessel.FTLEngine.TowingCapacity : 0f);
			float num2 = (float)(ParentVessel.GetCompoundMass() - (double)ParentVessel.Mass);
			if (num2 / num > 0.85f)
			{
				MassFiller.color = Colors.Red;
			}
			else
			{
				MassFiller.color = Colors.White;
			}
			if (num > 0f)
			{
				MassFiller.fillAmount = num2 / num;
				MassValue.text = FormatHelper.CurrentMax(num2, num);
			}
			else
			{
				MassFiller.fillAmount = 0f;
				MassValue.text = Localization.TowingDisabled.ToUpper();
				MassFiller.color = Colors.Red;
			}
			string text = string.Empty;
			foreach (SpaceObjectVessel allVessel in ParentVessel.AllVessels)
			{
				if (!(allVessel == ParentVessel))
				{
					string text2 = text;
					text = text2 + allVessel.Name + " - " + allVessel.Mass.ToString("0.0") + "\n";
				}
			}
			if (text == string.Empty)
			{
				DockedVesselsMass.text = Localization.NoDockedVessels.ToUpper();
			}
			else
			{
				DockedVesselsMass.text = text;
			}
		}

		[CompilerGenerated]
		private static bool _003COnInteract_003Em__0(SubSystem m)
		{
			return m.Type == SubSystemType.FTL;
		}

		[CompilerGenerated]
		private static bool _003CUpdateObjectData_003Em__1(SubSystem m)
		{
			return m.Type == SubSystemType.FTL;
		}

		[CompilerGenerated]
		private static double _003CUpdateSelectedObjectInfo_003Em__2(SpaceObjectVessel m)
		{
			return m.RadarSignature;
		}

		[CompilerGenerated]
		private static double _003CUpdateSelectedObjectInfo_003Em__3(SpaceObjectVessel m)
		{
			return m.RadarSignature;
		}

		[CompilerGenerated]
		private static bool _003CDistressSignals_003Em__4(MapObject m)
		{
			return m.RadarVisibilityType == RadarVisibilityType.Distress;
		}

		[CompilerGenerated]
		private static long _003CAuthorizedVesselsResponseListener_003Em__5(DockedVesselData m)
		{
			return m.GUID;
		}

		[CompilerGenerated]
		private static long _003CAuthorizedVesselsResponseListener_003Em__6(SpaceObjectVessel m)
		{
			return m.GUID;
		}

		[CompilerGenerated]
		private static bool _003CSetWarpCells_003Em__7(SceneMachineryPartSlot m)
		{
			return m.CanAttachItemType(ItemType.MachineryPart, null, MachineryPartType.WarpCell);
		}

		[CompilerGenerated]
		private static int _003CSetWarpCells_003Em__8(SceneMachineryPartSlot m)
		{
			return m.SlotIndex;
		}

		[CompilerGenerated]
		private static bool _003CUpdateManeuver_003Em__9(WarpCellUI m)
		{
			return m.IsSelected && m.Slot.Item != null;
		}

		[CompilerGenerated]
		private static bool _003CUpdateManeuver_003Em__A(WarpCellUI m)
		{
			return m.IsSelected;
		}

		[CompilerGenerated]
		private static int _003CUpdateManeuver_003Em__B(WarpCellUI m)
		{
			return m.Slot.SlotIndex;
		}

		[CompilerGenerated]
		private static double _003CShowHoverInfo_003Em__C(SpaceObjectVessel m)
		{
			return m.RadarSignature;
		}

		[CompilerGenerated]
		private static double _003CShowHoverInfo_003Em__D(SpaceObjectVessel m)
		{
			return m.RadarSignature;
		}
	}
}
