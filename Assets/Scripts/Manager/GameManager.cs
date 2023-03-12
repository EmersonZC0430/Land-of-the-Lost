using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public CharacterStats playerStates;

    /* 观察者模式反向注册的方法 */


    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();




    public void RigisterPlayer(CharacterStats player)
    {
        playerStates = player;
    }



    public void AddObservers(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObservers(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }


}
