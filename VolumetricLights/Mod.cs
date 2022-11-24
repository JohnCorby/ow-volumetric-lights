using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace VolumetricLights;

public class Mod : ModBehaviour
{
	public static IModHelper Helper;

	private void Start()
	{
		Helper = ModHelper;

		LoadManager.OnCompleteSceneLoad += (_, _) =>
		{
			Helper.Events.Unity.FireOnNextUpdate(Apply);
		};
	}

	public override void Configure(IModConfig config)
	{
		Apply();
	}

	private static void Apply()
	{
		if (LoadManager.GetCurrentScene() is not OWScene.SolarSystem or OWScene.EyeOfTheUniverse) return;

		Helper.Console.WriteLine("applying stuff");
		
		foreach (var camera in Resources.FindObjectsOfTypeAll<Camera>())
		{
			var volumetricLightRenderer = camera.gameObject.GetAddComponent<VolumetricLightRenderer>();
		}
		
		foreach (var light in Resources.FindObjectsOfTypeAll<Light>())
		{
			var volumetricLight = light.gameObject.GetAddComponent<VolumetricLight>();
		}
	}
}
