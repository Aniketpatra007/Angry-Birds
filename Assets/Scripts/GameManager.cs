using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int MaxNumberOfShots = 3;
    [SerializeField] private float _secondsToWaitBeforeDeath = 3f;
    [SerializeField] private GameObject _restartScreenObject;
    [SerializeField] private SlingShotHandler _slingShotHandler;
    [SerializeField] private Image _nextLevelImage;

    private int _usedNumberOfShots;

    private IconHandler _iconHandler;


    private List<Baddie> _baddies = new List<Baddie>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _iconHandler = GameObject.FindObjectOfType<IconHandler>();

        Baddie[] baddies = FindObjectsOfType<Baddie>();

        for(int i = 0; i <  baddies.Length; i++)
        {
            _baddies.Add(baddies[i]);
        }
        _nextLevelImage.enabled = false;
    }

    public void UseShot()
    {
        _usedNumberOfShots++;
        _iconHandler.UseShot(_usedNumberOfShots);
        //Debug.Log(_usedNumberOfShots);
        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {
        if(_usedNumberOfShots < MaxNumberOfShots)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckForLastShot()
    {
        if(_usedNumberOfShots == MaxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(_secondsToWaitBeforeDeath);

        if(_baddies.Count == 0)
        {
            WinGame();
        }
        else
        {
            RestartGame();
        }
    }

    public void RemoveBaddie(Baddie baddie)
    {
        _baddies.Remove(baddie);
        CheckForAllDeadBaddies();
    }

    private void CheckForAllDeadBaddies()
    {
        if(_baddies.Count == 0)
        {
            WinGame();
        }
    }

    #region Win/lose
    private void WinGame()
    {
        _restartScreenObject.SetActive(true);
        _slingShotHandler.enabled = false;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels = SceneManager.sceneCountInBuildSettings;

        if(currentSceneIndex+1 < maxLevels)
        {
            _nextLevelImage.enabled = true;
        }
    }

    public void RestartGame()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
