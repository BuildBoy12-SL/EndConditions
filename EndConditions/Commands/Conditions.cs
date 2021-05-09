// -----------------------------------------------------------------------
// <copyright file="Conditions.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace EndConditions.Commands
{
    using System;
    using System.Text;
    using CommandSystem;
    using NorthwoodLib.Pools;

    /// <summary>
    /// A command to view all saved round conditions.
    /// </summary>
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Conditions : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "conditions";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description { get; } = "Lists all current conditions.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            foreach (Condition condition in EventHandlers.Conditions)
            {
                stringBuilder.AppendLine().AppendLine(condition.Name).AppendLine($"Leading Team: {condition.LeadingTeam}")
                    .AppendLine($"Escape Conditions: {string.Join(", ", condition.EscapeConditions)}")
                    .AppendLine($"Roles: {string.Join(", ", condition.RoleConditions)}");
            }

            response = StringBuilderPool.Shared.ToStringReturn(stringBuilder);
            return true;
        }
    }
}