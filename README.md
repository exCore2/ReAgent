# ReAgent — PoE2

Generic user-authored rule engine for ExileCore2. This is an automation
framework, not a passive overlay.

## Logic

1. Build a read-only `RuleState` snapshot containing player vitals, Life, buffs,
   skills, flasks, charges, monsters, map stats, and UI visibility.
2. Compile rules as Dynamic-LINQ v1 or Roslyn C# v2 and group them by area/state
   conditions.
3. Evaluate enabled groups each frame behind foreground/escape/dead/grace gates.
4. Apply configured effects: overlays, flags/timers, PluginBridge calls,
   keyboard/mouse actions, delayed effects, and optional disconnect actions.
5. Persist profiles/rules locally and isolate compilation/runtime failures per
   rule. Detach callbacks and clear state on reload/dispose.

PoE2-specific behavior includes the two-flask model and data-driven ailment
names. Review every profile before enabling: user rules can send input.

Entity projections fail closed when an ExileCore2 `ValidEntitiesByType` bucket is
not present during startup or an area transition. Roslyn v2 rule contexts are
also unloaded when a rule is rebuilt or the plugin is disposed, limiting stale
collectible assembly accumulation during profile editing and hot reload.

## Status

Build: **PASS**. Classification: **CURRENT_WITH_WARNINGS**; runtime semantics
and safety are profile-dependent.

Documentation: [upstream ReAgent docs](https://excore2.github.io/ReAgent/).
Detailed report: [PoE2 plugin catalog](../../README.md) ·
[audit](../../../docs/plugins/ReAgent/AUDIT.md).
