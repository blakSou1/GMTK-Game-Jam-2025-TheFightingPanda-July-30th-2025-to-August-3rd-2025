using System;

[Serializable]
public class EntityComponentDefinition
{
    public virtual void Init(CMSEntityPfb cMSEntityPfb) { }
    public virtual void OnDestroy(CMSEntityPfb cMSEntityPfb){}
}
