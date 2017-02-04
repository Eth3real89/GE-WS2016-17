using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// https://unity3d.com/de/learn/tutorials/topics/scripting/events-creating-simple-messaging-system

/// <summary>
/// The EventManager is to be used if Callbacks wouldn't make sense (for notifications that only need to happen once,
/// for instance, or events where an "unrelated" object needs to know when something happens).
/// </summary>
public class EventManager : MonoBehaviour {

    private Dictionary<string, UnityEvent> eventDictionary;

    private static EventManager s_EventManager;

    public static EventManager Instance
    {
        get
        {
            if(!s_EventManager)
            {
                s_EventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                s_EventManager.Init();
            }

            return s_EventManager;
        }
    }

    void Init ()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (s_EventManager == null) return;
        UnityEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}
