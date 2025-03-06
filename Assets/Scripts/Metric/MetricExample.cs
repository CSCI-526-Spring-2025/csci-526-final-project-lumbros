using System.Collections.Generic;
using System;

public class MetricExample : MetricAbstract
{
    private int _testInt;
    public MetricExample()
    {
        formTags = new List<string>();
        url = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfURU3mpWxk__Sm_sX44O7Pj2LEhJVbxcpuMnm0dfvBJ0QIeA/formResponse";
        formTags.Add("entry.1828615863"); // tag for sessionId
        formTags.Add("entry.866404936");  // tag for randomInt

        Random r = new Random();
        _testInt = r.Next(1, 101);
        CustomSceneManager.gameStateChange += ExamplePosting;
    }

    private void ExamplePosting(GAMESTATE gs)
    {
        if (gs == GAMESTATE.GameOver)
        {
            List<string> formValues = new List<string>();
            formValues.Add("space for sessionId");
            formValues.Add(_testInt.ToString());
            //Post(formValues);
        }
    }
}