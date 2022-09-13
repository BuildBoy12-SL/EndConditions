// -----------------------------------------------------------------------
// <copyright file="EscapeRequirement.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace EndConditions.Enums
{
    /// <summary>
    /// Indicates the required amount of escapes.
    /// </summary>
    public enum EscapeRequirement
    {
        /// <summary>
        /// Indicates there is no requirement.
        /// </summary>
        Default,

        /// <summary>
        /// Indicates there must be at least one escape.
        /// </summary>
        Required,

        /// <summary>
        /// Indicates there should be no escapes.
        /// </summary>
        Restricted,
    }
}