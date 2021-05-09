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
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using NorthwoodLib.Pools;

    /// <summary>
    /// Contains methods which use events in <see cref="Exiled.Events.Handlers"/>.
    /// </summary>
    public static class EventHandlers
    {
        /// <summary>
        /// Gets the list of conditions to be used to check if the round should end.
        /// </summary>
        public static List<Condition> Conditions { get; } = new List<Condition>();

        /// <summary>
        /// Gets a dictionary to be used as a utility against escaped personnel checks.
        /// </summary>
        internal static Dictionary<string, bool> EscapeTracking { get; } = new Dictionary<string, bool>
        {
            ["-classd"] = false,
            ["+classd"] = false,
            ["-science"] = false,
            ["+science"] = false,
        };

        private static Config Config => Plugin.Instance.Config;

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnEndingRound(EndingRoundEventArgs)"/>
        public static void OnEndingRound(EndingRoundEventArgs ev)
        {
            if (!Config.AllowDefaultEndConditions)
            {
                ev.IsAllowed = false;
                ev.IsRoundEnded = false;
            }

            if (Warhead.IsDetonated && Config.EndOnDetonation)
            {
                Log.Debug("Ending the round via warhead detonation.", Config.AllowDebug);
                EndGame(ev, Config.DetonationWinner);
                return;
            }

            EscapeTracking["-classd"] = RoundSummary.escaped_ds == 0;
            EscapeTracking["+classd"] = RoundSummary.escaped_ds > 0;
            EscapeTracking["-science"] = RoundSummary.escaped_scientists == 0;
            EscapeTracking["+science"] = RoundSummary.escaped_scientists > 0;

            IEnumerable<string> roles = GetRoles();

            // Pull all the lists from the core dictionary and check em
            foreach (Condition condition in Conditions.Where(condition => !roles.Except(condition.RoleConditions).Any()))
            {
                Log.Debug($"Using conditions from condition name: '{condition.Name}'", Config.AllowDebug);

                // Check escape conditions
                List<string> failedConditions = ListPool<string>.Shared.Rent(condition.EscapeConditions.Where(cond => !EscapeTracking[cond]));
                if (failedConditions.Count > 0)
                {
                    Log.Debug($"Escape conditions failed at: {string.Join(", ", failedConditions)}", Config.AllowDebug);
                    ListPool<string>.Shared.Return(failedConditions);
                    continue;
                }

                Log.Debug($"Escape checks passed: {string.Join(", ", condition.EscapeConditions)}", Config.AllowDebug);
                ListPool<string>.Shared.Return(failedConditions);
                EndGame(ev, condition.LeadingTeam);
                return;
            }
        }

        private static IEnumerable<string> GetRoles()
        {
            foreach (Player ply in Player.List)
            {
                if (ply == null ||
                    string.IsNullOrEmpty(ply.UserId) ||
                    ply.Role == RoleType.Spectator ||
                    API.BlacklistedPlayers.Contains(ply))
                {
                    continue;
                }

                if (API.ModifiedRoles.TryGetValue(ply, out string modifiedRole))
                {
                    yield return modifiedRole.ToLower();
                    continue;
                }

                if (ply.SessionVariables.ContainsKey("IsScp035"))
                {
                    yield return "scp035";
                    continue;
                }

                if (ply.Role == RoleType.Tutorial && Config.IgnoreTutorials)
                    continue;

                yield return ply.Role.ToString().ToLower();
            }
        }

        private static void EndGame(EndingRoundEventArgs ev, LeadingTeam leadingTeam)
        {
            ev.LeadingTeam = leadingTeam;
            ev.IsAllowed = true;
            ev.IsRoundEnded = true;

            // Force end because people bothered me to put this in
            Round.ForceEnd();

            API.BlacklistedPlayers.Clear();
            API.ModifiedRoles.Clear();

            Log.Debug($"Force ending with {ev.LeadingTeam} as the lead team.", Config.AllowDebug);

            if (Config.RoundEndFf)
            {
                foreach (var player in Player.List)
                {
                    player.IsFriendlyFireEnabled = true;
                }
            }
        }
    }
}