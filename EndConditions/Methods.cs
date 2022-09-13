// -----------------------------------------------------------------------
// <copyright file="Methods.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace EndConditions
{
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Various required methods.
    /// </summary>
    public class Methods
    {
        private readonly Plugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Methods"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public Methods(Plugin plugin) => this.plugin = plugin;

        /// <summary>
        /// Gets all alive player role names.
        /// </summary>
        /// <returns>A collection of all player role names.</returns>
        public IEnumerable<string> GetRoles()
        {
            foreach (Player player in Player.List)
            {
                if (player.IsDead || API.BlacklistedPlayers.Contains(player))
                    continue;

                if (API.ModifiedRoles.TryGetValue(player, out string modifiedRole))
                {
                    yield return modifiedRole;
                    continue;
                }

                if (player.Role.Type == RoleType.Tutorial && plugin.Config.IgnoreTutorials)
                    continue;

                yield return RoleName(player.Role.Type);
            }
        }

        /// <summary>
        /// Ends the round with the specified parameters.
        /// </summary>
        /// <param name="ev">An instance of <see cref="EndingRoundEventArgs"/>.</param>
        /// <param name="leadingTeam">The team that won.</param>
        public void EndGame(EndingRoundEventArgs ev, LeadingTeam leadingTeam)
        {
            ev.LeadingTeam = leadingTeam;
            ev.IsAllowed = true;
            ev.IsRoundEnded = true;

            API.BlacklistedPlayers.Clear();
            API.ModifiedRoles.Clear();

            if (plugin.Config.RoundEndFf)
                Server.FriendlyFire = true;
        }

        private static string RoleName(RoleType roleType)
        {
            return roleType switch
            {
                RoleType.None => nameof(RoleType.None),
                RoleType.Scp173 => nameof(RoleType.Scp173),
                RoleType.ClassD => nameof(RoleType.ClassD),
                RoleType.Spectator => nameof(RoleType.Spectator),
                RoleType.Scp106 => nameof(RoleType.Scp106),
                RoleType.NtfSpecialist => nameof(RoleType.NtfSpecialist),
                RoleType.Scp049 => nameof(RoleType.Scp049),
                RoleType.Scientist => nameof(RoleType.Scientist),
                RoleType.Scp079 => nameof(RoleType.Scp079),
                RoleType.ChaosConscript => nameof(RoleType.ChaosConscript),
                RoleType.Scp096 => nameof(RoleType.Scp096),
                RoleType.Scp0492 => nameof(RoleType.Scp0492),
                RoleType.NtfSergeant => nameof(RoleType.NtfSergeant),
                RoleType.NtfCaptain => nameof(RoleType.NtfCaptain),
                RoleType.NtfPrivate => nameof(RoleType.NtfPrivate),
                RoleType.Tutorial => nameof(RoleType.Tutorial),
                RoleType.FacilityGuard => nameof(RoleType.FacilityGuard),
                RoleType.Scp93953 => nameof(RoleType.Scp93953),
                RoleType.Scp93989 => nameof(RoleType.Scp93989),
                RoleType.ChaosRifleman => nameof(RoleType.ChaosRifleman),
                RoleType.ChaosRepressor => nameof(RoleType.ChaosRepressor),
                RoleType.ChaosMarauder => nameof(RoleType.ChaosMarauder),
                _ => nameof(RoleType.None)
            };
        }
    }
}