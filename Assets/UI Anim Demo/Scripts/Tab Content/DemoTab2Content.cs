
using System;
using DG.Tweening;

public class DemoTab2Content : DemoTabContent
{
    public enum DemoTab2Item
    {
        Item1,
        Item2,
        Item3,
        Item4,
        Item5,
        Item6,
        Item7,
        Item8,
        Item9
    }
    
    public override void Show()
    {
        base.Show();

        for (int i = 0; i < Enum.GetValues(typeof(DemoTab2Item)).Length; i++)
        {
            HCButton btn = Instantiate(itemPrefab, content);
            btn.Hide(true);
            items.Add(btn);
        }

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            DOVirtual.DelayedCall(.1f * i, () => item.Show());
        }
    }
}