using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;
public class RouletteController : MonoBehaviour
{
    public WheelGenerator generator;
    [SerializeField] GameObject result;
    [SerializeField] TextMeshProUGUI resultText;

    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioClip spin;
    [SerializeField] private AudioClip end;

    [Header("Wheel Settings")]
    public RectTransform wheel;
    public int segmentCount = 6;

    [Header("Spin Settings")]
    public float spinDuration = 5f;
    public int minRotations = 5;
    public int maxRotations = 10;

    [Header("Result Settings")]
    public int resultIndex = -1;

    private bool isSpinning = false;

    void Start()
    {
        wheel = generator.wheel;
        segmentCount = generator.segments.Count;
    }
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Spin();
        }
    }

    public void Spin()
    {
        if (isSpinning || result.activeSelf || wheel == null ||
            segmentCount <= 0)
            return;

        StartCoroutine(SpinWheel());
    }

    private IEnumerator SpinWheel()
    {
        isSpinning = true;

        float segmentAngle = 360f / segmentCount;

        resultIndex = Random.Range(0, segmentCount);

        int rotations = Random.Range(
            minRotations, maxRotations + 1);

        float startAngle = wheel.localEulerAngles.z;

        float targetAngle = resultIndex * segmentAngle;

        float deltaAngle = Mathf.Repeat(
            targetAngle - startAngle, 360f);

        float totalRotation =
            rotations * 360f + deltaAngle;

        float elapsed = 0f;

        int currentIndex = resultIndex;
        int previousIndex = currentIndex;

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / spinDuration);

            // Smooth deceleration
            float easedT = 1f -
                Mathf.Pow(1f - t, 3f);

            float angle = startAngle +
                totalRotation * easedT;

            wheel.localRotation =
                Quaternion.Euler(0f, 0f, angle);

            currentIndex = Mathf.RoundToInt((angle / segmentAngle)) % segmentCount;

            if(previousIndex != currentIndex)
            { 
                previousIndex = currentIndex;
                audio.PlayOneShot(spin);
            }


            yield return null;
        }

        wheel.localRotation =
            Quaternion.Euler(0f, 0f,
                startAngle + totalRotation);

        isSpinning = false;

        string winner = GetWinningText(resultIndex);
        Debug.Log("Winning segment: " + winner);
        result.SetActive(true);
        resultText.text = winner.ToString();
        audio.PlayOneShot(end);
    }
    public string GetWinningText(int idx)
    {
        if (idx < 0 || generator == null)
            return "";

        return generator.GetSegmentText(idx);
    }
}