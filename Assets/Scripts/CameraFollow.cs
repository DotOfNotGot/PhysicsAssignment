using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraFollow : MonoBehaviour
{
    private Camera _camera;
    
    [SerializeField]
    private GolfController currentGolfController;

    private Transform ballTransform;
    private Vector3 cameraNextPosition;
    private Vector3 oldCameraPosition;

    private Vector2 _rotationDirection;
    private Vector3 _rotationAngles;
    
    [Header("Follow Settings")]
    [SerializeField]
    private bool shouldLerp;
    [SerializeField]
    private float waitTime = 3;

    [Header("Rotation")] 
    [SerializeField] 
    private float rotationSpeed = 10;
    
    [Header("Offsets")]
    // [SerializeField]
    // private float xOffset = 100;
    [SerializeField]
    private float yOffset = 75;
    [FormerlySerializedAs("zOffset")] [SerializeField, Range(-10, 25)]
    private float armLength = 100;

    void Awake()
    {
        _camera = Camera.main;
        _camera.transform.localPosition = new Vector3(transform.position.x, transform.position.y + armLength,
            transform.position.z - armLength);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ballTransform = currentGolfController.transform;

        cameraNextPosition = new Vector3(ballTransform.position.x, ballTransform.position.y, ballTransform.position.z);

        _camera.transform.rotation =
            Quaternion.LookRotation((ballTransform.position - _camera.transform.position).normalized, Vector3.up);
        
        if (shouldLerp)
        {
            transform.position = Vector3.Lerp(transform.position, cameraNextPosition, Time.deltaTime);
        }
        else
        {
            transform.position = cameraNextPosition;
        }

        if (transform.position == oldCameraPosition)
        {
            shouldLerp = false;
        }
        oldCameraPosition = transform.position;
    }

    public void ChangeCurrentPlayerController(GolfController newController)
    {
        if (currentGolfController)
        {
            currentGolfController.CameraRotation -= HandleRotationInput;
        }
        
        currentGolfController = newController;
        currentGolfController.CameraRotation += HandleRotationInput;
    }
    
    private IEnumerator UpdateFollowTarget()
    {
       yield return new WaitForSeconds(waitTime);
       // currentGolfController = playerSpawner.ballGOs[gameManager.currentTurnIndex].GetComponent<BallController>();
       shouldLerp = true;
    }
    
    private void HandleRotationInput(Vector2 value)
    {
        if (value == Vector2.zero) return;
        
        _rotationAngles += new Vector3(-value.y, value.x, 0).normalized * rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(_rotationAngles);
    }

}
