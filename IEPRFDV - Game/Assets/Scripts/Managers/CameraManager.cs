using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private TargetGroupHelper targetGroupHelper;

    private void Start()
    {
        targetGroupHelper = GetComponent<TargetGroupHelper>();
        // ChangeToIntermissionView();
        ChangeToPlayerView();
    }

    public void ChangeToIntermissionView()
    {
        if (PlayerManager.Instance == null) return;
        List<Transform> transforms = PlayerManager.Instance.GetTransformList();
        

        foreach (Transform t in transforms)
        {
            targetGroupHelper.AddTargetSmooth(t);
        }

        CinemachineGroupFraming frame = GetComponentInChildren<CinemachineGroupFraming>();
        Vector2 temp = frame.OrthoSizeRange;
        frame.OrthoSizeRange = new Vector2(temp.x - 2, temp.y);
    }

    public void ChangeToPlayerView()
    {
        if (PlayerManager.Instance == null) return;
        List<Transform> transforms = PlayerManager.Instance.GetTransformList();

        foreach (Transform t in transforms)
        {
            targetGroupHelper.RemoveTargetSmooth(t);
        }

        CinemachineGroupFraming frame = GetComponentInChildren<CinemachineGroupFraming>();
        Vector2 temp = frame.OrthoSizeRange;
        frame.OrthoSizeRange = new Vector2(temp.x + 2, temp.y);

    }

    public void AddTargetToGroup(Transform transform)
    {
        targetGroupHelper.AddTargetSmooth(transform);
    }

    public void RemoveTargetFromGroup(Transform transform)
    {
        targetGroupHelper.RemoveTargetSmooth(transform);
    }

}
