using System;
using System.Reflection;
using ExileCore2.PoEMemory.Components;

namespace ReAgent.State;

[Api]
public class VitalsInfo
{
    [Api]
    public Vital HP { get; }

    [Api]
    public Vital ES { get; }

    [Api]
    public Vital Mana { get; }

    [Api]
    public Vital Ward { get; }

    public VitalsInfo(Life lifeComponent)
    {
        HP = Vital.From(lifeComponent.Health);
        ES = Vital.From(lifeComponent.EnergyShield);
        Mana = Vital.From(lifeComponent.Mana);
        Ward = ReadOptionalWard(lifeComponent);
    }

    private static Vital ReadOptionalWard(Life lifeComponent)
    {
        try
        {
            var wardProperty = typeof(Life).GetProperty("Ward", BindingFlags.Instance | BindingFlags.Public);
            var ward = wardProperty?.GetValue(lifeComponent);
            var current = ward?.GetType().GetProperty("Current")?.GetValue(ward);
            var max = ward?.GetType().GetProperty("Max")?.GetValue(ward);
            return new Vital(Convert.ToDouble(current ?? 0), Convert.ToDouble(max ?? 0));
        }
        catch { return new Vital(0, 0); }
    }
}
