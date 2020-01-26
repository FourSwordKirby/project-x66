using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransitionManager : MonoBehaviour {

    private static TransitionManager instance;
    public static TransitionManager Instance
    {
        get
        {
            if(instance == null)
            {
                if((instance = FindObjectOfType<TransitionManager>()) == null)
                {
                    Debug.LogError("Attempt to access TransitionManager when there is none!");
                }
            }
            return instance;
        }
    }

    public delegate void ResponseFunction();
    private event ResponseFunction Finish;

    public bool Blocking { get; private set; }

    private float transitionTime;
    private bool inTransition;
    private Animator anim;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != instance)
            {
                Destroy(this.gameObject);
            }
        }

        inTransition = false;
        transitionTime = 0.0f;
        anim = GetComponent<Animator>();
    }

	// Use this for initialization
	void Start () {
        OnLoad();
	}

    void OnLevelWasLoaded(int index)
    {
        OnLoad();
    }
	
	// Update is called once per frame
	void Update () {
        if (inTransition)
        {
            if (transitionTime <= 0.0f)
            {
                OnFinish();
                inTransition = false;
                ClearFinishEvents();
                return;
            }
            transitionTime -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            FadeToDark(null);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FadeToEmpty(null);
        }
	}

    private void OnLoad()
    {
        transform.SetSiblingIndex(1000);
    }

    public void SetScreenDark()
    {
        StartTransition("Dark", 0.0f);
    }

    public void SetScreenEmpty()
    {
        StartTransition("Empty", 0.0f);
    }

    public void FadeToEmpty(ResponseFunction onFinish)
    {
        StartTransition("ToEmpty", 1.0f, onFinish);
    }

    public void FadeToDark(ResponseFunction onFinish)
    {
        StartTransition("ToDark", 1.0f, onFinish);
    }

    public void ImageIn(ResponseFunction onFinish)
    {
        StartTransition("ImageIn", 0.5f, onFinish);
    }

    public void ImageOut(ResponseFunction onFinish)
    {
        StartTransition("ImageOut", 0.5f, onFinish);
    }

    private void StartTransition(string name, float duration,
                                 ResponseFunction onFinish = null)
    {
        if (inTransition) return;
        anim.SetTrigger(name);
        inTransition = true;
        transitionTime = duration;
        Finish += onFinish;
    }

    private void OnFinish()
    {
        if (Finish != null)
            Finish();
    }

    private void ClearFinishEvents()
    {
        Finish = null;
    }
}
