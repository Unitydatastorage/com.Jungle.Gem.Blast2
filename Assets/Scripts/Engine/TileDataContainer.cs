namespace MatchThreeEngine
{
	public readonly struct TileDataContainer
	{
		public readonly int X;
		public readonly int Y;

		public readonly int TypeId;

		public TileDataContainer(int x, int y, int typeId)
		{
			X = x;
			Y = y;

			TypeId = typeId;
		}
	}
}
