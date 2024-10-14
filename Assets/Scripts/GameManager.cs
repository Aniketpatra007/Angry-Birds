using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int MaxNumberOfShots = 3;
    [SerializeField] private float _secondsToWaitBeforeDeath = 3f;
    [SerializeField] private GameObject _restartScreenObject;

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
            LoseGame();
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
        Debug.Log("Win");
        _restartScreenObject.SetActive(true);
    }

    private void LoseGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
}