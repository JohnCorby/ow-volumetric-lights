using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace VolumetricLights;

public class Mod : ModBehaviour
{
	public static IModHelper Helper;
	public static AssetBundle ResourceBundle;

	private static readonly List<VolumetricLightRenderer> _renderers = new();
	private static readonly List<(VolumetricLight, LightShadows)> _lights = new();

	private void Start()
	{
		Helper = ModHelper;

		ResourceBundle = ModHelper.Assets.LoadBundle("volumetriclights");

		LoadManager.OnCompleteSceneLoad += (_, _) =>
		{
			if (LoadManager.GetCurrentScene() is not OWScene.SolarSystem or OWScene.EyeOfTheUniverse) return;

			Helper.Events.Unity.FireOnNextUpdate(() =>
			{
				_renderers.Clear();
				_lights.Clear();

				foreach (var camera in Resources.FindObjectsOfTypeAll<Camera>())
				{
					var volumetricLightRenderer = camera.gameObject.AddComponent<VolumetricLightRenderer>();
					_renderers.Add(volumetricLightRenderer);
				}

				foreach (var light in Resources.FindObjectsOfTypeAll<Light>())
				{
					if (light.type == LightType.Directional) continue;
					if (light.type == LightType.Point && light.cookie) continue;
					if (light.name.StartsWith("ThrusterLight")) continue;

					var volumetricLight = light.gameObject.AddComponent<VolumetricLight>();
					_lights.Add((volumetricLight, light.shadows));
				}

				Apply();
			});
		};
	}

	public override void Configure(IModConfig config) => Apply();

	private static void Apply()
	{
		var resolution = EnumUtils.Parse<VolumetricLightRenderer.VolumtericResolution>(Helper.Config.GetSettingsValue<string>("Resolution"));
		var shadows = Helper.Config.GetSettingsValue<bool>("Shadows");

		foreach (var volumetricLightRenderer in _renderers)
		{
			volumetricLightRenderer.Resolution = resolution;
		}

		foreach (var (volumetricLight, lightShadows) in _lights)
		{
			if (shadows)
			{
				if (volumetricLight.Light.shadows == LightShadows.None)
					volumetricLight.Light.shadows = LightShadows.Soft;
			}
			else
			{
				volumetricLight.Light.shadows = lightShadows;
			}
		}
	}
}
