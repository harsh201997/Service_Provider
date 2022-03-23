using AMC.EDM;
using System;
using System.Collections;
abstract class Subject
  {
    private ArrayList observers = new ArrayList();

    public void AddObserver(Observer observer)
    {
      observers.Add(observer);
    }

    public void RemoveObserver(Observer observer)
    {
      observers.Remove(observer);
    }

    public void Notify()
    {
      foreach (Observer o in observers)
      {
        o.SendEmail();
      }
    }
  }