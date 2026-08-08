using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore2;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.Shared.Enums;

namespace ReAgent.State;

[Api]
public record FlaskInfo(
    [property: Api] bool Active,
    [property: Api] bool CanBeUsed,
    [property: Api] int Charges,
    [property: Api] int MaxCharges,
    [property: Api] int ChargesPerUse,
    [property: Api] string ClassName,
    [property: Api] string BaseName,
    [property: Api] string UniqueName,
    [property: Api] float CanBeUsedIn)
{
    [Api]
    public string Name => !string.IsNullOrEmpty(UniqueName) ? UniqueName : BaseName;

    public static FlaskInfo From(
        GameController state,
        ServerInventory.InventSlotItem flaskItem)
    {
        if (flaskItem?.Address is 0 or null || flaskItem.Item?.Address is null or 0)
        {
            return new FlaskInfo(false, false, 0, 1, 1, "", "", "", 100);
        }

        var active = false;
        var canBeUsedIn = 0f;
        bool canbeUsed = false;
        var chargeComponent = flaskItem.Item.GetComponent<Charges>();
        if (state.Player.TryGetComponent<Buffs>(out var playerBuffs))
        {
            if (flaskItem.Item.TryGetComponent<Flask>(out var flask))
            {
                var buffNames = GetFlaskBuffNames(flask);
                active = buffNames.Any(playerBuffs.HasBuff);
                canbeUsed = (chargeComponent?.NumCharges ?? 0) >= (chargeComponent?.ChargesPerUse ?? 1);
            }
        }

        var className = "";
        var baseName = "";
        if (flaskItem.Item.TryGetComponent<Base>(out var baseC))
        {
            className = baseC.Info?.BaseItemTypeDat?.ClassName ?? "";
            baseName = baseC.Name;
        }

        var uniqueName = "";
        if (flaskItem.Item.TryGetComponent<Mods>(out var mods))
        {
            uniqueName = mods.UniqueName;
        }

        return new FlaskInfo(active, canbeUsed, chargeComponent?.NumCharges ?? 0, chargeComponent?.ChargesMax ?? 1, chargeComponent?.ChargesPerUse ?? 1, className, baseName, uniqueName, canBeUsedIn);
    }

    private static readonly string[] LifeFlaskBuffs = { "flask_effect_life" };

    private static readonly string[] ManaFlaskBuffs =
    {
        "flask_effect_mana",
        "flask_effect_mana_not_removed_when_full",
        "flask_instant_mana_recovery_at_end_of_effect"
    };

    // These are the only host members currently available for distinguishing
    // life/mana/hybrid flasks in the pinned PoE2 preview. Keep them named and
    // fail closed so a future layout change cannot turn a stale read into a
    // false-positive automation trigger.
    private const int FlaskTypePointerOffset = 0x28;
    private const int FlaskTypeValueOffset = 0x20;
    private const int CustomBuffPointerOffset = 0x18;
    private const int CustomBuffPointerIndex = 0x0;

    private static IEnumerable<string> GetFlaskBuffNames(Flask flask)
    {
        try
        {
            var type = flask.M.Read<int>(flask.Address + FlaskTypePointerOffset, FlaskTypeValueOffset);
            return type switch
            {
                1 => LifeFlaskBuffs,
                2 => ManaFlaskBuffs,
                3 => LifeFlaskBuffs.Concat(ManaFlaskBuffs),
                4 when flask.M.ReadStringU(flask.M.Read<long>(flask.Address + FlaskTypePointerOffset, CustomBuffPointerOffset, CustomBuffPointerIndex)) is { } s and not "" => new[] { s },
                _ => Enumerable.Empty<string>()
            };
        }
        catch
        {
            // A stale/unknown memory layout must disable classification, not
            // break the rule-state snapshot or execute a wrong flask rule.
            return Enumerable.Empty<string>();
        }
    }
}
