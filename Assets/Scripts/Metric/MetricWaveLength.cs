using System.Collections.Generic;
using System;
using System.Diagnostics;

public class MetricWaveLength : MetricAbstract
{
    private Stopwatch stopwatch;
    private int wave;
    public MetricWaveLength()
    {
        formTags = new List<string>();
        url = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSd3vs2YHy364rwA2AvFTkrbAUlaySUFeQtyuHXE0gpaK4vEZQ/formResponse";
        formTags.Add("entry.1693662383"); // tag for sessionId
        formTags.Add("entry.671978643");  // tag for curWave
        formTags.Add("entry.620868095"); // tag for secs it took to finish the wave

        stopwatch = new Stopwatch();

        WaveManager.waveBegin += WaveBegin;
        WaveManager.waveEnd += WaveEnd;
    }

    private void WaveBegin(int curWave, int waveKillLimit)
    {
        wave = curWave;
        stopwatch.Start();
    }

    private void WaveEnd(int curWave)
    {
        stopwatch.Stop();
        TimeSpan timeTaken = stopwatch.Elapsed;

        List<string> formValues = new List<string>();
        formValues.Add("space for sessionId");
        formValues.Add(wave.ToString());
        formValues.Add(timeTaken.TotalSeconds.ToString());
        Post(formValues);

        stopwatch.Reset();
    }
}