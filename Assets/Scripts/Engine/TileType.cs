using UnityEngine;

namespace MatchThreeEngine
{
	[CreateAssetMenu(menuName = "Match 3 Engine/Tile Type Asset")]
	public sealed class TileType : ScriptableObject
	{
		public int id;

		public int value;

		public Sprite sprite;
	}
}
