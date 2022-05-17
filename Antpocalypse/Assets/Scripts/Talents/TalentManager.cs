using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentManager : MonoBehaviour
{
    

    struct TalentContainer
    {
        public Talent talent;
        public float popupChance;
    }

    List<TalentContainer> generalTalents;

    // Start is called before the first frame update
    void Start()
    {
       /* GenerateTalentList();


        foreach (TalentContainer item in generalTalents)
        {
            Debug.Log(item.talent.Name + " with a chance of: " + item.popupChance);
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Talent[] GetChosenTalents(int n)
    {
        Talent[] chosen = new Talent[n];

        GenerateTalentList();

        for (int i = 0; i < n; i++)
        {
            chosen[i] = generalTalents[i].talent;
        }

        return chosen;

    }

    public void GenerateTalentList()
    {
        generalTalents = new List<TalentContainer>();

        Talent[] rawTalents = GetComponents<Talent>();
        for (int i = 0; i < rawTalents.Length; i++)
        {
            float chance = Random.Range(rawTalents[i].GetMinChance(), rawTalents[i].GetMaxChance());

            TalentContainer cont = new TalentContainer();
            cont.talent = rawTalents[i];
            cont.popupChance = chance;

            generalTalents.Add(cont);

           // Debug.Log(rawTalents[i].Name + " with a chance of: " + chance);
        }

        QuickSort(generalTalents);

        List<TalentContainer> newGeneralTalentList = new List<TalentContainer>();
        foreach (TalentContainer talentCont in generalTalents)
        {
            if (talentCont.talent.talentLevel < talentCont.talent.maxLevel)
            {
                newGeneralTalentList.Add(talentCont);
            }
        }

        generalTalents = newGeneralTalentList;

    }

    void QuickSort(List<TalentContainer> array)
    {
        QuickSort(array, 0, array.Count - 1);
    }

    void QuickSort(List<TalentContainer> array, int left, int right)
    {
        if (left >= right)
            return;

        // pivot pick
        int pivot = (left + right) / 2;
        int index = partition(array, left, right, pivot);

        QuickSort(array, left, index - 1);
        QuickSort(array, index, right);
    }
    int partition(List<TalentContainer> array, int left, int right, int pivot)
    {
        while (left <= right)
        {
            while (array[left].popupChance > array[pivot].popupChance) left++;
            while (array[right].popupChance < array[pivot].popupChance) right--;

            if (left <= right)
            {
                TalentContainer temp = array[left];
                array[left] = array[right];
                array[right] = temp;

                left++;
                right--;
            }
        }

        return left;
    }
}
