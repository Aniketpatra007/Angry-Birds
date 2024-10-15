using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;

    [Header("Transform References")]
    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _idlePosition;
    [SerializeField] private Transform _elasticTransform;

    [Header("SlingShot Stats")]
    [SerializeField] private float _maxDistance = 3.5f;
    [SerializeField] private float _shotForce = 9f;
    [SerializeField] private float _timeBetweenBirdRespawns = 2f;
    [SerializeField] private float _elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve _elasticCurve;
    [SerializeField] private float _maxAnimationTime = 1f;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea _slingShotArea;
    [SerializeField] private CameraManager _cameraManager;

    [Header("Bird")]
    [SerializeField] private AngieBird _angieBirdPrefab;
    [SerializeField] private float _angieBirdPositionOffset = 2f;

    [Header("Sounds")]
    [SerializeField] private AudioClip _elasticPulledClip;
    [SerializeField] private AudioClip _elasticReleasedClip;

    private Vector2 _slingShotLinesPosition;
    private Vector2 _direction;
    private Vector2 _directionNormalized;

    private bool _clickedWithinArea;
    private bool _birdOnSlingShot;

    private AngieBird _spawnedAngieBird;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>(); 
        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;

        SpawnAngieBird();
    }

    void Update()
    {
        if (InputManager.WasLeftMouseButtonPressed && _slingShotArea.isWithinSlingShotArea())
        {
            _clickedWithinArea = true;

            if(_birdOnSlingShot)
            {
                SoundManager.instance.PlayClip(_elasticPulledClip, _audioSource);
                _cameraManager.SwitchToFollowCam(_spawnedAngieBird.transform);
            }
        }

        if(InputManager.IsLeftMousePressed && _clickedWithinArea && _birdOnSlingShot)
        {
            DrawSlingShot();
            PositionAndRotateAngieBird();
        }
        if (InputManager.WasLeftMouseButtonReleased && _birdOnSlingShot && _clickedWithinArea)
        {
            if (GameManager.Instance.HasEnoughShots())
            {
                _clickedWithinArea = false;
                _birdOnSlingShot = false;

                _spawnedAngieBird.LaunchBird(_direction, _shotForce);

                SoundManager.instance.PlayClip(_elasticReleasedClip, _audioSource);
                GameManager.Instance.UseShot();
                AnimateSlingShot();
               
                if (GameManager.Instance.HasEnoughShots())
                {
                    StartCoroutine(SpawnAngieBirdAfterTime());

                }
            }
        }
    }


    #region SlingShot Methods
    private void DrawSlingShot()
    {
        

        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position, _maxDistance);

        SetLines(_slingShotLinesPosition);

        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if(!_leftLineRenderer.enabled && !_rightLineRenderer.enabled)
        {
            _leftLineRenderer.enabled=true;
            _rightLineRenderer.enabled=true;
        }
        _leftLineRenderer.SetPosition(0, position);
        _leftLineRenderer.SetPosition(1,_leftStartPosition.position);

        _rightLineRenderer.SetPosition(0,position);
        _rightLineRenderer.SetPosition(1,_rightStartPosition.position);
    }

    #endregion

    #region Angie Bird Methods

    private void SpawnAngieBird()
    {
        _elasticTransform.DOComplete();
        SetLines(_idlePosition.position);

        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)_idlePosition.position + dir * _angieBirdPositionOffset;

        _spawnedAngieBird =  Instantiate(_angieBirdPrefab, spawnPosition, Quaternion.identity);
        _spawnedAngieBird.transform.right = dir;


        _birdOnSlingShot = true;
    }

    private void PositionAndRotateAngieBird()
    {
        _spawnedAngieBird.transform.position = _slingShotLinesPosition + _directionNormalized *_angieBirdPositionOffset;
        _spawnedAngieBird.transform.right = _directionNormalized;
    }


    private IEnumerator SpawnAngieBirdAfterTime()
    {
        yield return new WaitForSeconds(_timeBetweenBirdRespawns);

        //some code
        SpawnAngieBird();
        _cameraManager.SwitchToIdleCam();
    }
    #endregion

    #region Animate SlingShot

    private void AnimateSlingShot()
    {
        _elasticTransform.position = _leftLineRenderer.GetPosition(0);

        float dist = Vector2.Distance(_elasticTransform.position, _centerPosition.position);

        float time = dist / _elasticDivider;

        _elasticTransform.DOMove(_centerPosition.position, time).SetEase(_elasticCurve);
        StartCoroutine(AnimateSlingShotLines(_elasticTransform, time));
    }

    private IEnumerator AnimateSlingShotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while(elapsedTime < time && elapsedTime < _maxAnimationTime)
        {
            elapsedTime += Time.deltaTime;

            SetLines(trans.position);

            yield return null;
        }
    }

    #endregion
}
