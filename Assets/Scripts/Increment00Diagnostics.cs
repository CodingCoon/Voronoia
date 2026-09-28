#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

internal static class Increment00Diagnostics
{
    private static readonly Stopwatch RoundTimer = new Stopwatch();
    private static int roundNumber;

    internal static void SceneStarted(int leaderCount)
    {
        UnityEngine.Debug.Log("Basis00 scene start: leaders=" + leaderCount +
            ", reservedMemoryBytes=" + Profiler.GetTotalReservedMemoryLong());
    }

    internal static void RoundStarted(int leaderCount)
    {
        roundNumber++;
        RoundTimer.Restart();
        UnityEngine.Debug.Log("Basis00 round " + roundNumber + " started: leaders=" + leaderCount);
    }

    internal static void RoundFinished(int leaderCount)
    {
        if (!RoundTimer.IsRunning)
            return;

        RoundTimer.Stop();
        UnityEngine.Debug.Log("Basis00 round " + roundNumber + " resolved: elapsedMs=" +
            RoundTimer.Elapsed.TotalMilliseconds.ToString("F2") +
            ", leaders=" + leaderCount +
            ", reservedMemoryBytes=" + Profiler.GetTotalReservedMemoryLong());
    }

    internal static void GeometryFinished(long calculationTicks, long assignmentTicks, int cellCount)
    {
        double tickToMs = 1000.0 / Stopwatch.Frequency;
        UnityEngine.Debug.Log("Basis00 geometry: calculationMs=" +
            (calculationTicks * tickToMs).ToString("F2") +
            ", assignmentMs=" + (assignmentTicks * tickToMs).ToString("F2") +
            ", cells=" + cellCount);
    }
}
#endif
