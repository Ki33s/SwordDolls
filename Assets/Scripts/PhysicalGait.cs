using UnityEngine;

// Physical foot placement: targets move, the ragdoll bones are never teleported.
internal sealed class PhysicalGait
{
    sealed class Foot
    {
        public Rigidbody rig;
        public Vector3 planted, start, end, previousTarget;
        public float sole, driveMass;
    }
    readonly Body body;
    readonly Rigidbody hips;
    readonly Foot left, right;
    readonly RaycastHit[] hits = new RaycastHit[24];
    readonly Vector3 localRight, localUp;
    readonly float legLength;
    Vector3 velocity;
    Foot swinging;
    bool nextLeft;
    float phase, duration, releaseUntil;
    public int Steps { get; private set; }
    public float Speed => velocity.magnitude;

    public PhysicalGait(Body body)
    {
        this.body = body;
        var joint = body.upperLegL.GetComponent<Joint>();
        hips = joint && joint.connectedBody ? joint.connectedBody : body.upperLegL.transform.parent.GetComponent<Rigidbody>();
        if (!hips) hips = body.head.GetComponent<Rigidbody>();
        legLength = Mathf.Max(.4f, (Vector3.Distance(body.upperLegL.transform.position, body.lowerLegL.transform.position)
            + Vector3.Distance(body.lowerLegL.transform.position, body.footL.transform.position)
            + Vector3.Distance(body.upperLegR.transform.position, body.lowerLegR.transform.position)
            + Vector3.Distance(body.lowerLegR.transform.position, body.footR.transform.position)) * .5f);
        localRight = hips.transform.InverseTransformDirection((body.upperLegR.transform.position - body.upperLegL.transform.position).normalized);
        localUp = hips.transform.InverseTransformDirection(Vector3.up);
        left = Create(body.footL, body.upperLegL, body.lowerLegL); right = Create(body.footR, body.upperLegR, body.lowerLegR);
    }
    Foot Create(GameObject go, GameObject upper, GameObject lower)
    {
        var collider = go.GetComponent<Collider>();
        var foot = new Foot { rig = go.GetComponent<Rigidbody>(), sole = collider ? Mathf.Clamp(go.transform.position.y - collider.bounds.min.y, .035f, .16f) : .07f };
        foot.driveMass = foot.rig.mass + body.footLegMass * (upper.GetComponent<Rigidbody>().mass + lower.GetComponent<Rigidbody>().mass);
        foot.planted = Ground(go.transform.position, foot.sole);
        foot.previousTarget = foot.rig.position;
        return foot;
    }
    Vector3 Ground(Vector3 point, float sole)
    {
        var origin = new Vector3(point.x, hips.position.y + legLength * .5f, point.z);
        int count = Physics.RaycastNonAlloc(origin, Vector3.down, hits, legLength * 2.5f + .5f, ~0, QueryTriggerInteraction.Ignore);
        float nearest = float.PositiveInfinity;
        Vector3 result = new Vector3(point.x, Mathf.Min(left != null ? left.planted.y : point.y, point.y), point.z);
        for (int i = 0; i < count; i++)
        {
            var hit = hits[i];
            if (hit.normal.y < .6f || hit.collider.GetComponentInParent<Body>() || hit.collider.GetComponentInParent<Sword>() || hit.collider.GetComponentInParent<Weapon>()) continue;
            if (hit.rigidbody && !hit.rigidbody.isKinematic) continue;
            if (hit.distance < nearest) { nearest = hit.distance; result = hit.point + Vector3.up * sole; }
        }
        return result;
    }
    public void Release(float time) { releaseUntil = Time.time + time; swinging = null; }
    public void Tick(float health, bool stay)
    {
        float dt = Time.fixedDeltaTime;
        if (Time.time < releaseUntil)
        {
            left.planted = Ground(left.rig.position, left.sole); right.planted = Ground(right.rig.position, right.sole);
            return;
        }
        velocity = Vector3.Lerp(velocity, Vector3.ProjectOnPlane(hips.linearVelocity, Vector3.up), 1 - Mathf.Exp(-dt * 12));
        Vector3 side = Vector3.ProjectOnPlane(hips.transform.TransformDirection(localRight), Vector3.up).normalized;
        if (side.sqrMagnitude < .1f) side = Vector3.right;
        float speed = velocity.magnitude;
        float run = Mathf.InverseLerp(.5f, 4.5f, speed);
        float period = Mathf.Lerp(body.walkStepTime, body.runStepTime, run);
        float width = Mathf.Clamp(body.stepWidth, .13f, .4f) * legLength;
        Vector3 lead = Vector3.ClampMagnitude(velocity * (body.footPrediction + period * .5f), legLength * body.footReach);
        Vector3 leftTarget = Ground(hips.position - side * width + lead, left.sole);
        Vector3 rightTarget = Ground(hips.position + side * width + lead, right.sole);
        float errorL = Vector3.ProjectOnPlane(leftTarget - left.rig.position, Vector3.up).magnitude;
        float errorR = Vector3.ProjectOnPlane(rightTarget - right.rig.position, Vector3.up).magnitude;
        float threshold = Mathf.Lerp(.24f, .16f, run) * legLength;
        // Replant after a teleport, possession, or a large fall instead of pulling towards an old world-space anchor.
        if (Vector3.Distance(hips.position, left.planted) > legLength * 2.2f || Vector3.Distance(hips.position, right.planted) > legLength * 2.2f)
        {
            left.planted = Ground(left.rig.position, left.sole); right.planted = Ground(right.rig.position, right.sole); swinging = null;
        }
        bool upright = Vector3.Dot(hips.transform.TransformDirection(localUp), Vector3.up) > .25f;
        bool grounded = Mathf.Min(hips.position.y - leftTarget.y, hips.position.y - rightTarget.y) < legLength * 1.3f;
        if (!stay && upright && grounded && swinging == null && Mathf.Max(errorL, errorR) > threshold)
        {
            // Prefer alternation, allowing the trailing leg to recover after a sharp reversal.
            swinging = Mathf.Abs(errorL - errorR) > legLength * .45f ? (errorL > errorR ? left : right) : (nextLeft ? left : right);
            nextLeft = swinging != left;
            swinging.start = swinging.rig.position;
            swinging.end = swinging == left ? leftTarget : rightTarget;
            swinging.previousTarget = swinging.start;
            phase = 0; duration = period; Steps++;
        }
        // Include the landing endpoint instead of ending one physics tick short.
        if (swinging != null) phase = Mathf.Min(1, phase + dt / duration);
        Drive(left, leftTarget, health, run); Drive(right, rightTarget, health, run);
        if (swinging != null)
        {
            if (phase >= 1)
            {
                swinging.planted = swinging.end;
                swinging.previousTarget = swinging.planted;
                swinging = null;
            }
        }
    }
    void Drive(Foot foot, Vector3 nextTarget, float health, float run)
    {
        bool swing = foot == swinging;
        Vector3 target = foot.planted, targetVelocity = Vector3.zero;
        if (swing)
        {
            // Retarget only the first half of a step; landing then stays still on the ground.
            if (phase < .55f) foot.end = Vector3.Lerp(foot.end, nextTarget, .4f);
            float t = Mathf.Clamp01(phase), blend = t * t * (3 - 2 * t);
            target = Vector3.Lerp(foot.start, foot.end, blend) + Vector3.up * (Mathf.Sin(t * Mathf.PI) * body.footLift * Mathf.Lerp(.85f, 1.15f, run));
            targetVelocity = Vector3.ClampMagnitude((target - foot.previousTarget) / Time.fixedDeltaTime, 14);
        }
        Vector3 acceleration = (target - foot.rig.position) * (swing ? body.footSpring : 100)
            + (targetVelocity - foot.rig.linearVelocity) * (swing ? body.footDamping : 15);
        foot.rig.AddForce(Vector3.ClampMagnitude(acceleration, body.maxFootAcceleration) * (swing ? foot.driveMass : foot.rig.mass) * Mathf.Lerp(.55f, 1, health), ForceMode.Force);
        // Never press the swinging foot down: the old controller cancelled its own lift force.
        if (!swing) foot.rig.AddForce(Vector3.down * (body.footsGroundForce * 100 * Time.fixedDeltaTime * health * .65f));
        foot.previousTarget = target;
    }
}
