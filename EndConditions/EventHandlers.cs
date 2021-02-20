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
                EndGame(ev, _config.DetonationWinner);
                return;
            }

            _escAdditions["-classd"] = RoundSummary.escaped_ds == 0;
            _escAdditions["+classd"] = RoundSummary.escaped_ds != 0;
            _escAdditions["-science"] = RoundSummary.escaped_scientists == 0;
            _escAdditions["+science"] = RoundSummary.escaped_scientists != 0;

            // Pull all the lists from the core dictionary and check em
            foreach (var condition in Conditions.Where(condition => !GetRoles().Except(condition.RoleConditions).Any()))
            {
                try
                {
                    Log.Debug($"Using conditions from condition name: '{condition.Name}'", _config.AllowDebug);
                    
                    // Check for escape conditions
                    string[] splitName = condition.Name.Split(' ');
                    IEnumerable<string> parsedConditions = splitName.Where(str => _escAdditions.Keys.Contains(str));
                    List<string> failedConditions = parsedConditions.Where(x => !_escAdditions[x]).ToList();
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
            List<string> list = new List<string>();
            foreach (Player ply in Player.List)
            {
                if (ply.Role == RoleType.Spectator || API.BlacklistedPlayers.Contains(ply))
                    continue;

                if (API.ModifiedRoles.ContainsKey(ply))
                {
                    list.Add(API.ModifiedRoles[ply]);
                    continue;
                }

                if (ply.CustomInfo == "<color=#FF0000>SCP-035</color>")
                {
                    list.Add("scp035");
                    continue;
                }

                if (ply.Role == RoleType.Tutorial && _config.IgnoreTutorials)
                    continue;

                list.Add(ply.Role.ToString().ToLower());
            }

            return list;
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