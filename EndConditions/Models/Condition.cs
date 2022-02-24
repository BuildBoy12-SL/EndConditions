// -----------------------------------------------------------------------
// <copyright file="Condition.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace EndConditions.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A container to be used to pack and unpack requirements to end the round.
    /// </summary>
    [Serializable]
    public class Condition
    {
        /// <summary>
        /// Gets or sets the name of the condition.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the escapes required to end the round.
        /// </summary>
        public EscapeCondition EscapeCondition { get; set; }

        /// <summary>
        /// Gets or sets the roles required to end the round.
        /// </summary>
        public List<string> RoleConditions { get; set; }

        /// <summary>
        /// Checks if the status of the current round matches the parameters of the condition.
        /// </summary>
        /// <param name="roles">The alive roles in the current round.</param>
        /// <returns>A value indicating whether the round info matches the parameters of the condition.</returns>
        public bool Check(List<string> roles)
        {
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            if (EscapeCondition == null || RoleConditions == null)
                return false;

            return EscapeCondition.Check(roles) && !roles.Except(RoleConditions, StringComparer.OrdinalIgnoreCase).Any();
        }
    }
}