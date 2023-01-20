using System;
using System.Collections.Generic;
using Luminosity.IO;

namespace ZeroGravity.UI
{
	public class InputDefaultSaverJSON : IInputSaver
	{
		public void Save(SaveData parameters)
		{
			Json.SerializeDataPath(parameters, "Resources/Data/ControlsDefault.json");
		}
	}
}
