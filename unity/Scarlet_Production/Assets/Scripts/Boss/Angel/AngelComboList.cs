using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelComboList {

    protected static List<AngelCombo> s_Combos;
    protected static List<int> s_Used;

    protected List<AngelCombo> m_Combos;
    protected List<int> m_StartIndices;
    protected List<int> m_UsedCombos;

    public AngelComboList()
    {
        if (s_Combos == null)
        {
            s_Combos = new List<AngelCombo>();
            s_Used = new List<int>();
        }

        m_Combos = new List<AngelCombo>();
        m_StartIndices = new List<int>();
        m_UsedCombos = new List<int>();
    }

    public void AddCombo(AngelCombo combo, int startIndex = -1)
    {
        m_Combos.Add(combo);
        m_StartIndices.Add(startIndex);
    }

    public bool Contains(AngelCombo combo)
    {
        return m_Combos.Contains(combo);
    }

    public int GetRandomCombo()
    {
        int comboIndex = 0;
        float[] probabilities = DetermineProbabilities();

        float value = UnityEngine.Random.value - 0.001f;
        for (int i = 0; i < m_Combos.Count; i++)
        {
            if (probabilities[i] >= value)
            {
                comboIndex = i;
                break;
            }
        }
        m_UsedCombos.Add(comboIndex);

        if (m_UsedCombos.Count > 15)
        {
            m_UsedCombos.RemoveAt(0);
        }

        return comboIndex;
    }

    public AngelCombo ComboAt(int i)
    {
        return m_Combos[i];
    }

    public int StartIndexAt(int i)
    {
        return m_StartIndices[i];
    }

    private float[] DetermineProbabilities()
    {
        float[] probabilities = new float[m_Combos.Count];
        for (int i = 0; i < m_Combos.Count; i++)
        {
            probabilities[i] = DetermineProbability(i);
        }

        float total = 0;
        for(int i = 0; i < probabilities.Length; i++)
        {
            total += probabilities[i];
        }

        for (int i = 0; i < probabilities.Length; i++)
        {
            probabilities[i] *= 1 / total;
            if (i != 0)
                probabilities[i] += probabilities[i - 1];
        }

        return probabilities;
    }

    private float DetermineProbability(int comboIndex)
    {
        return (1f - (CountOccurences(comboIndex) / (m_UsedCombos.Count + 1))) / m_Combos.Count;
    }

    private int CountOccurences(int comboIndex)
    {
        int count = 0;
        for(int i = 0; i < m_UsedCombos.Count; i++)
        {
            if (m_UsedCombos[i] == comboIndex)
                count++;
        }
        return count;
    }
}
