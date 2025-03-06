using System.Collections.Generic;
using System;

public abstract class MetricAbstract
{
    // these should be define in each class
    protected string url;
    protected List<string> formTags;

    public event Action<string, List<string>, List<string>> post;

    protected void Post(List<string> formValues)
    {
        post?.Invoke(url, formTags, formValues);
    }
}
