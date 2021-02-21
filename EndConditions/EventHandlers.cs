namespace EndConditions
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventHandlers
    {
        public EventHandlers(Config config) => _config = config;
        private readonly Config _config;

        public static List<Condition> Conditions { get; } = new();

        private readonly Dictionary<string, bool> _escAdditions = new()
        {
            ["-classd"] = false,
            ["+classd"] = false,
            ["-science"] = false,
            ["+science"] = false
        };

        public void OnRoundStart()
        {
            if (_config.RoundEndFf)
            {
                foreach (var player in Player.List)
                    player.IsFriendlyFireEnabled = ServerConsole.FriendlyFire;
            }
        }

        public void OnCheckRoundEnd(EndingRoundEventArgs ev)
        {
            if (!_config.AllowDefaultEndConditions)
            {
                ev.IsAllowed = false;
                ev.IsRoundEnded = false;
            }

            if (Warhead.IsDetonated && _config.EndOnDetonation)
            {
                Log.Debug("Ending the round via warhead detonation.", _config.AllowDebug);
                EndGame(ev, _config.DetonationWinner);
                return;
            }

            _escAdditions["-classd"] = RoundSummary.escaped_ds == 0;
            _escAdditions["+classd"] = RoundSummary.escaped_ds != 0;
            _escAdditions["-science"] = RoundSummary.escaped_scientists == 0;
            _escAdditions["+science"] = RoundSummary.escaped_scientists != 0;

            IEnumerable<string> roles = GetRoles();

            // Pull all the lists from the core dictionary and check em
            foreach (var condition in Conditions.Where(condition => !roles.Except(condition.RoleConditions).Any()))
            {
                try
                {
                    Log.Debug($"Using conditions from condition name: '{condition.Name}'", _config.AllowDebug);

                    // Check for escape conditions
                    string[] splitName = condition.Name.Split(' ');
                    List<string> failedConditions = splitName.Where(escapeCondition => _escAdditions.TryGetValue(escapeCondition, out bool passed) && !passed).ToList();
                    if (failedConditions.Count > 0)
                    {
                        Log.Debug($"Failed at: {string.Join(", ", failedConditions)}", _config.AllowDebug);
                        continue;
                    }

                    Log.Debug("Escape checks passed, ending round.", _config.AllowDebug);
                    EndGame(ev, condition.LeadingTeam);
                    return;
                }
                catch (Exception e)
                {
                    Log.Error($"Exception during final checks: {e.Message}\n{e.StackTrace}");
                }
            }
        }

        private IEnumerable<string> GetRoles()
        {
            foreach (Player ply in Player.List)
            {
                if (string.IsNullOrEmpty(ply.UserId) || ply.Role == RoleType.Spectator || API.BlacklistedPlayers.Contains(ply))
                    continue;

                if (API.ModifiedRoles.ContainsKey(ply))
                {
                    yield return API.ModifiedRoles[ply].ToLower();
                }

                if (ply.CustomInfo == "<color=#FF0000>SCP-035</color>")
                {
                    yield return "scp035";
                }

                if (ply.Role == RoleType.Tutorial && _config.IgnoreTutorials)
                    continue;
                
                yield return ply.Role.ToString().ToLower();
            }
        }

        private void EndGame(EndingRoundEventArgs ev, LeadingTeam leadingTeam)
        {
            ev.LeadingTeam = leadingTeam;
            ev.IsAllowed = true;
            ev.IsRoundEnded = true;

            // Force end because people bothered me to put this in
            Round.ForceEnd();

            API.BlacklistedPlayers.Clear();
            API.ModifiedRoles.Clear();

            Log.Debug($"Force ending with {ev.LeadingTeam} as the lead team.", _config.AllowDebug);

            if (_config.RoundEndFf)
            {
                foreach (var player in Player.List)
                    player.IsFriendlyFireEnabled = true;
            }
        }
    }
}