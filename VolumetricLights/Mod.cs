using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace VolumetricLights;

public class Mod : ModBehaviour
{
	public static AssetBundle ResourceBundle;

	private static readonly HashSet<Light> _lightsWithNoShadows = new();

	private void Start()
	{
		ResourceBundle = ModHelper.Assets.LoadBundle("volumetriclights");

		LoadManager.OnCompleteSceneLoad += (_, _) =>
		{
			ModHelper.Events.Unity.FireOnNextUpdate(() =>
			{
				_lightsWithNoShadows.Clear();
				Apply();
			});
		};
	}

	public override void Configure(IModConfig config)
	{
		ModHelper.Events.Unity.RunWhen(() => ResourceBundle, Apply);
	}

	private void Apply()
	{
		var resolution = EnumUtils.Parse<VolumetricLightRenderer.VolumtericResolution>(ModHelper.Config.GetSettingsValue<string>("Resolution"));
		var shadows = ModHelper.Config.GetSettingsValue<bool>("Shadows");
		var sampleCount = ModHelper.Config.GetSettingsValue<int>("Sample Count");
		var scatteringCoef = ModHelper.Config.GetSettingsValue<float>("Scattering Coefficient");
		var extinctionCoef = ModHelper.Config.GetSettingsValue<float>("Extinction Coefficient");
		var mieG = ModHelper.Config.GetSettingsValue<float>("Mie Scattering");

		foreach (var camera in Resources.FindObjectsOfTypeAll<Camera>())
		{
			var volumetricLightRenderer = camera.gameObject.GetAddComponent<VolumetricLightRenderer>();
			volumetricLightRenderer.Resolution = resolution;
		}

		foreach (var light in Resources.FindObjectsOfTypeAll<Light>())
		{
			if (light.type == LightType.Directional) continue;
			if (light.type == LightType.Point && light.cookie) continue;
			if (light.name.StartsWith("ThrusterLight")) continue;

			var volumetricLight = light.gameObject.GetAddComponent<VolumetricLight>();
			volumetricLight.SampleCount = sampleCount;
			volumetricLight.ScatteringCoef = scatteringCoef;
			volumetricLight.ExtinctionCoef = extinctionCoef;
			volumetricLight.MieG = mieG;

			if (shadows)
			{
				if (light.shadows == LightShadows.None)
				{
					_lightsWithNoShadows.Add(light);
					light.shadows = LightShadows.Hard;
				}
			}
			else
			{
				if (_lightsWithNoShadows.Contains(light)) light.shadows = LightShadows.None;
			}
		}
	}
}
