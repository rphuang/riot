
namespace Riot
{
    /// <summary>
    /// IIotPoint defines the base interface for any components in RIOT
    /// </summary>
    public interface IIotPoint
    {
        /// <summary>
        /// the ID for the node
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// The parent of this node
        /// </summary>
        IIotPoint Parent { get; set; }
    }
}
