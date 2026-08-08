using System;
using System.Collections.Generic;

namespace ReAgent.State;

/// <summary>
/// Pure PoE2 flask-type mapping. Memory reads stay in <see cref="FlaskInfo"/>
/// while this stable semantic projection can be verified without a host.
/// </summary>
public static class FlaskLayoutClassifier
{
    private static readonly string[] LifeFlaskBuffs = ["flask_effect_life"];
    private static readonly string[] ManaFlaskBuffs =
    [
        "flask_effect_mana",
        "flask_effect_mana_not_removed_when_full",
        "flask_instant_mana_recovery_at_end_of_effect"
    ];
    private static readonly string[] HybridFlaskBuffs =
    [
        "flask_effect_life",
        "flask_effect_mana",
        "flask_effect_mana_not_removed_when_full",
        "flask_instant_mana_recovery_at_end_of_effect"
    ];

    public static IReadOnlyList<string> GetBuffNames(int flaskType, string customBuff)
    {
        return flaskType switch
        {
            1 => LifeFlaskBuffs,
            2 => ManaFlaskBuffs,
            3 => HybridFlaskBuffs,
            4 when customBuff is { Length: > 0 } => [customBuff],
            _ => Array.Empty<string>()
        };
    }
}
