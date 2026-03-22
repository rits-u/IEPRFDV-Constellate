using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class TargetGroupHelper : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup group;
    [SerializeField] private float transitionDuration = 1.5f;


    public void AddTargetSmooth(Transform target)
    {
        group.AddMember(target, 0f, 0f);
        StartCoroutine(LerpWeight(target, 1f, transitionDuration));
    }

    public void RemoveTargetSmooth(Transform target)
    {
        StartCoroutine(RemoveRoutine(target, transitionDuration));
    }

    private IEnumerator LerpWeight(Transform target, float targetWeight, float duration)
    {
        int index = FindIndex(target);
        if (index == -1) yield break;

        float startWeight = group.Targets[index].Weight;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            index = FindIndex(target);
            if (index == -1) yield break;

            var member = group.Targets[index];
            member.Weight = Mathf.Lerp(startWeight, targetWeight, t / duration);
            group.Targets[index] = member;

            yield return null;
        }

        //set final weight
        index = FindIndex(target);
        if (index != -1)
        {
            var finalMember = group.Targets[index];
            finalMember.Weight = targetWeight;
            group.Targets[index] = finalMember;
        }
    }

    private IEnumerator RemoveRoutine(Transform target, float duration)
    {
        //lerp
        yield return LerpWeight(target, 0f, duration);

        group.RemoveMember(target);
    }

    private int FindIndex(Transform target)
    {
        for (int i = 0; i < group.Targets.Count; i++)
        {
            if (group.Targets[i].Object == target)
                return i;
        }

        return -1;
    }
}