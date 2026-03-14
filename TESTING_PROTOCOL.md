# Tower Defense Testing Protocol (Methodology)

Date: ________________  
Tester: ________________  
Build: ________________

## 1) Functional checks

1. Start game from `Menu` -> phase changes to `Preparation`.
   - Expected: Start panel hides, build menu is visible.
   - Actual: ______________________

2. Build towers only in `Preparation`.
   - Expected: Tower can be placed and gold is reduced.
   - Actual: ______________________

3. Try to build in `Battle`.
   - Expected: Build menu hidden and placing towers is blocked.
   - Actual: ______________________

4. Enemy movement along waypoints.
   - Expected: Enemies follow fixed path and reach base endpoint.
   - Actual: ______________________

5. Targeting logic.
   - Expected: Tower attacks enemy with highest path progress in range.
   - Actual: ______________________

6. Enemy types by rounds.
   - Expected: Early rounds mostly Goblin, then Orc, later Ghost.
   - Actual: ______________________

7. Combat effects.
   - Expected: Projectile flight visible, hit feedback visible, HP bars reduce on hit.
   - Actual: ______________________

8. Base hit feedback.
   - Expected: Base plays hit animation and base HP decreases.
   - Actual: ______________________

9. End conditions.
   - Expected: Victory after final round with HP > 0, Defeat at base HP = 0.
   - Actual: ______________________

10. Restart button.
    - Expected: Current scene reloads correctly.
    - Actual: ______________________

## 2) Audio checks

1. Tower shoot SFX plays on attacks.
2. Projectile impact SFX plays on hit.
3. Enemy death SFX plays on kill.
4. Base hit SFX plays when enemy reaches base.

Expected: no missing references or null errors in Console.  
Actual: ______________________

## 3) Performance / acceptance scenario

Scenario required by methodology:
- `20` towers on map
- at least `50` enemies active in wave

Run steps:
1. Apply stress preset in `EnemySpawner` (or set values manually for 50+).
2. Place up to 20 towers during preparation rounds.
3. Run battle and observe responsiveness.

Acceptance criteria:
1. No long freezes; gameplay remains controllable.
2. Rounds complete correctly.
3. No critical Console errors.

Result: PASS / FAIL  
Notes: ______________________

## 4) WebGL validation

1. Build WebGL release.
2. Run locally with HTTP server.
3. Open in browser and repeat key checks + stress scenario.

Expected:
1. Game loads and runs in browser.
2. Core gameplay works same as in Editor.
3. No critical browser console errors.

Result: PASS / FAIL  
Browser + version: ______________________  
Notes: ______________________
