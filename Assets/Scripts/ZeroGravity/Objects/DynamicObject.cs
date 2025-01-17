using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenHellion.Net;
using UnityEngine;
using ZeroGravity.CharacterMovement;
using ZeroGravity.Data;
using ZeroGravity.LevelDesign;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(TransitionTriggerHelper))]
	public class DynamicObject : SpaceObjectTransferable
	{
		public static float SendMovementInterval = 0.1f;

		private float sendMovementTime;

		public Rigidbody rigidBody;

		private GameObject collisionDetector;

		public bool Master = true;

		private float velocityCheckTimer;

		private float takeoverTimer;

		private float movementReceivedTime = -1f;

		private Vector3 movementTargetLocalPosition;

		private Quaternion movementTargetLocalRotation;

		private Vector3 movementVelocity;

		private Vector3 movementAngularVelocity;

		private Collider takeoverTrigger;

		private float lastImpactTime;

		private Vector3 prevPosition;

		[HideInInspector] public Item Item;

		private List<Collider> collidersWithTriggerChanged = new List<Collider>();

		[SerializeField] private bool drawDebugPath;

		public override SpaceObjectType Type => SpaceObjectType.DynamicObject;

		public float Diameter { get; private set; }

		public float Mass => rigidBody.mass;

		public new Vector3 Velocity
		{
			get => rigidBody.velocity;
			set
			{
				if (Master)
				{
					rigidBody.velocity = value;
				}
			}
		}

		public new Vector3 AngularVelocity
		{
			get => rigidBody.angularVelocity;
			set
			{
				if (Master)
				{
					rigidBody.angularVelocity = value;
				}
			}
		}

		public bool IsKinematic => rigidBody.isKinematic;

		public bool IsAttached =>
			Item != null && (Item.InvSlot != null || Item.AttachPoint != null || Parent is DynamicObject);

		public override SpaceObject Parent
		{
			get => base.Parent;
			set
			{
				base.Parent = value;
				if (GetParent<MyPlayer>() != null)
				{
					Master = true;
				}
			}
		}

		private void Awake()
		{
			if (TransitionTrigger == null)
			{
				TransitionTrigger = GetComponent<TransitionTriggerHelper>();
			}

			if (TransitionTrigger == null)
			{
				Debug.LogError("Transition trigger not set for dynamic object" + name + gameObject.scene);
			}

			gameObject.SetLayerRecursively(LayerMask.NameToLayer("DynamicObject"), "FirstPerson", "Triggers");
			rigidBody = GetComponent<Rigidbody>();
			rigidBody.useGravity = false;
			rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			Item = GetComponent<Item>();
			EventSystem.AddListener(typeof(DynamicObjectStatsMessage), DynamicObjectStatsMessageListener);
		}

		private void Start()
		{
		}

		public void SendStatsMessage(DynamicObjectAttachData attachData = null, DynamicObjectStats statsData = null)
		{
			if (attachData != null || statsData != null)
			{
				DynamicObjectStatsMessage dynamicObjectStatsMessage = new DynamicObjectStatsMessage();
				dynamicObjectStatsMessage.Info = new DynamicObjectInfo();
				dynamicObjectStatsMessage.Info.GUID = Guid;
				if (attachData != null)
				{
					dynamicObjectStatsMessage.AttachData = attachData;
				}

				if (statsData != null)
				{
					dynamicObjectStatsMessage.Info.Stats = statsData;
				}

				NetworkController.SendToGameServer(dynamicObjectStatsMessage);
			}
		}

		public void ProcessDynamicObectMovementMessage(DynamicObectMovementMessage mm)
		{
			if (!IsAttached && !(takeoverTimer < 1f))
			{
				Master = false;
				ToggleKinematic(value: true);
				movementReceivedTime = Time.realtimeSinceStartup;
				movementTargetLocalPosition = mm.LocalPosition.ToVector3();
				movementTargetLocalRotation = mm.LocalRotation.ToQuaternion();
				movementVelocity = mm.Velocity.ToVector3();
				movementAngularVelocity = mm.AngularVelocity.ToVector3();
			}
		}

		private bool AreAttachDataSame(DynamicObjectAttachData data)
		{
			return Parent.Type == data.ParentType && Parent.Guid == data.ParentGUID && IsAttached == data.IsAttached;
		}

		private void DynamicObjectStatsMessageListener(NetworkData data)
		{
			DynamicObjectStatsMessage dosm = data as DynamicObjectStatsMessage;
			if (dosm.Info.GUID != Guid)
			{
				return;
			}

			if (dosm.DestroyDynamicObject)
			{
				if (Parent is Pivot && Parent.Type == SpaceObjectType.DynamicObjectPivot)
				{
					Destroy(Parent.gameObject);
				}
				else
				{
					Destroy(gameObject);
				}

				return;
			}

			if (dosm.Info.Stats != null && Item != null)
			{
				Item.ProcesStatsData(dosm.Info.Stats);
			}

			if (dosm.AttachData == null)
			{
				return;
			}

			if ((Item != null && Item.AreAttachDataSame(dosm.AttachData)) ||
			    (Item == null && AreAttachDataSame(dosm.AttachData)))
			{
				return;
			}

			SpaceObject prevParent = Parent;
			if (dosm.AttachData.ParentType == SpaceObjectType.DynamicObjectPivot)
			{
				ArtificialBody parent = GetParent<ArtificialBody>();
				if (parent == null)
				{
					Debug.LogError("Dynamic object exited vessel but we don't know from where. " + Guid + Parent +
						dosm.AttachData.ParentType + dosm.AttachData.ParentGUID);
					return;
				}

				Pivot pivot = World.SolarSystem.GetArtificialBody(Guid) as Pivot;
				if (pivot == null)
				{
					pivot = Pivot.Create(SpaceObjectType.DynamicObjectPivot, Guid, parent, isMainObject: false);
				}

				bool myPlayerIsParent = Parent is MyPlayer;
				if (Item != null)
				{
					Item.AttachToObject(pivot, sendAttachMessage: false);
				}
				else
				{
					Parent = pivot;
					SetParentTransferableObjectsRoot();
					ResetRoomTriggers();
					ToggleActive(isActive: true);
					ToggleEnabled(isEnabled: true, toggleColliders: true);
				}

				Task task = new Task(delegate
				{
					if (!myPlayerIsParent || !Master)
					{
						if (dosm.AttachData.LocalPosition != null)
						{
							transform.localPosition = dosm.AttachData.LocalPosition.ToVector3();
						}

						if (dosm.AttachData.LocalRotation != null)
						{
							transform.localRotation = dosm.AttachData.LocalRotation.ToQuaternion();
						}
					}

					if (Master)
					{
						if (dosm.AttachData.Velocity != null)
						{
							rigidBody.velocity = dosm.AttachData.Velocity.ToVector3();
						}

						if (dosm.AttachData.Torque != null)
						{
							AddTorque(dosm.AttachData.Torque.ToVector3(), ForceMode.Impulse);
						}

						if (dosm.AttachData.ThrowForce != null)
						{
							Vector3 vector = dosm.AttachData.ThrowForce.ToVector3();
							if ((MyPlayer.Instance.CurrentRoomTrigger == null ||
							     !MyPlayer.Instance.CurrentRoomTrigger.UseGravity ||
							     MyPlayer.Instance.CurrentRoomTrigger.GravityForce == Vector3.zero) &&
							    prevParent == MyPlayer.Instance)
							{
								float num = MyPlayer.Instance.rigidBody.mass + Mass;
								AddForce(vector * (MyPlayer.Instance.rigidBody.mass / num), ForceMode.VelocityChange);
								MyPlayer.Instance.rigidBody.AddForce(-vector * (Mass / num), ForceMode.VelocityChange);
							}
							else
							{
								AddForce(vector, ForceMode.Impulse);
							}
						}
					}
				});
				if (Parent is MyPlayer && MyPlayer.Instance.AnimHelper.DropTask != null)
				{
					MyPlayer.Instance.AnimHelper.AfterDropTask = task;
				}
				else
				{
					task.RunSynchronously();
				}
			}
			else if (Parent is Pivot && (dosm.AttachData.ParentType == SpaceObjectType.Ship ||
			                             dosm.AttachData.ParentType == SpaceObjectType.Station ||
			                             dosm.AttachData.ParentType == SpaceObjectType.Asteroid))
			{
				if (!(Parent is Pivot))
				{
					Debug.LogError("Entered vessel but we don't know from where." + Guid + Parent +
						dosm.AttachData.ParentType + dosm.AttachData.ParentGUID);
					return;
				}

				World.SolarSystem.RemoveArtificialBody(Parent as Pivot);
				Destroy(Parent.gameObject);
				Parent = World.GetVessel(dosm.AttachData.ParentGUID);
				if (Item != null)
				{
					Item.AttachToObject(Parent, sendAttachMessage: false);
					return;
				}

				transform.parent = Parent.TransferableObjectsRoot.transform;
				ResetRoomTriggers();
				ToggleActive(isActive: true);
				ToggleEnabled(isEnabled: true, toggleColliders: true);
			}
			else if (Item != null)
			{
				Item.ProcessAttachData(dosm.AttachData, prevParent);
			}
		}

		private void FixedUpdate()
		{
			if (IsDestroying || Guid == 0 || IsAttached)
			{
				return;
			}

			if (Master && sendMovementTime + SendMovementInterval <= Time.realtimeSinceStartup &&
			    !rigidBody.isKinematic)
			{
				sendMovementTime = Time.realtimeSinceStartup;
				SendMovementMessage();
			}

			if (IsInsideSpaceObject && Gravity.IsNotEpsilonZero() && !IsKinematic)
			{
				rigidBody.velocity += Gravity * Time.fixedDeltaTime;
			}
		}

		private void SendMovementMessage()
		{
			DynamicObectMovementMessage dynamicObectMovementMessage = new DynamicObectMovementMessage();
			dynamicObectMovementMessage.GUID = Guid;
			dynamicObectMovementMessage.LocalPosition = transform.localPosition.ToArray();
			dynamicObectMovementMessage.LocalRotation = transform.localRotation.ToArray();
			dynamicObectMovementMessage.Velocity = Velocity.ToArray();
			dynamicObectMovementMessage.AngularVelocity = AngularVelocity.ToArray();
			dynamicObectMovementMessage.ImpactVelocity = ImpactVelocity;
			dynamicObectMovementMessage.Timestamp = Time.fixedTime;
			DynamicObectMovementMessage data = dynamicObectMovementMessage;
			NetworkController.SendToGameServer(data);
			ImpactVelocity = 0f;
			transform.hasChanged = false;
		}

		private void OnCollisionEnter(Collision coli)
		{
			if (!IsAttached && IsKinematic)
			{
				ToggleKinematic(value: false);
				SpaceObjectTransferable componentInParent =
					coli.gameObject.GetComponentInParent<SpaceObjectTransferable>();
				if (componentInParent is MyPlayer)
				{
					Master = true;
					takeoverTimer = 0f;
				}
				else if (componentInParent is DynamicObject && (componentInParent as DynamicObject).Master)
				{
					Master = true;
				}

				AddForce(coli.relativeVelocity, ForceMode.VelocityChange);
			}
		}

		public void ResetRoomTriggers()
		{
			TransitionTrigger.ResetTriggers();
		}

		public void ToggleKinematic(bool value)
		{
			if (!Master && !value)
			{
				value = true;
			}

			if (!value)
			{
				if (takeoverTrigger != null)
				{
					Destroy(takeoverTrigger);
				}

				velocityCheckTimer = 0f;
			}

			rigidBody.isKinematic = value;
		}

		public void ToggleEnabled(bool isEnabled, bool toggleColliders)
		{
			enabled = isEnabled;
			TransitionTrigger.enabled = isEnabled;
			if (toggleColliders || isEnabled)
			{
				if ((bool)OnPlatform && !isEnabled)
				{
					OnPlatform.RemoveFromPlatform(this);
				}

				if (Item != null && Item.CustomCollidereToggle(isEnabled))
				{
					return;
				}

				Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren)
				{
					collider.enabled = isEnabled;
				}
			}

			if (collisionDetector != null)
			{
				collisionDetector.SetActive(isEnabled && !IsAttached);
			}
		}

		public void ToggleTriggerColliders(bool areCollidersTrigger)
		{
			if (areCollidersTrigger)
			{
				Collider[] componentsInChildren = Item.GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren)
				{
					if (!collider.isTrigger)
					{
						if (!collidersWithTriggerChanged.Contains(collider))
						{
							collidersWithTriggerChanged.Add(collider);
						}

						collider.isTrigger = true;
					}
				}
			}
			else
			{
				if (collidersWithTriggerChanged.Count <= 0)
				{
					return;
				}

				foreach (Collider item in collidersWithTriggerChanged)
				{
					item.isTrigger = false;
				}

				collidersWithTriggerChanged.Clear();
			}
		}

		public void ToggleActive(bool isActive)
		{
			gameObject.SetActive(isActive);
			TransitionTrigger.enabled = isActive;
			if (collisionDetector != null)
			{
				collisionDetector.SetActive(isActive && !IsAttached);
			}
		}

		public void AddForce(Vector3 force, ForceMode forceMode)
		{
			if (Master && !IsAttached)
			{
				if (IsKinematic)
				{
					ToggleKinematic(value: false);
				}

				rigidBody.AddForce(force, forceMode);
			}
		}

		public void AddTorque(Vector3 torque)
		{
			if (Master && !IsAttached)
			{
				if (IsKinematic)
				{
					ToggleKinematic(value: false);
				}

				rigidBody.AddTorque(torque);
			}
		}

		public void AddTorque(Vector3 torque, ForceMode forceMode)
		{
			if (Master && !IsAttached && !IsKinematic)
			{
				rigidBody.AddTorque(torque, forceMode);
			}
		}

		public static DynamicObject SpawnDynamicObject(SpawnObjectResponseData data)
		{
			SpawnDynamicObjectResponseData spawnDynamicObjectResponseData = data as SpawnDynamicObjectResponseData;
			return SpawnDynamicObject(spawnDynamicObjectResponseData.Details,
				World.GetObject(spawnDynamicObjectResponseData.Details.AttachData.ParentGUID,
					spawnDynamicObjectResponseData.Details.AttachData.ParentType));
		}

		public static DynamicObject SpawnDynamicObject(DynamicObjectDetails details, SpaceObject parent)
		{
			DynamicObjectData dynamicObjectData = !StaticData.DynamicObjectsDataList.ContainsKey(details.ItemID)
				? null
				: StaticData.DynamicObjectsDataList[details.ItemID];
			if (dynamicObjectData != null)
			{
				return SpawnDynamicObject(details, dynamicObjectData, parent);
			}

			return null;
		}

		private static void SetupCollisionModeChanger(DynamicObject dobj)
		{
			GameObject gameObject = new GameObject("CollisionDetectionModeChanger");
			gameObject.transform.parent = dobj.transform;
			gameObject.transform.Reset();
			Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
			Quaternion rotation = dobj.transform.rotation;
			dobj.transform.rotation = Quaternion.identity;
			Bounds bounds = new Bounds(dobj.transform.position, Vector3.zero);
			MeshRenderer[] componentsInChildren = dobj.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if (meshRenderer.bounds.size.magnitude.IsNotEpsilonZero(1E-05f))
				{
					bounds.Encapsulate(meshRenderer.bounds);
				}
			}

			bounds.center -= dobj.transform.position;
			dobj.transform.rotation = rotation;
			sphereCollider.center = bounds.center;
			sphereCollider.radius = bounds.size.magnitude;
			dobj.Diameter = sphereCollider.radius;
			gameObject.AddComponent<CollisionDetectionModeChanger>();
			gameObject.layer = LayerMask.NameToLayer("Triggers");
			dobj.collisionDetector = gameObject;
		}

		public static DynamicObject SpawnDynamicObject(DynamicObjectDetails details, DynamicObjectData data,
			SpaceObject parent)
		{
			DynamicObject dynamicObject = World.GetDynamicObject(details.GUID);
			try
			{
				if (dynamicObject == null)
				{
					GameObject gameObject = Instantiate(Resources.Load(data.PrefabPath),
						new Vector3(20000f, 20000f, 20000f), Quaternion.identity) as GameObject;
					gameObject.SetActive(value: false);
					dynamicObject = gameObject.GetComponent<DynamicObject>();
					dynamicObject.tag = "Untagged";
					dynamicObject.Guid = details.GUID;
					dynamicObject.name = "DynamicObject_" + details.GUID;
					gameObject.SetActive(value: true);
				}

				if (dynamicObject.Item != null)
				{
					if (details.AttachData != null)
					{
						dynamicObject.Item.ProcessAttachData(details.AttachData);
					}

					if (details.StatsData != null)
					{
						dynamicObject.Item.ProcesStatsData(details.StatsData);
					}
				}

				dynamicObject.Parent = parent;
				if (!dynamicObject.IsAttached)
				{
					dynamicObject.transform.localPosition = details.LocalPosition.ToVector3();
					dynamicObject.transform.localRotation = details.LocalRotation.ToQuaternion();
					dynamicObject.rigidBody.velocity = details.Velocity.ToVector3();
					dynamicObject.rigidBody.angularVelocity = details.AngularVelocity.ToVector3();
				}

				World.AddDynamicObject(dynamicObject.Guid, dynamicObject);
				if (details.ChildObjects != null)
				{
					if (details.ChildObjects.Count > 0)
					{
						foreach (DynamicObjectDetails childObject in details.ChildObjects)
						{
							SpawnDynamicObject(childObject, dynamicObject);
						}

						return dynamicObject;
					}

					return dynamicObject;
				}

				return dynamicObject;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
				Debug.LogErrorFormat("Could not find dynamic object with GUID {0}, path {1}", details.GUID, data.PrefabPath);
				return dynamicObject;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			EventSystem.RemoveListener(typeof(DynamicObjectStatsMessage), DynamicObjectStatsMessageListener);
			World.RemoveDynamicObject(Guid);
			if (MyPlayer.Instance.Inventory != null &&
			    ((object)Item != null ? Item.Slot : null) == MyPlayer.Instance.Inventory.HandsSlot)
			{
				World.InGameGUI.HelmetHud.HandsSlotUpdate();
			}

			CheckNearbyObjects();
		}

		public override void EnterVessel(SpaceObjectVessel vessel)
		{
			if (!IsAttached)
			{
				if (Parent is Pivot && Parent != vessel)
				{
					World.SolarSystem.RemoveArtificialBody(Parent as Pivot);
					Destroy(Parent.gameObject);
				}

				Parent = vessel;
				transform.parent = vessel.TransferableObjectsRoot.transform;
			}
		}

		public override void ExitVessel(bool forceExit)
		{
			if (!IsAttached || forceExit)
			{
				ArtificialBody artificialBody = null;
				artificialBody = !(Parent is SpaceObjectVessel)
					? GetParent<ArtificialBody>()
					: (Parent as SpaceObjectVessel).MainVessel;
				if (artificialBody == null)
				{
					Debug.LogErrorFormat("Cannot exit vessel, cannot find parents artificial body {0}, {1}", name, Guid);
					return;
				}

				Parent = Pivot.Create(SpaceObjectType.DynamicObjectPivot, Guid, artificialBody,
					isMainObject: false);
				SetParentTransferableObjectsRoot();
				SendStatsMessage(new DynamicObjectAttachData
				{
					InventorySlotID = -1111,
					IsAttached = false,
					ParentGUID = Parent.Guid,
					ParentType = Parent.Type,
					LocalPosition = transform.localPosition.ToArray(),
					LocalRotation = transform.localRotation.ToArray()
				});
			}
		}

		public override void DockedVesselParentChanged(SpaceObjectVessel vessel)
		{
			if (IsAttached)
			{
				Debug.LogErrorFormat("Attached object changed parent {0}, {1}, {2}, {3}", Parent.Guid, Parent.Type, vessel.Guid, vessel.Type);
			}

			Parent = vessel;
			transform.parent = vessel.TransferableObjectsRoot.transform;
			SendStatsMessage(new DynamicObjectAttachData
			{
				ParentGUID = vessel.Guid,
				ParentType = vessel.Type,
				LocalPosition = transform.localPosition.ToArray(),
				LocalRotation = transform.localRotation.ToArray()
			});
		}

		public override void OnGravityChanged(Vector3 oldGravity)
		{
			if (oldGravity != Vector3.zero && Gravity.IsEpsilonEqual(Vector3.zero))
			{
				AddForce(
					new Vector3(UnityEngine.Random.Range(0.001f, 0.05f), UnityEngine.Random.Range(0.001f, 0.05f),
						UnityEngine.Random.Range(0.001f, 0.05f)), ForceMode.Impulse);
				AddTorque(new Vector3(UnityEngine.Random.Range(0.001f, 0.05f), UnityEngine.Random.Range(0.001f, 0.05f),
					UnityEngine.Random.Range(0.001f, 0.05f)));
			}
		}

		public override void RoomChanged(SceneTriggerRoom prevRoomTrigger)
		{
			base.RoomChanged(prevRoomTrigger);
		}

		private void Update()
		{
			float num = Time.realtimeSinceStartup - movementReceivedTime;
			if (!IsKinematic)
			{
				if (AngularVelocity.IsEpsilonEqual(Vector3.zero, 0.5f) && Velocity.IsEpsilonEqual(Vector3.zero, 0.1f))
				{
					velocityCheckTimer += Time.deltaTime;
					if (velocityCheckTimer > 1f)
					{
						SendMovementMessage();
						ToggleKinematic(value: true);
					}
				}
				else
				{
					velocityCheckTimer = 0f;
				}
			}
			else if (movementReceivedTime > 0f && num < 1f)
			{
				transform.localPosition = Vector3.Lerp(transform.localPosition, movementTargetLocalPosition,
					Mathf.Pow(num, 0.5f));
				transform.localRotation = Quaternion.Slerp(transform.localRotation,
					movementTargetLocalRotation, Mathf.Pow(num, 0.5f));
			}
			else if (num > 1f && num - Time.deltaTime <= 1f)
			{
				ForceActivate();
			}

			takeoverTimer += Time.deltaTime;
		}

		private void ForceActivate()
		{
			Master = true;
			ToggleKinematic(value: false);
			Velocity = movementVelocity;
			AngularVelocity = movementAngularVelocity;
		}

		public void CheckNearbyObjects(HashSet<DynamicObject> alreadyTraversed = null)
		{
			if (alreadyTraversed == null)
			{
				alreadyTraversed = new HashSet<DynamicObject>();
			}

			if (!alreadyTraversed.Add(this))
			{
				return;
			}

			Renderer componentInChildren = GetComponentInChildren<Renderer>();
			if (componentInChildren == null)
			{
				return;
			}

			Collider[] array =
				Physics.OverlapSphere(transform.position, componentInChildren.bounds.size.magnitude);
			foreach (Collider collider in array)
			{
				DynamicObject componentInParent = collider.GetComponentInParent<DynamicObject>();
				if (componentInParent != null && !componentInParent.IsAttached && componentInParent.IsKinematic)
				{
					ToggleKinematic(value: false);
					componentInParent.CheckNearbyObjects(alreadyTraversed);
				}
			}
		}

		public void SendAttachMessage(SpaceObject newParent, IItemSlot slot, Vector3? localPosition = null,
			Quaternion? localRotation = null, Vector3? impulse = null, Vector3? angularImpulse = null,
			Vector3? velocity = null)
		{
			bool flag = slot != null || newParent is DynamicObject;
			SendStatsMessage(new DynamicObjectAttachData
			{
				IsAttached = flag,
				ParentGUID = newParent.Guid,
				ParentType = newParent.Type,
				ItemSlotID = (short)(slot is ItemSlot ? (slot as ItemSlot).ID : 0),
				InventorySlotID = (short)(!(slot is InventorySlot) ? -1111 : (slot as InventorySlot).SlotID),
				APDetails = !(slot is BaseSceneAttachPoint)
					? null
					: new AttachPointDetails
					{
						InSceneID = (slot as BaseSceneAttachPoint).InSceneID
					},
				LocalPosition = flag || !localPosition.HasValue ? null : localPosition.Value.ToArray(),
				LocalRotation = flag || !localRotation.HasValue ? null : localRotation.Value.ToArray(),
				Velocity = !velocity.HasValue ? null : velocity.Value.ToArray(),
				Torque = !angularImpulse.HasValue ? null : angularImpulse.Value.ToArray(),
				ThrowForce = !impulse.HasValue ? null : impulse.Value.ToArray()
			});
		}
	}
}
