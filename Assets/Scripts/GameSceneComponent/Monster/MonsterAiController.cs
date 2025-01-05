using UnityEngine;
using UnityEngine.AI;

public class MonsterAiController : MonoBehaviour, ITriggerableByPlayer {
    [SerializeField] private float roamRadius = 10f;
    [SerializeField] private float chaseRangeLimit = 25f;
    [SerializeField] private float hearPlayerRange = 2f;

    private enum State {
        Roaming,
        Chasing,
        GoingBackToStart
    }

    private State state;
    private Vector3 startingPosition;
    private Vector3 roamingPosition;
    private NavMeshAgent monsterNavMeshAgent;


    private void Start() {
        monsterNavMeshAgent = GetComponent<NavMeshAgent>();
        startingPosition = transform.position;
        roamingPosition = GetRoamingPosition();
        state = State.Roaming;
    }
    private void Update() {
        switch (state) {
            case State.Roaming:
                monsterNavMeshAgent.destination = roamingPosition;
                float reachedPositionDistance = 1f;
                if (Vector3.Distance(transform.position, roamingPosition) < reachedPositionDistance) {
                    //Reached Roam Position, get new position in room to go to
                    roamingPosition = GetRoamingPosition();
                }
                FindTargetInRange();
                break;
            case State.Chasing:
                monsterNavMeshAgent.destination = Player.Instance.GetPlayerPosition();
                ReachedLimitChaseRange();
                break;
            case State.GoingBackToStart:
                monsterNavMeshAgent.destination = startingPosition;
                reachedPositionDistance = 1f;
                if (Vector3.Distance(transform.position, startingPosition) < reachedPositionDistance) {
                    //Reached Start Position, get new position in room to go to
                    roamingPosition = GetRoamingPosition();
                    state = State.Roaming;
                }
                FindTargetInRange();
                break;
        }
    }

    private Vector3 GetRoamingPosition() {
        float angle = Random.Range(0f, Mathf.PI * 2);
        Vector3 randomDirection = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
        Vector3 randomPositionWithinCircle = startingPosition + randomDirection * Random.Range(0f, roamRadius);
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(randomPositionWithinCircle, out navMeshHit, roamRadius, NavMesh.AllAreas)) {
            return navMeshHit.position;
        }
        return transform.position;
    }

    private void FindTargetInRange() {
        if (Vector3.Distance(transform.position, Player.Instance.GetPlayerPosition()) < hearPlayerRange) {
            //Player within Range, start to chase player
            state = State.Chasing;
        }
    }

    private void ReachedLimitChaseRange() {

        if (Vector3.Distance(transform.position, startingPosition) > chaseRangeLimit) {
            state = State.GoingBackToStart;
        }
    }

    public void Trigger(Player player) {
        DoorGameManager.Instance.Heal(-20);
        Destroy(gameObject);
    }
}
