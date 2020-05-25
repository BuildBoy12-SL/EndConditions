# EndConditions
- Grants the ability to set custom round end conditions using [EXILED](https://github.com/galaxy119/EXILED/).
- Requires the included Newtonsoft.Json.dll and YamlDotNet.dll as dependencies.

## EXILED Configs:
| Config Option | Value Type | Default Value | Description |
|:------------------------:|:----------:|:-------------:|:------------------------------------------:|
| `ec_global` | bool | true | Determines if the server reads from the central config file, otherwise it makes a new one. |

__All other config options are located inside the config.yml file that will generate when the plugin is run for the first time__

## EndConditions Configs:
| Config Option | Value Type | Default Value | Description |
|:------------------------:|:----------:|:-------------:|:------------------------------------------:|
| `enabled` | bool | true | Enables/Disables the plugin. |
| `verbose` | bool | false | Enables/Disables printing of confirmation messages when configured conditions are met. |
| `default` | bool | false | Enables/Disables the use of base game round end conditions. |
| `warheadwinner` | string | none | Determines who will win when the warhead detonates using LeadingTeam names. |
| `ignoretut` | bool | true | Determines if tutorials are calculated as a player. |

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

Credit to Killers0992#8021 for inspiration of configs