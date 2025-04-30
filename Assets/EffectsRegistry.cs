using Microsoft.Xna.Framework.Graphics;
using RealisticSky.Common.DataStructures;

namespace RealisticSky.Assets;

/// <summary>
///     A centralized registry of all common shaders within the mod.
/// </summary>
public static class EffectsRegistry
{
    private const string EffectsPath = $"{nameof(RealisticSky)}/Assets/Effects";

    public static readonly LazyAsset<Effect> AtmosphereShader = LazyAsset<Effect>.RequestAsync($"{EffectsPath}/AtmosphereShader");

    public static readonly LazyAsset<Effect> CloudShader = LazyAsset<Effect>.RequestAsync($"{EffectsPath}/CloudShader");

    public static readonly LazyAsset<Effect> StarPrimitiveShader = LazyAsset<Effect>.RequestAsync($"{EffectsPath}/StarPrimitiveShader");
}
