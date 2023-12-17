using System.Collections;
using UnityEngine;
using GameCreator.Core;
using GameCreator.Characters;

[AddComponentMenu("")]
public class ActionCharacterTeleport : IAction
{
    public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);
    public Transform transformToTeleport; // Ajout d'un nouveau Transform pour la téléportation

    [Space]
    public TargetPosition position = new TargetPosition(TargetPosition.Target.Position);
    public bool rotate = false;

    public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
    {
        // Si un Character est défini, utilisez-le
        Character targetCharacter = this.character.GetCharacter(target);
        if (targetCharacter != null)
        {
            yield return null;
            Vector3 targetPosition = this.position.GetPosition(target);

            switch (this.rotate)
            {
                case true:
                    targetCharacter.characterLocomotion.Teleport(
                        targetPosition,
                        this.position.GetRotation(target)
                    );
                    break;

                case false:
                    targetCharacter.characterLocomotion.Teleport(targetPosition);
                    break;
            }
        }
        // Sinon, téléportez le Transform spécifié
        else if (transformToTeleport != null)
        {
            Vector3 targetPosition = this.position.GetPosition(target);
            transformToTeleport.position = targetPosition;
            if (rotate)
            {
                transformToTeleport.rotation = this.position.GetRotation(target);
            }
        }

        yield return 0;
    }

#if UNITY_EDITOR

    public static new string NAME = "Character/Teleport";
    private const string NODE_TITLE = "Teleport {0} to {1}";

    public override string GetNodeTitle()
    {
        return string.Format(NODE_TITLE, this.character, this.position);
    }

#endif
}
