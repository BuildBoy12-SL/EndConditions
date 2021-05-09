// -----------------------------------------------------------------------
// <copyright file="Condition.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace EndConditions
{
    using System.Collections.Generic;
    using Exiled.API.Enums;

    /// <summary>
    /// A container to be used to pack and unpack requirements to end the round.
    /// </summary>
    public class Condition
    {
        /// <summary>
        /// Gets or sets the escapes required to end the round.
        /// </summary>
        public List<string> EscapeConditions { get; set; }

        /// <summary>
        /// Gets or sets the team that should win if the condition succeeds.
        /// </summary>
        public LeadingTeam LeadingTeam { get; set; }

        /// <summary>
        /// Gets or sets the name of the condition.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the roles required to end the round.
        /// </summary>
        public List<string> RoleConditions { get; set; }
    }
}