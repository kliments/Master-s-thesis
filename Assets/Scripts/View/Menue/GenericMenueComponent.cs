using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMenueComponent : MonoBehaviour {

    public int id = 0;
    private string name = "";
    private List<IMenueComponentListener> myListener = new List<IMenueComponentListener>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void initMe(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public int getId()
    {
        return id;
    }

    public string getName()
    {
        return name;
    }

    public bool addListener(IMenueComponentListener newListener)
    {
        if (!myListener.Contains(newListener))
        {
            myListener.Add(newListener);
            return true;
        }
        else
        {
            Debug.Log("Menue component does allready have this listener. Listener was not added again.");
            return false;
        }
    }

    public bool removeListener(IMenueComponentListener listener)
    {
        if (myListener.Contains(listener))
        {
            myListener.Remove(listener);
            return true;
        }
        else
        {
            Debug.Log("Menue component does not have this listener. Listener was not removed.");
            return false;
        }
    }

    public List<IMenueComponentListener> getListeners()
    {
        return myListener;
    }
}
