using System;
using Unity.Behavior;

[BlackboardEnum]
public enum CustomerState
{
    WalkingToQueue, // Step 2: Walking from Spawn to the Queue line
    Queueing, // Step 3: Waiting in line until the Manager assigns a Seat
    WalkingToSeat, // Step 4: Walking from Queue to the assigned Table
    Ordering, // Step 5: Sitting at table, deciding, showing the UI bubble
    AwaitingFood, // Step 6: Patience ticking, waiting for correct food
    Eating, // Step 7: Consuming the food (Duration timer)
    Reacting, // Step 8: Finishing, spawning coins/stars, leaving a tip
    Leaving, // Step 9: Walking to the Exit door to despawn
}
