using System.Collections.Generic;
using System;

public class MetricChosenUpgrade : MetricAbstract
{
    //private string upgradeChosen;
    public MetricChosenUpgrade()
    {
        formTags = new List<string>();
        url = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdCz2sjnRVsnJisri7jDb6Uqfpobs1-QUBSAAlEko_y0OOfEw/formResponse";
        formTags.Add("entry.1708115493"); // tag for sessionId
        formTags.Add("entry.1149762439");  // tag for upgradeChosen

        Upgrades.OnUpgrade += ChosenUpgradePosting;
    }

    private void ChosenUpgradePosting(string upgradeChosen)
    {
        List<string> formValues = new List<string>();
        formValues.Add("space for sessionId");
        formValues.Add(upgradeChosen);
        Post(formValues);
    }
}