// -----------------------------------------------------------------------
// <copyright file="EscapeCondition.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace EndConditions.Models
{
    using System;
    using System.Collections.Generic;
    using EndConditions.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Defines the thresholds on escapes for win conditions.
    /// </summary>
    [Serializable]
    public class EscapeCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeCondition"/> class.
        /// </summary>
        public EscapeCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeCondition"/> class.
        /// </summary>
        /// <param name="classD"><inheritdoc cref="ClassD"/></param>
        /// <param name="scientists"><inheritdoc cref="Scientists"/></param>
        public EscapeCondition(EscapeRequirement classD, EscapeRequirement scientists)
        {
            ClassD = classD;
            Scientists = scientists;
        }

        /// <summary>
        /// Gets or sets the requirement for ClassD escapes.
        /// </summary>
        public EscapeRequirement ClassD { get; set; } = EscapeRequirement.Default;

        /// <summary>
        /// Gets or sets the requirement for Scientist escapes.
        /// </summary>
        public EscapeRequirement Scientists { get; set; } = EscapeRequirement.Default;

        /// <summary>
        /// Compares the requirements of the condition against the round's data.
        /// </summary>
        /// <param name="roles">The alive roles in the current round.</param>
        /// <returns>Whether the round's current escapes matches the specified parameters.</returns>
        public bool Check(List<string> roles) => CheckRestricted(roles) && CheckRequired(roles);

        /// <inheritdoc />
        public override string ToString() => $"ClassD: {ClassD} | Scientists: {Scientists}";

        private bool CheckRestricted(List<string> roles)
        {
            return ((Round.EscapedDClasses == 0 && !ClassDAlive(roles)) || ClassD != EscapeRequirement.Restricted) &&
                   ((Round.EscapedScientists == 0 && !ScientistAlive(roles)) || Scientists != EscapeRequirement.Restricted);
        }

        private bool CheckRequired(List<string> roles)
        {
            return (Round.EscapedDClasses > 0 || ClassDAlive(roles) || ClassD != EscapeRequirement.Required) &&
                   (Round.EscapedScientists > 0 || ScientistAlive(roles) || Scientists != EscapeRequirement.Required);
        }

        private bool ClassDAlive(List<string> roles) => roles.Contains("ClassD", StringComparison.OrdinalIgnoreCase);

        private bool ScientistAlive(List<string> roles) => roles.Contains("Scientist", StringComparison.OrdinalIgnoreCase);
    }
}