// -----------------------------------------------------------------------
// <copyright file="Conditions.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace EndConditions.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CommandSystem;
    using EndConditions.Models;
    using Exiled.API.Enums;
    using NorthwoodLib.Pools;

    /// <summary>
    /// A command to view all saved round conditions.
    /// </summary>
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Conditions : ICommand
    {
        /// <inheritdoc/>
        public string Command => "conditions";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => "Lists all current conditions.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            foreach (KeyValuePair<LeadingTeam, List<Condition>> kvp in Plugin.Instance.Config.WinConditions.Conditions)
            {
                foreach (Condition condition in kvp.Value)
                {
                    stringBuilder.AppendLine().AppendLine(condition.Name).AppendLine($"Leading Team: {kvp.Key}")
                        .AppendLine($"Escape Conditions: {condition.EscapeCondition}")
                        .AppendLine($"Roles: {string.Join(", ", condition.RoleConditions)}");
                }
            }

            response = StringBuilderPool.Shared.ToStringReturn(stringBuilder);
            return true;
        }
    }
}