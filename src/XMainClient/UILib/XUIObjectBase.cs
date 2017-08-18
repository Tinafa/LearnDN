using UILib;
using UnityEngine;

public abstract class XUIObjectBase : MonoBehaviour, IXUIObject
{
    public virtual IXUIObject parent
    {
        get { return m_parent; }
        set { m_parent = value; }
    }

    public ulong ID
    {
        get { return m_id; }
        set { m_id = value; }
    }

    public bool Exculsive 
    { 
        get { return m_bExculsive; }
        set { m_bExculsive = value; }
    }

    public bool IsVisible() { return gameObject.activeInHierarchy; }

    public virtual void SetVisible(bool bVisible)
    {
        //UIManager.singleton.SetVisible(gameObject, bVisible);
        gameObject.SetActive(bVisible);
    }

    public virtual void OnFocus()
    {
        if (null != parent)
        {
            parent.OnFocus();
        }
    }

    public IXUIObject GetUIObject(string strName)
    {
        Transform transform = base.transform.FindChild(strName);
        if (null != transform)
        {
            return transform.GetComponent<XUIObjectBase>();
        }
        return null;
    }

    public virtual void Highlight(bool bTrue)
    {

    }

    protected virtual void OnPress(bool isPressed)
    {
        OnFocus();
    }

    protected virtual void OnDrag(Vector2 delta)
    {
        OnFocus();
    }

    public virtual void Init()
    {

    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void OnStart()
    {

    }

    protected virtual void OnUpdate()
    {

    }

    void Awake()
    {
        OnAwake();
    }

    void Start()
    {
        OnStart();
    }

    //void Update()
    //{
    //    OnUpdate();
    //}

    private IXUIObject m_parent = null;
    private ulong m_id;
    private bool m_bExculsive = false;
}
