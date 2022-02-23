// -----------------------------------------------------------------------
// <copyright file="EventHandlers.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace EndConditions
{
    using System.Collections.Generic;
    using System.Linq;
    using EndConditions.Models;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using NorthwoodLib.Pools;

    /// <summary>
    /// Contains methods which use events in <see cref="Exiled.Events.Handlers"/>.
    /// </summary>
    public class EventHandlers
    {
        private readonly Plugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlers"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnEndingRound(EndingRoundEventArgs)"/>
        public void OnEndingRound(EndingRoundEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.IsRoundEnded = false;

            if (Warhead.IsDetonated && plugin.Config.EndOnDetonation)
            {
                plugin.Methods.EndGame(ev, plugin.Config.DetonationWinner);
                return;
            }

            List<string> roles = ListPool<string>.Shared.Rent(plugin.Methods.GetRoles());
            foreach (KeyValuePair<LeadingTeam, List<Condition>> kvp in plugin.Config.WinConditions.Conditions)
            {
                if (!kvp.Value.Any(condition => condition.Check(roles)))
                    continue;

                plugin.Methods.EndGame(ev, kvp.Key);
                break;
            }

            ListPool<string>.Shared.Return(roles);
        }
    }
}