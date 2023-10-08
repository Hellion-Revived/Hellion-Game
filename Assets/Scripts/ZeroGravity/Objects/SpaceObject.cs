using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenHellion;
using OpenHellion.Net;
using UnityEngine;
using ZeroGravity.Math;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
	public abstract class SpaceObject : MonoBehaviour
	{
		private SpaceObject _parent;

		protected static World World;

		[HideInInspector] public GameObject TransferableObjectsRoot;

		[HideInInspector] public GameObject ConnectedObjectsRoot;

		[HideInInspector] public GameObject GeometryRoot;

		[HideInInspector] public GameObject GeometryPlaceholder;

		[HideInInspector] public Rigidbody ArtificalRigidbody;

		public bool IsInVisibilityRange = true;

		public Quaternion? TargetRotation;

		[NonSerialized] public bool DelayedSubscribeRequested;

		private GameObject _otherCharacterGeometryRoot;

		protected bool _isDummyObject;

		private Vector3 _forward = Vector3.forward;

		private Vector3 _up = Vector3.up;

		private Vector3 _angularVelocity = Vector3.zero;

		public Vector3D RotationVec = Vector3D.Zero;

		public long GUID { get; set; }

		public virtual SpaceObjectType Type => SpaceObjectType.None;

		public virtual SpaceObject Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		[HideInInspector] public bool IsSubscribedTo { get; protected set; }

		[HideInInspector] public bool SceneObjectsLoaded { get; protected set; }

		[HideInInspector] public bool SpawnRequested { get; private set; }

		public GameObject OtherCharacterGeometryRoot
		{
			get
			{
				if (GeometryPlaceholder != null && _otherCharacterGeometryRoot == null)
				{
					_otherCharacterGeometryRoot = new GameObject("OtherCharacterGeometryRoot");
					_otherCharacterGeometryRoot.transform.parent = GeometryPlaceholder.transform;
					_otherCharacterGeometryRoot.transform.Reset();
				}

				return _otherCharacterGeometryRoot;
			}
		}

		public bool IsDummyObject
		{
			get { return _isDummyObject; }
			protected set
			{
				if (_isDummyObject == value)
				{
					return;
				}

				if (this is SpaceObjectVessel && !_isDummyObject && value)
				{
					SpaceObjectVessel spaceObjectVessel = this as SpaceObjectVessel;
					spaceObjectVessel.DummyDockedVessels = spaceObjectVessel.AllDockedVessels.Select(
						(SpaceObjectVessel m) => new DockedVesselData
						{
							GUID = m.GUID,
							Type = m.Type,
							Data = m.VesselData,
							VesselObjects = null
						}).ToList();
				}

				_isDummyObject = value;
			}
		}

		public virtual Vector3D Position => Vector3D.Zero;

		public virtual Vector3D Velocity => Vector3D.Zero;

		public virtual Vector3 Forward
		{
			get { return _forward; }
			set { _forward = ((!value.IsEpsilonEqual(Vector3.zero, 1E-10f)) ? value : _forward); }
		}

		public virtual Vector3 Up
		{
			get { return _up; }
			set { _up = ((!value.IsEpsilonEqual(Vector3.zero, 1E-10f)) ? value : _up); }
		}

		public bool IsMainObject => MyPlayer.Instance != null && (MyPlayer.Instance.Parent == this ||
		                                                          MyPlayer.Instance.IsInVesselHierarchy(
			                                                          this as SpaceObjectVessel));

		public Vector3 AngularVelocity
		{
			get { return _angularVelocity; }
			set { _angularVelocity = value; }
		}

		protected virtual bool ShouldSetLocalTransform => MyPlayer.Instance == null ||
		                                                  MyPlayer.Instance.Parent != this ||
		                                                  MyPlayer.Instance.Parent is Pivot;

		protected virtual bool ShouldUpdateTransform => true;

		private void Awake()
		{
			World ??= GameObject.Find("/World").GetComponent<World>();
		}

		public virtual void DestroyGeometry()
		{
			IsDummyObject = true;
			SpawnRequested = false;
		}

		public virtual void LoadGeometry()
		{
			IsDummyObject = false;
			RequestSpawn();
		}

		public virtual void OnSubscribe()
		{
		}

		public virtual void OnUnsubscribe()
		{
		}

		public virtual void OnRequestSpawn()
		{
		}

		public virtual void Subscribe()
		{
			if (!IsSubscribedTo)
			{
				StartCoroutine(SubscribeCoroutine());
			}
		}

		private IEnumerator SubscribeCoroutine()
		{
			yield return new WaitUntil(() => SceneObjectsLoaded);
			DelayedSubscribeRequested = false;
			NetworkController.Instance.RequestObjectSubscribe(GUID);
			IsSubscribedTo = true;
			OnSubscribe();
		}

		public virtual void Unsubscribe()
		{
			if (IsSubscribedTo)
			{
				NetworkController.Instance.RequestObjectUnsubscribe(GUID);
				IsSubscribedTo = false;
				OnUnsubscribe();
			}
		}

		public void RequestSpawn()
		{
			if (!SpawnRequested)
			{
				NetworkController.Instance.RequestObjectSpawn(GUID);
				SpawnRequested = true;
				OnRequestSpawn();
			}
		}

		public virtual void ParseSpawnData(SpawnObjectResponseData data)
		{
			SpawnRequested = false;
		}

		public static T GetParent<T>(SpaceObject parent) where T : SpaceObject
		{
			if (parent is null)
			{
				return (T)null;
			}

			if (parent is T spaceObject)
			{
				return spaceObject;
			}

			return GetParent<T>(parent.Parent);
		}

		public T GetParent<T>() where T : SpaceObject
		{
			return GetParent<T>(Parent);
		}

		protected virtual bool PositionAndRotationPhysicsCheck(ref Vector3? nextPos, ref Quaternion? nextRot)
		{
			return true;
		}

		private static void SmoothTime(ref float time, ref List<float> samples, int maxSamples)
		{
			if (samples.Count >= maxSamples)
			{
				samples.RemoveAt(0);
			}

			float num = time + samples.Sum();
			time = num / (samples.Count + 1);
			samples.Add(time);
		}

		public virtual void SetTargetPositionAndRotation(Vector3? localPosition, Quaternion? localRotation,
			bool instant = false, double time = -1.0)
		{
			IsInVisibilityRange = true;
			if (localPosition.HasValue && ShouldSetLocalTransform)
			{
				transform.localPosition = localPosition.Value;
			}

			if (!localRotation.HasValue)
			{
				return;
			}

			if (instant)
			{
				Forward = localRotation.Value * Vector3.forward;
				Up = localRotation.Value * Vector3.up;
				if (ShouldSetLocalTransform)
				{
					base.transform.localRotation = localRotation.Value;
				}

				TargetRotation = null;
			}
			else
			{
				TargetRotation = localRotation.Value;
			}
		}

		public virtual void SetTargetPositionAndRotation(Vector3? localPosition, Vector3? forward, Vector3? up,
			bool instant = false, double time = -1.0)
		{
			if (forward.HasValue && up.HasValue)
			{
				SetTargetPositionAndRotation(localPosition, Quaternion.LookRotation(forward.Value, up.Value), instant,
					time);
			}
			else
			{
				SetTargetPositionAndRotation(localPosition, null, instant, time);
			}
		}

		public virtual void ModifyPositionAndRotation(Vector3? position = null, Quaternion? rotation = null)
		{
			if (IsDummyObject || !IsInVisibilityRange)
			{
				return;
			}

			if (position.HasValue && ShouldSetLocalTransform)
			{
				transform.localPosition += position.Value;
			}

			if (rotation.HasValue)
			{
				if (ShouldSetLocalTransform)
				{
					transform.localRotation *= rotation.Value;
				}

				Quaternion quaternion = Quaternion.LookRotation(Forward, Up) * rotation.Value;
				Forward = quaternion * Vector3.forward;
				Up = quaternion * Vector3.up;
			}

			if ((position.HasValue || rotation.HasValue) && ArtificalRigidbody is not null)
			{
				UpdateArtificialBodyPosition(updateChildren: true);
				transform.hasChanged = false;
			}
		}

		public virtual void UpdateArtificialBodyPosition(bool updateChildren)
		{
			if (ArtificalRigidbody is not null && GeometryPlaceholder is not null)
			{
				GeometryRoot.transform.position = GeometryPlaceholder.transform.position;
				GeometryRoot.transform.rotation = GeometryPlaceholder.transform.rotation;
				ArtificalRigidbody.position = GeometryPlaceholder.transform.position;
				ArtificalRigidbody.rotation = GeometryPlaceholder.transform.rotation;
			}
		}

		protected virtual void UpdatePositionAndRotation(bool setLocalPositionAndRotation)
		{
			if (ArtificalRigidbody is not null && GeometryPlaceholder is not null && base.transform.hasChanged)
			{
				ArtificalRigidbody.position = GeometryPlaceholder.transform.position;
				ArtificalRigidbody.rotation = GeometryPlaceholder.transform.rotation;
				transform.hasChanged = false;
			}
		}
	}
}
