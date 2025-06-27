using UnityEngine;
using System.Collections;
using UnityEngine.Splines;

public class SplineMover : MonoBehaviour
{
    public SplineContainer spline;
    public GameObject objectToMove;
    public float moveDuration = 3f;

    public void StartMoving()
    {
        StartCoroutine(MoveAlongSpline());
    }

    public IEnumerator MoveAlongSpline()
    {
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            objectToMove.transform.position = spline.EvaluatePosition(t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        objectToMove.transform.position = spline.EvaluatePosition(1f);
    }
}
