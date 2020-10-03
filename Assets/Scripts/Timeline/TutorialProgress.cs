using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProgress : MonoBehaviour
{
	public enum Mechanic
    {
        Ducking,
        Throwing,
        ThrowingUp,
        Attacking,
        PickingUp,
    }

    private static List<Mechanic> learnedMechanics = new List<Mechanic>();

    public static void LearnMechanic(Mechanic mechanic)
    {
        if (!learnedMechanics.Contains(mechanic))
        {
            learnedMechanics.Add(mechanic);
        }
    }

    public static bool SetMechanicLearned(Mechanic mechanic)
    {
        return learnedMechanics.Contains(mechanic);
    }
}