using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelComboList {

    protected static List<AngelComboList> s_Instances;

    protected List<AngelCombo> m_Combos;
    protected List<int> m_StartIndices;
    protected List<int> m_UsedCombos;

    private static void UpdateLists(AngelCombo combo)
    {
        for(int i = 0; i < s_Instances.Count; i++)
        {
            if (s_Instances[i] == null)
                continue;

            if (s_Instances[i].Contains(combo))
            {
                s_Instances[i].m_UsedCombos.Add(s_Instances[i].m_Combos.IndexOf(combo));
            }
        }
    }

    public AngelComboList()
    {
        if (s_Instances == null)
        {
            s_Instances = new List<AngelComboList>();
        }
        s_Instances.Add(this);

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

        if (m_Combos.Count > 1 && m_UsedCombos.Count >= 1 && m_UsedCombos[m_UsedCombos.Count - 1] == comboIndex)
        {
            comboIndex = GetRandomCombo();
        }

        UpdateLists(m_Combos[comboIndex]);

        if (m_UsedCombos.Count > 15)
        {
            m_UsedCombos.RemoveAt(0);
        }

        return comboIndex;
    }

    public int ComboIndex(AngelCombo angelCombo)
    {
        return m_Combos.IndexOf(angelCombo);
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
