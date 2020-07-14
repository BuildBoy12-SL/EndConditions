# EndConditions
- Grants the ability to set custom round end conditions using [EXILED](https://github.com/galaxy119/EXILED/).
- Requires the included Newtonsoft.Json.dll as a dependency.

## EXILED Configs:
| Config Option | Value Type | Default Value | Description |
|:------------------------:|:----------:|:-------------:|:------------------------------------------:|
| `IsEnabled` | bool | true | Enables/Disables the plugin. |
| `UsesGlobalConfig` | bool | true | Determines if the server reads from the central config file, otherwise it makes a new one. |
| `AllowVerbose` | bool | false | Enables/Disables printing of confirmation messages when configured conditions are met. |
| `AllowDebug` | bool | false | Enables/Disables printing of various debug messages, requires this and Exiled's debug to be enabled. |
| `AllowDefaultEndConditions` | bool | false | Enables/Disables the use of base game round end conditions. |
| `DetonationWinner` | string | none | Determines who will win when the warhead detonates using LeadingTeam names. |
| `IgnoreTutorials` | bool | true | Determines if tutorials are calculated as a player. |

## Leading team names:
- FacilityForces
- ChaosInsurgency
- Anomalies
- Draw

## Class names
- Scp173
- Scp106
- Scp049
- Scp079
- Scp096
- Scp0492
- Scp93953
- Scp93989
- NtfScientist
- ChaosInsurgency
- NtfLieutenant
- NtfCommander
- NtfCadet
- FacilityGuard
- Scientist
- ClassD
- Tutorial