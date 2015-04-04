﻿namespace Pulsar4X.ECSLib.DataBlobs
{
    /// <summary>
    /// Contains info on the hanger space in the ship.
    /// </summary>
    public class HangerDB : BaseDataBlob
    {
        /// <summary>
        /// Total amount of hanger space in the ship, in tons.
        /// </summary>
        public int HangerSpace { get; set; }
    }
}