using UnityEngine;

namespace ZeroGravity.LevelDesign
{
	public class SceneColliderPlayer : MonoBehaviour, ISceneCollider
	{
		public SceneColliderType Type => SceneColliderType.Player;
	}
}
