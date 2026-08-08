using System;
using System.Reflection;
using ExileCore2.PoEMemory.Components;

namespace ReAgent.State;

[Api]
public class VitalsInfo
{
    private static readonly PropertyInfo WardProperty =
        typeof(Life).GetProperty("Ward", BindingFlags.Instance | BindingFlags.Public);
    private static readonly PropertyInfo WardCurrentProperty =
        WardProperty?.PropertyType.GetProperty("Current", BindingFlags.Instance | BindingFlags.Public);
    private static readonly PropertyInfo WardMaxProperty =
        WardProperty?.PropertyType.GetProperty("Max", BindingFlags.Instance | BindingFlags.Public);

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
        if (lifeComponent == null || WardProperty == null || WardCurrentProperty == null || WardMaxProperty == null)
        {
            return new Vital(0, 0);
        }

        try
        {
            var ward = WardProperty.GetValue(lifeComponent);
            var current = WardCurrentProperty.GetValue(ward);
            var max = WardMaxProperty.GetValue(ward);
            return new Vital(Convert.ToDouble(current ?? 0), Convert.ToDouble(max ?? 0));
        }
        catch (TargetInvocationException) { return new Vital(0, 0); }
        catch (ArgumentException) { return new Vital(0, 0); }
        catch (InvalidCastException) { return new Vital(0, 0); }
        catch (FormatException) { return new Vital(0, 0); }
        catch (OverflowException) { return new Vital(0, 0); }
    }
}
