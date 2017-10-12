using System;
using UnityEngine;
using System.Collections;

// T extends SituationStatus
public interface AISituation {

    GameUnit GetOwner();

    void update(float tpf);

    int getCurrentStatus();
}
