using System.Linq;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
	public class MachineryPart : Item
	{
		public MachineryPartType PartType;

		public string PartName
		{
			get
			{
				return PartType.ToLocalizedString();
			}
		}

		public string PartDescription
		{
			get
			{
				string value;
				if (Localization.MachineryPartsDescriptions.TryGetValue(PartType, out value))
				{
					return value;
				}
				return string.Empty;
			}
		}

		public override DynamicObjectAuxData GetAuxData()
		{
			MachineryPartData baseAuxData = GetBaseAuxData<MachineryPartData>();
			baseAuxData.PartType = PartType;
			baseAuxData.MaxHealth = base.MaxHealth;
			baseAuxData.Health = base.Health;
			baseAuxData.AuxValues = AuxValues.ToArray();
			return baseAuxData;
		}

		public override void ProcesStatsData(DynamicObjectStats dos)
		{
			base.ProcesStatsData(dos);
			MachineryPartStats machineryPartStats = dos as MachineryPartStats;
			UpdateHealthIndicator(base.Health, base.MaxHealth);
		}
	}
}
