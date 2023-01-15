using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace VolumetricLights;

public class Mod : ModBehaviour
{
	public static IModHelper Helper;

	public static AssetBundle ResourceBundle;

	private void Start()
	{
		Helper = ModHelper;

		LoadManager.OnCompleteSceneLoad += (_, _) =>
		{
			Helper.Events.Unity.FireOnNextUpdate(Apply);
		};

		ResourceBundle = ModHelper.Assets.LoadBundle("volumetric");

		foreach (var item in ResourceBundle.GetAllAssetNames())
		{
			ModHelper.Console.WriteLine(item);
		}

		foreach (var item in ResourceBundle.GetAllScenePaths())
		{
			ModHelper.Console.WriteLine(item);
		}
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
			if (light.type == LightType.Point && light.cookie != null)
			{
				continue;
			}

			if (light.type == LightType.Directional)
			{
				continue;
			}

			var volumetricLight = light.gameObject.GetAddComponent<VolumetricLight>();

			if (light.shadows == LightShadows.None)
			{
				light.shadows = LightShadows.Soft;
			}
		}
	}
}
