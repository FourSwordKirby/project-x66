using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DistanceBasedWeightedMovement : _WeightedMovement {
    public _TargetedMovementComponent movementComponent;

    public override float getWeight() {
        return 1 / (movementComponent.getTargetPosition() - getMovement().position).magnitude;
    }

    public override _MovementAlgorithm getMovement() {
        return movementComponent.getMovementAlgorithm();
    }
}

public class CompoundDistanceBasedMovement : _CompoundMovement {
    public List<DistanceBasedWeightedMovement> weightedMovements { get; set; }

    public CompoundDistanceBasedMovement(List<DistanceBasedWeightedMovement> weightedMovements) {
        this.weightedMovements = weightedMovements;
    }

    public override IEnumerable<_WeightedMovement> getWeightedMovements() {
        foreach (DistanceBasedWeightedMovement weightedMovement in weightedMovements) {
            yield return (_WeightedMovement)weightedMovement;
        }
    }
}

public class CompoundDistanceBasedMovementComponent : _MovementComponent {
    public List<DistanceBasedWeightedMovement> weightedMovements;

    private CompoundDistanceBasedMovement movement;

    void Awake() {
        movement = new CompoundDistanceBasedMovement(weightedMovements);
    }

    public override _MovementAlgorithm getMovementAlgorithm() {
        return movement;
    }
}
