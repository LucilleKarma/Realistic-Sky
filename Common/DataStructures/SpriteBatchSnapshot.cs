using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RealisticSky.Common.DataStructures;

/// <summary>
///     Contains the data for a <see cref="SpriteBatch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix)"/> call.
///     <br/>This implementation requires use of a publicizer, I've chosen to use krafs as it has the simplest setup I've found. 
///     <br/>You can find this in the .csproj file.
/// </summary>
public readonly struct SpriteBatchSnapshot
{
    private static readonly Matrix identityMatrix = Matrix.Identity;

    public SpriteSortMode sortMode { get; }
    public BlendState blendState { get; }
    public SamplerState samplerState { get; }
    public DepthStencilState depthStencilState { get; }
    public RasterizerState rasterizerState { get; }
    public Effect? effect { get; }
    public Matrix transformationMatrix { get; }

    public SpriteBatchSnapshot(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState? blendState = null, SamplerState? samplerState = null, DepthStencilState? depthStencilState = null, RasterizerState? rasterizerState = null, Effect? effect = null, Matrix? transformationMatrix = null)
    {
        this.sortMode = sortMode;
        this.blendState = blendState ?? BlendState.AlphaBlend;
        this.samplerState = samplerState ?? SamplerState.LinearClamp;
        this.depthStencilState = depthStencilState ?? DepthStencilState.None;
        this.rasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;
        this.effect = effect;
        this.transformationMatrix = transformationMatrix ?? identityMatrix;
    }

    /// <summary>
    /// Pull all the parameters from the last <see cref="SpriteBatch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix)"/> call and create a new <see cref="SpriteBatchSnapshot"/> instance. 
    /// </summary>
    /// <param name="spriteBatch">The target <see cref="SpriteBatch"/> to pull data from.</param>
    /// <returns>A new <see cref="SpriteBatchSnapshot"/> instance with the parameters of the last <see cref="SpriteBatch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix)"/> call.</returns>
    public static SpriteBatchSnapshot Capture(SpriteBatch spriteBatch)
    {
        SpriteSortMode sortMode = spriteBatch.sortMode;
        BlendState blendState = spriteBatch.blendState;
        SamplerState samplerState = spriteBatch.samplerState;
        DepthStencilState depthStencilState = spriteBatch.depthStencilState;
        RasterizerState rasterizerState = spriteBatch.rasterizerState;
        Effect effect = spriteBatch.customEffect;
        Matrix transformMatrix = spriteBatch.transformMatrix;

        return new SpriteBatchSnapshot(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
    }
}
