using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace common
{
    public sealed class MessageContains : MonoBehaviour
    {
        public sealed class MessageContain : MonoBehaviour
        {
            #region Singleton
            //
            static MessageContain _instance;
            public static MessageContain Intance
            {
                get
                {
                    //if instance not exist, then create
                    if (_instance == null)
                    {
                        //create gameobject, and add interfaceMessage component
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<MessageContain>();
                        singletonObject.name = "Singleton-contact";
                        Common.Log("Create singleton : {0}", singletonObject.name);
                    }
                    return _instance;
                }
                private set { }
            }
            //
            void Awake()
            {
                //check another instance already exist in scence
                if (_instance != null && _instance.GetInstanceID() != this.GetInstanceID())
                {
                    Common.Log("An instance of EventDispatcher already exist : <{1}>, " +
                                "So destroy this instance : <{2}>!!", _instance.name, name);
                    Destroy(gameObject);
                }
            }
            //
            void OnDestroy()
            {
                // reset this static var to null if it's the singleton instance
                if (_instance == this)
                    _instance = null;
            }
            #endregion
            //
            //
            #region Init, declare all store Listener dictionary
            //
            Dictionary<MessageID, List<Action<Component, object>>> listener = new Dictionary<MessageID, List<Action<Component, object>>>();
            #endregion
            //
            //
            #region Add listener object, post message, wait message
            //
            public void Wait(MessageID _message, Action<Component, object> callback)
            {
                //checking parmas
                //checking if key exist in dictionary
                //only add dictionary when event not 0
                //
                if (listener.ContainsKey(_message))
                {
                    listener[_message].Add(callback);
                }
                else
                {
                    var newListener = new List<Action<Component, object>>();
                    newListener.Add(callback);
                    listener.Add(_message, newListener);
                }            
            }

            public void RemoveMessage()
            {

            }
            #endregion
            //
            //
            #region postMessage
            //
            public void Post(MessageID _message, Component sender, object param = null)
            {
                //get action list from dictionary
                List<Action<Component, object>> waitList = new List<Action<Component, object>>();
                //

                waitList.Clear();
                if (listener.TryGetValue(_message, out waitList))
                {
                    for (int i = 0, amount = waitList.Count; i < amount; i++)
                    {
                        try
                        {
                            waitList[i](sender, param);
                        }
                        catch (Exception e)
                        {
                            //remove listeer at i - that cause the exception
                            //if no listener remain then declete this key
                            Common.LogWarning(this, "Error whenn PostEvent : {0}, message : {2}", _message, e.Message);
                            waitList.RemoveAt(i);
                            if (0 == waitList.Count)
                            {
                                listener.Remove(_message);
                            }
                            amount--;
                            i--;
                        }
                    }
                }
                else
                {
                    Common.LogWarning(this, "PostEvent, event : {0}, no listener for this event", _message.ToString());
                }

            }
            //
            #endregion
            //
            //

            public void UniTest()
            {
                Debug.Log(" It's so OK");
            }

        }
        //
        //
        #region  Extension class
        //declear some shortcut for using path method
        public static class InterfaceDispather
        {
            #region add message, viva data
            //use extenstion class to register
            public static void MessageWait(this MonoBehaviour sender, MessageID _message, Action<Component, object> callback)
            {
                MessageContain.Intance.Wait(_message, callback);
            }
            //
            //
            public static void MessagePost(this MonoBehaviour sender, MessageID _message)
            {
                MessageContain.Intance.Post(_message, sender, null);
            }

            //send and get value var
            //send value when post message
            
            #endregion
            //
            //
            #region include action data
            //include action event

            #endregion
        }
        //
        #endregion
    }
}
