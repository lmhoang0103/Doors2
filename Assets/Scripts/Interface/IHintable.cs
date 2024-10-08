using System;

public interface IHintable
{
    public event EventHandler OnHintShow;
    public event EventHandler OnHintHidden;
}
