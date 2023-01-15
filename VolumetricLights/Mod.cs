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

	private static readonly HashSet<Light> _lightsWithNoShadows = new();

	private void Start()
	{
		Helper = ModHelper;

		ResourceBundle = ModHelper.Assets.LoadBundle("volumetriclights");

		LoadManager.OnCompleteSceneLoad += (_, _) =>
		{
			Helper.Events.Unity.FireOnNextUpdate(() =>
			{
				_lightsWithNoShadows.Clear();
				Apply();
			});
		};
	}

	public override void Configure(IModConfig config) => Apply();

	private static void Apply()
	{
		var resolution = EnumUtils.Parse<VolumetricLightRenderer.VolumtericResolution>(Helper.Config.GetSettingsValue<string>("Resolution"));
		var shadows = Helper.Config.GetSettingsValue<bool>("Shadows");
		var sampleCount = Helper.Config.GetSettingsValue<int>("Sample Count");
		var scatteringCoef = Helper.Config.GetSettingsValue<float>("Scattering Coefficient");
		var extinctionCoef = Helper.Config.GetSettingsValue<float>("Extinction Coefficient");
		var mieG = Helper.Config.GetSettingsValue<float>("Mie Scattering");

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
				if (_lightsWithNoShadows.Contains(light))
				{
					light.shadows = LightShadows.None;
				}
			}
		}
	}
}
