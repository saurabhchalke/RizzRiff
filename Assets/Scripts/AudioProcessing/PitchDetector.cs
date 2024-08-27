using UnityEngine;
using System;

public class PitchDetector
{
    private int bufferSize;
    private int sampleRate;
    private float[] yinBuffer;
    private const float DEFAULT_THRESHOLD = 0.15f;

    public PitchDetector(int bufferSize, int sampleRate)
    {
        this.bufferSize = bufferSize;
        this.sampleRate = sampleRate;
        this.yinBuffer = new float[bufferSize / 2];
    }

    public float DetectPitch(float[] audioBuffer)
    {
        int tauEstimate = -1;
        float pitchInHz = 0f;

        DifferenceFunction(audioBuffer);
        CumulativeMeanNormalizedDifferenceFunction();
        tauEstimate = AbsoluteThreshold();

        if (tauEstimate != -1)
        {
            float betterTau = ParabolicInterpolation(tauEstimate);
            pitchInHz = sampleRate / betterTau;
        }

        return pitchInHz;
    }

    private void DifferenceFunction(float[] audioBuffer)
    {
        for (int tau = 0; tau < yinBuffer.Length; tau++)
        {
            yinBuffer[tau] = 0;
        }

        for (int tau = 1; tau < yinBuffer.Length; tau++)
        {
            for (int i = 0; i < yinBuffer.Length; i++)
            {
                float delta = audioBuffer[i] - audioBuffer[i + tau];
                yinBuffer[tau] += delta * delta;
            }
        }
    }

    private void CumulativeMeanNormalizedDifferenceFunction()
    {
        float runningSum = 0;
        yinBuffer[0] = 1;

        for (int tau = 1; tau < yinBuffer.Length; tau++)
        {
            runningSum += yinBuffer[tau];
            yinBuffer[tau] *= tau / runningSum;
        }
    }

    private int AbsoluteThreshold()
    {
        for (int tau = 2; tau < yinBuffer.Length; tau++)
        {
            if (yinBuffer[tau] < DEFAULT_THRESHOLD)
            {
                while (tau + 1 < yinBuffer.Length && yinBuffer[tau + 1] < yinBuffer[tau])
                {
                    tau++;
                }
                return tau;
            }
        }
        return -1;
    }

    private float ParabolicInterpolation(int tauEstimate)
    {
        float betterTau;
        int x0 = (tauEstimate < 1) ? tauEstimate : tauEstimate - 1;
        int x2 = (tauEstimate + 1 < yinBuffer.Length) ? tauEstimate + 1 : tauEstimate;
        
        if (x0 == tauEstimate)
        {
            betterTau = (yinBuffer[tauEstimate] <= yinBuffer[x2]) ? tauEstimate : x2;
        }
        else if (x2 == tauEstimate)
        {
            betterTau = (yinBuffer[tauEstimate] <= yinBuffer[x0]) ? tauEstimate : x0;
        }
        else
        {
            float s0 = yinBuffer[x0];
            float s1 = yinBuffer[tauEstimate];
            float s2 = yinBuffer[x2];
            betterTau = tauEstimate + (s2 - s0) / (2 * (2 * s1 - s2 - s0));
        }
        return betterTau;
    }
}