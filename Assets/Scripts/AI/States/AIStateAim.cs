using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AIStateAim<T> : State<T>
{
    private AIManager _controller;

    public AIStateAim(AIManager controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        _controller.StartTimer(StatesEnum.Aim);
    }

    public override void Execute()
    {
        Vector3 charPos = _controller.CurrentControlledCharacter.CharacterPosition;

        Debug.DrawLine(charPos, _controller.TargetPosition, Color.cyan);
        Debug.DrawLine(_controller.CurrentControlledCharacter.ProjectileOutPosition, charPos + _controller.CurrentControlledCharacter.AimingDirection * 50, Color.red);
    }

    public override void LateExecute()
    {
        // https://www.forrestthewoods.com/blog/solving_ballistic_trajectories/
        float gravity = Mathf.Abs(Physics.gravity.y);
        float projectileSpeed =  2 * _controller.CurrentControlledCharacter.CharacterData.AbilitiesList[_controller.CurrentControlledCharacter.SelectedAbility].AbilityProjectileBaseSpeed;

        Debug.Log(projectileSpeed);

        Vector3 diff = _controller.TargetPosition - _controller.CurrentControlledCharacter.ProjectileOutPosition;
        Vector3 diffXY = new Vector3(diff.x, diff.y, 0);
        float groundDist = diffXY.magnitude;

        float speed2 = projectileSpeed * projectileSpeed;
        float speed4 = projectileSpeed * projectileSpeed * projectileSpeed * projectileSpeed;
        float y = diff.y;
        float x = groundDist;
        float gx = gravity * x;

        float root = speed4 - gravity * (gravity * x * x + 2 * y * speed2);

        // No solution
        if (root < 0)
        { 
            Debug.Log("No solutions");
            return;
        }

        root = Mathf.Sqrt(root);

        float lowAng = Mathf.Atan2(speed2 - root, gx);
        float highAng = Mathf.Atan2(speed2 + root, gx);
        int numSolutions = lowAng != highAng ? 2 : 1;

        Vector3 groundDir = diffXY.normalized;
        Vector3 s0 = groundDir * Mathf.Cos(lowAng) * projectileSpeed + Vector3.up * Mathf.Sin(lowAng) * projectileSpeed;

        
        if (numSolutions > 1)
            s0 = groundDir * Mathf.Cos(highAng) * projectileSpeed + Vector3.up * Mathf.Sin(highAng) * projectileSpeed;
        

        _controller.CurrentIControlleable.InputAim(s0);
    }

    public override void Sleep()
    {
        _controller.StartTimer(StatesEnum.Fire);
    }
}
