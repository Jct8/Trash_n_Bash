namespace SheetCodes
{
	//Generated code, do not edit!

	public enum UpgradesIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("More Projectiles 1")] MoreProjectiles1 = 1,
		[Identifier("More Projectiles 2")] MoreProjectiles2 = 2,
		[Identifier("Long Ranged 1")] LongRanged1 = 3,
		[Identifier("Barricade Cost Reduction 1")] BarricadeCostReduction1 = 4,
		[Identifier("Target Specific Enemies 1")] TargetSpecificEnemies1 = 5,
		[Identifier("Fire Projectiles 1")] FireProjectiles1 = 6,
		/*IDENTIFIERS*/
	}

	public static class UpgradesIdentifierExtension
	{
		public static UpgradesRecord GetRecord(this UpgradesIdentifier identifier)
		{
			return ModelManager.UpgradesModel.GetRecord(identifier);
		}
	}
}
