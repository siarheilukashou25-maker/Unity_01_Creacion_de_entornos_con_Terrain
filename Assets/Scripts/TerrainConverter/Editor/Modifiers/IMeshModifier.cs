using TerrainConverter.Editor.Core;

namespace TerrainConverter.Editor.Modifiers
{
    /// <summary>
    /// A single in-place transformation step applied to mesh data.
    /// New behaviours are added by implementing this interface, without touching the converter.
    /// </summary>
    public interface IMeshModifier
    {
        void Apply(TerrainMeshData meshData);
    }
}
