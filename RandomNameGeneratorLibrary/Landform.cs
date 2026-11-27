namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// The kind of land a place stands on, which <see cref="FantasyNameGenerator.GenerateRandomLandformPlaceName"/>
    /// uses to pick fitting words.
    /// </summary>
    public enum Landform
    {
        /// <summary>Hills, downs and ridges.</summary>
        Hill,

        /// <summary>Rivers, streams and fords.</summary>
        River,

        /// <summary>Forests, woods and groves.</summary>
        Forest,

        /// <summary>Marshes, fens and bogs.</summary>
        Marsh,

        /// <summary>Coasts, bays and headlands.</summary>
        Coast,

        /// <summary>Mountains, peaks and passes.</summary>
        Mountain,

        /// <summary>Plains, fields and moors.</summary>
        Plain,
    }
}
