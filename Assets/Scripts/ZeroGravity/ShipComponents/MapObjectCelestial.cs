using UnityEngine;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
	public class MapObjectCelestial : MapObject
	{
		public Transform CelestialObjects;

		public Transform ChildObjects;

		public GameObject CelestialVisual;

		public GameObject IconVisual;

		public Transform SphereOfInfluence;

		public Sprite CelestialIcon;

		public override Sprite Icon
		{
			get { return CelestialIcon; }
			set { }
		}

		public override string Description
		{
			get
			{
				string localizedField = Localization.GetLocalizedField("Description");
				string empty = string.Empty;
				string text;
				if (Name == "Hellion")
				{
					text = empty;
					empty = text + "\n<color=#74A1CA>" + Localization.GravityInfluenceRadius + ":</color> " +
					        (Orbit.GravityInfluenceRadius / 1000.0).ToString("0");
				}
				else
				{
					text = empty;
					empty = text + "\n<color=#74A1CA>" + Localization.GravityInfluenceRadius + ":</color> " +
					        (Orbit.GravityInfluenceRadius / 1000.0).ToString("0") + " km";
				}

				text = empty;
				empty = text + "\n<color=#74A1CA>" + Localization.Radius + ":</color> " +
				        (Orbit.CelestialBody.Radius / 1000.0).ToString("0") + " km";
				empty = empty + "\n" + Localization.DefauldMapCelestialDescription;
				if (localizedField != null)
				{
					return localizedField + empty;
				}

				return empty;
			}
			set { }
		}

		public override void UpdateObject()
		{
			Position.position = ObjectPosition;
			Orbits.localScale = Vector3.one * (float)ObjectScale;
			Visual.localScale = Vector3.one * (float)(Radius * ObjectScale) * 2f;
			if (!double.IsInfinity(Orbit.GravityInfluenceRadius))
			{
				SphereOfInfluence.localScale =
					Vector3.one * (float)(Orbit.GravityInfluenceRadius * ObjectScale) * 2f;
			}
		}

		public override void UpdateOrbitColors()
		{
			float num = 8.45228E+09f / Radius * Map.ClosestSunScale / OrbitFadeEnd;
			float num2 = 8.45228E+09f / Radius * Map.ClosestSunScale / OrbitFadeStart;
			Color orbitColor = OrbitColor;
			OrbitAlpha = OrbitFadeCurve.Evaluate(((float)Map.Scale - num2) * 1f / (num - num2));
			orbitColor.a = OrbitAlpha;
			MyOrbitRenderer.startColor = orbitColor;
			MyOrbitRenderer.endColor = orbitColor;
			if (SphereOfInfluence.gameObject.GetComponent<MeshRenderer>().enabled)
			{
				SphereOfInfluence.gameObject.GetComponent<MeshRenderer>().material.SetFloat("_Fade", OrbitAlpha);
			}

			if (Orbit.CelestialBody.Guid != 1 && Orbit.Parent.CelestialBody.Guid != 1)
			{
				if (OrbitAlpha < 0.999f)
				{
					PositionCollider.enabled = true;
					IconVisual.gameObject.SetActive(true);
				}
				else
				{
					PositionCollider.enabled = false;
					IconVisual.gameObject.SetActive(false);
				}
			}
		}

		public override void CreateVisual()
		{
			if (MainObject != null)
			{
				CelestialBody celestialBody = MainObject as CelestialBody;
				GameObject gameObject =
					Instantiate(Resources.Load(CelestialBodyData.NavigationPrefabPath) as GameObject);
				gameObject.SetLayerRecursively("Map");
				gameObject.transform.SetParent(Visual);
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localPosition = Vector3.zero;
				SetOrbit();
				UpdateOrbitPlane();
			}
		}

		public void ShowGravityInfluence()
		{
			SphereOfInfluence.gameObject.GetComponent<Animator>().SetBool("Show", true);
		}

		public void HideGravityInfluence()
		{
			SphereOfInfluence.gameObject.GetComponent<Animator>().SetBool("Show", false);
		}
	}
}
