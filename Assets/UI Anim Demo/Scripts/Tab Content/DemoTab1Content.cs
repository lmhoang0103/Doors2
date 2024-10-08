
using System;
using UnityEngine;

public class DemoTab1Content : DemoTabContent
{
    public enum DemoTab1Item
    {
        Item1,
        Item2,
        Item3,
        Item4,
        Item5,
        Item6
    }
    
    public override void Show()
    {
        base.Show();

        for (int i = 0; i < Enum.GetValues(typeof(DemoTab1Item)).Length; i++)
        {
            HCButton btn = Instantiate(itemPrefab, content);
            btn.Hide(true);
            items.Add(btn);
        }
        
        items.ForEach(item => item.Show());
    }
}