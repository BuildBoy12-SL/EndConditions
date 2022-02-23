// -----------------------------------------------------------------------
// <copyright file="WinConditionsConfig.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace EndConditions.Configs
{
    using System.Collections.Generic;
    using EndConditions.Enums;
    using EndConditions.Models;
    using Exiled.API.Enums;

    /// <summary>
    /// Handles configs in relation to win conditions.
    /// </summary>
    public class WinConditionsConfig
    {
        /// <summary>
        /// Gets or sets the conditions to meet in order to end a game.
        /// </summary>
        public Dictionary<LeadingTeam, List<Condition>> Conditions { get; set; } = new Dictionary<LeadingTeam, List<Condition>>()
        {
            [LeadingTeam.Draw] = new List<Condition>
            {
                new Condition
                {
                    Name = "Chaos Draw",
                    EscapeCondition = new EscapeCondition(EscapeRequirement.Restricted, EscapeRequirement.Required),
                    RoleConditions = new List<string>
                    {
                        "ChaosConscript",
                        "ChaosRifleman",
                        "ChaosRepressor",
                        "ChaosMarauder",
                    },
                },
                new Condition
                {
                    Name = "MTF Draw",
                    EscapeCondition = new EscapeCondition(EscapeRequirement.Default, EscapeRequirement.Restricted),
                    RoleConditions = new List<string>
                    {
                        "NtfCaptain",
                        "NtfSergeant",
                        "NtfPrivate",
                        "NtfSpecialist",
                        "FacilityGuard",
                        "Scientist",
                    },
                },
                new Condition
                {
                    Name = "SCP Draw",
                    EscapeCondition = new EscapeCondition(EscapeRequirement.Restricted, EscapeRequirement.Default),
                    RoleConditions = new List<string>
                    {
                        "Scp173",
                        "Scp106",
                        "Scp049",
                        "Scp079",
                        "Scp096",
                        "Scp0492",
                        "Scp93953",
                        "Scp93989",
                    },
                },
            },
            [LeadingTeam.FacilityForces] = new List<Condition>
            {
                new Condition
                {
                    Name = "MTF Victory",
                    EscapeCondition = new EscapeCondition(EscapeRequirement.Default, EscapeRequirement.Required),
                    RoleConditions = new List<string>
                    {
                        "NtfCaptain",
                        "NtfSergeant",
                        "NtfPrivate",
                        "NtfSpecialist",
                        "FacilityGuard",
                        "Scientist",
                    },
                },
            },
            [LeadingTeam.Anomalies] = new List<Condition>
            {
                new Condition
                {
                    Name = "SCP Victory",
                    EscapeCondition = new EscapeCondition(EscapeRequirement.Restricted, EscapeRequirement.Default),
                    RoleConditions = new List<string>
                    {
                        "Scp173",
                        "Scp106",
                        "Scp049",
                        "Scp079",
                        "Scp096",
                        "Scp0492",
                        "Scp93953",
                        "Scp93989",
                        "ChaosConscript",
                        "ChaosRifleman",
                        "ChaosRepressor",
                        "ChaosMarauder",
                    },
                },
            },
            [LeadingTeam.ChaosInsurgency] = new List<Condition>
            {
                new Condition
                {
                    Name = "Chaos Victory",
                    EscapeCondition = new EscapeCondition(EscapeRequirement.Required, EscapeRequirement.Default),
                    RoleConditions = new List<string>
                    {
                        "ClassD",
                        "ChaosConscript",
                        "ChaosRifleman",
                        "ChaosRepressor",
                        "ChaosMarauder",
                    },
                },
                new Condition
                {
                    Name = "Chaos Victory No ClassD",
                    EscapeCondition = new EscapeCondition(EscapeRequirement.Required, EscapeRequirement.Default),
                    RoleConditions = new List<string>
                    {
                        "ChaosConscript",
                        "ChaosRifleman",
                        "ChaosRepressor",
                        "ChaosMarauder",
                        "Scp173",
                        "Scp106",
                        "Scp049",
                        "Scp079",
                        "Scp096",
                        "Scp0492",
                        "Scp93953",
                        "Scp93989",
                    },
                },
            },
        };
    }
}