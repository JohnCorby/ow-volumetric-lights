using OWML.Common;
using OWML.ModHelper;

namespace VolumetricLights;

public class Mod : ModBehaviour
{
	public override void Configure(IModConfig config)
	{
	}

	private static void ApplyLights()
	{
		if (LoadManager.GetCurrentScene() != OWScene.SolarSystem) return;

	}
}
