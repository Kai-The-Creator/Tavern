using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using _Core._Combat;
using _Core._Combat.Services;

public class CombatFlowTests
{
    [UnityTest]
    public IEnumerator VictoryWhenAllEnemiesDead() => UniTask.ToCoroutine(async () =>
    {
        var serviceGO = new GameObject();
        var service = serviceGO.AddComponent<CombatService>();
        service.OnStart().Forget();

        var cfg = ScriptableObject.CreateInstance<BattleConfig>();
        var playerGO = new GameObject("Player");
        var player = playerGO.AddComponent<PlayerEntity>();
        var enemyGO = new GameObject("Enemy");
        var enemy = enemyGO.AddComponent<EnemyEntity>();

        service.Configure(cfg, new List<CombatEntity>{player, enemy});
        enemy.Resources.Health = 0;
        var cts = new System.Threading.CancellationTokenSource();
        var battleTask = service.StartBattle(cts.Token);
        await UniTask.DelayFrame(2);
        cts.Cancel();
        Assert.AreEqual(BattleState.Victory, service.State);
    });
}
