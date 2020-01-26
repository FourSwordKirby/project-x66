using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public abstract class _WeightedMovement {
    public abstract _MovementAlgorithm getMovement();
    public abstract float getWeight();
}

public abstract class _CompoundMovement : _MovementAlgorithm {
    public override Vector3 getVelocity() {

        float totalWeight = getWeightedMovements().Select(x => x.getWeight()).Aggregate((x, y) => x + y);

        if (totalWeight == 0) {
            return Vector3.zero;
        }

        Vector3 totalVelocity = Vector3.zero;

        foreach (_WeightedMovement weightedMovement in getWeightedMovements()) {
            _MovementAlgorithm movement = weightedMovement.getMovement();

            if (movement is _KinematicMovement) {
                totalVelocity += movement.getVelocity() * weightedMovement.getWeight();
            }
            else if (movement is _DynamicMovement) {
                ((_DynamicMovement)movement).update();
                totalVelocity += movement.getVelocity() * weightedMovement.getWeight();
            }
        }
        
        totalVelocity /= totalWeight;
        return totalVelocity;
    }

    public abstract IEnumerable<_WeightedMovement> getWeightedMovements();
}
