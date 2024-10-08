
using System;
using DG.Tweening;

public class DemoTab3Content : DemoTabContent
{
    public enum DemoTab3Item
    {
        Item1,
        Item2,
        Item3,
        Item4,
        Item5,
        Item6,
        Item7,
        Item8,
        Item9,
        Item10,
        Item11,
        Item12
    }
    
    public override void Show()
    {
        base.Show();

        for (int i = 0; i < Enum.GetValues(typeof(DemoTab3Item)).Length; i++)
        {
            HCButton btn = Instantiate(itemPrefab, content);
            btn.Hide(true);
            items.Add(btn);
        }

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            DOVirtual.DelayedCall(i / 3 * .15f, () => item.Show());
        }
    }
}