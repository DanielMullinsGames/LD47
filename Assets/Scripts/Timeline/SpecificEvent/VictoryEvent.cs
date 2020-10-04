using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryEvent : TimelineEvent
{
    public List<TimelineEvent> preBattleEvents;
    public List<TimelineEvent> postBattleEvents;

    public Animator flag;

    protected override void ResetToStart()
    {
        flag.Play("flag_wave");
    }

    protected override void ResetToEnd()
    {
        flag.Play("flag_wave_changed");
    }

    protected override IEnumerator EventSequence()
    {
        PlayerController.Instance.enabled = false;
        yield return new WaitForSeconds(1f);

        flag.SetTrigger("change");
        yield return new WaitForSeconds(2f);

        if (!TutorialProgress.victory)
        {
            yield return TimelineBar.Instance.ExpandEntireTimeline();

            var newEvents = new List<TimelineEvent>();
            newEvents.AddRange(preBattleEvents);
            newEvents.AddRange(TimelineController.Instance.events);
            newEvents.AddRange(postBattleEvents);

            TimelineController.Instance.events = newEvents;
            TimelineController.Instance.currentPositionIndex = TimelineController.Instance.events.IndexOf(this);
            TimelineController.Instance.RangeStartIndex = preBattleEvents.Count;

            TutorialProgress.victory = true;
        }
    }
}